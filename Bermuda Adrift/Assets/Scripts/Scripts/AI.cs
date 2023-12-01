using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AI : MonoBehaviour
{
    public event Action<int> OnEnemyHurt;
    public event Action<int> OnCrit;
    public event Action<int, Color> OnStatusDamage;

    public event Action<int> SetupHealthBar;

    private Enemy enemy;
    private GameObject enemyManager;

    private Enemy Minion;
    [SerializeField] private GameObject prefab;

    [SerializeField] private GameObject extra;
    [SerializeField] private Bullet bullet;

    private Transform movement;

    private Transform goal;
    private GameObject goalGO;

    private bool arrived;
    private int Health;
    private int damage;
    private bool stop;
    private bool dead;
    private bool forgotten = false;

    [SerializeField] private bool noRotation;
    private Animator animator;
    private Buffs debuffToInflict;
    private Buffs[] debuffs;

    #region Setup and Update
    public void setEnemy(Enemy newEnemy)    //Setup function when told what type of enemy to be
    {
        enemyManager = GameObject.FindGameObjectWithTag("Managers");
        enemy = newEnemy;
        debuffs = new Buffs[10];

        Health = (int) (enemy.getHealth() * enemyManager.GetComponent<EnemyManager>().getRoundScale());    //Maybe add health scaling later based on the number of times the set of enemies has been picked?
        gameObject.GetComponent<Animator>().runtimeAnimatorController = enemy.getAnim();
        debuffToInflict = enemy.getDefuff();

        if (enemy.getExtra() != null)
            extra = enemy.getExtra();
        if (enemy.getBullet() != null)
            bullet = enemy.getBullet();

        gameObject.GetComponent<BoxCollider2D>().size = new Vector3(enemy.getXSize(), enemy.getYSize());    //Sets size of the hitbox. Might need another variable for the offset from the center since some of the sprites aren't centered

        if (enemy.getType() == Enemy.Types.WaterBoss || enemy.getType() == Enemy.Types.AirborneBoss)    //Start attack cycle if it's a boss
        {    
            StartCoroutine(randomAttacks());
            Health = (int) (Health * Mathf.Pow(1.15f, GameObject.Find("Managers").GetComponent<EnemyManager>().getRound() / 10f));
            Debug.Log("Health: " + Health);
        }

        if (extra != null && extra.name == "Buff_EnemyBuff" && debuffToInflict != null)
            StartCoroutine(Buffs());
        else if (extra != null && extra.name == "Bullet_EnemyBullet" && bullet != null)
            StartCoroutine(Bullets());


        SetupHealthBar?.Invoke(Health);
    }
    public void setMinion(Enemy newMinion) { Minion = newMinion; }  //Sets the minion when the boss spawns in. Should be random
    private void Start()    //Set up what setEnemy didn't
    {
        //setEnemy seems to run faster than Start, so there should be nothing set here that's set in setEnemy
        animator = gameObject.GetComponent<Animator>();
        movement = gameObject.transform;

        nearestEntrance();  //Sets the entrance to be going towards

        turn();
    }
    void Update()   //Temp buttons, move, and check if the enemy has arrived at the center
    {
        //if (Input.GetKeyDown("p"))
        //    TakeDamage(Health);

        healthCheck();    //Constantly update health to deal with health buffs being added/wearing off

        if (!arrived)
        {
            move();

            if (Mathf.Abs(movement.position.x) + Mathf.Abs(movement.position.y) <= 2.5)  //Stop when they reach the center and set position to a spot depending on the lane they're in. This needs to be redone at some point, maybe use hitboxes?
            {
                nowArriving();
            }
        }
    }
    #endregion


    #region Movement functions
    private void move() //Moves towards the goal
    {
        turn();

        if (goal != null && goal.position != null && !stop)
        {
            movement.position = Vector3.MoveTowards(movement.position, goal.position, enemy.getSpeed() * Time.deltaTime * 0.5f * getSpeedMult()); ;
            if (movement.position == goal.position && !getDistracted())
                enteringChannel();
        }
    }
    private void turn() //Face towards current goal
    {
        if (goal != null && !noRotation)    //Face towards goal. Default is noRotation being false now. This function can be modified once we standardize the artstyle and the default direction
        {
            //Only rotates once, so if we do knockback or something we'll have to put this in the Update function or the TakeDamage function
            float z = (Mathf.Atan2(movement.position.y - goal.position.y, movement.position.x - goal.position.x) * (180 / Mathf.PI)); //Atan2 gives inverse tan in radians from current cordinates, transform takes degrees
            movement.localRotation = Quaternion.Euler(0, 0, z);  //Faces towards goal (If sprites faces left)
        }
    }
    private void nowArriving()  //Sets the position that the enemy will be attacking the center form
    {
        arrived = true;     //This section of code will only run once
        animator.SetBool("Attacking", true);    //Start animations

        if (movement.position.x < 0) { movement.SetPositionAndRotation(new Vector3(-2.5f, 0), movement.rotation); }        //Left lane
        else if (movement.position.x > 0) { movement.SetPositionAndRotation(new Vector3(2.5f, 0), movement.rotation); }    //Right lane
        else if (movement.position.y > 0) { movement.SetPositionAndRotation(new Vector3(0, 2.5f), movement.rotation); }    //Top lane
        else { movement.SetPositionAndRotation(new Vector3(0, -2.5f), movement.rotation); }                                //Bottom Lane

        StartCoroutine(attack(goal.gameObject));
    }
    #endregion

    #region Projectile/Buff enemies
    IEnumerator Buffs()
    {
        Buffs buff = debuffToInflict;
        debuffToInflict = null;
        while (gameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(enemy.getAttackSpeed() * getAttackSpeed() * 2f);

            var effect = Instantiate(extra, gameObject.transform.position, Quaternion.identity);
            effect.SendMessage("setBuff", buff);
            //effect.SendMessage("setRadius", 4);

            yield return new WaitForSeconds(1f);
            Destroy(effect);
        }
    }
    IEnumerator Bullets()
    {
        while (gameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(enemy.getAttackSpeed() * getAttackSpeed() * 2f);

            Vector3 offset = Vector3.zero - transform.position;
            Quaternion output = Quaternion.LookRotation(Vector3.forward, offset).normalized;
            var boolet = Instantiate(extra, gameObject.transform.position, Quaternion.Euler(output.eulerAngles - (output.eulerAngles / 2)));
            boolet.SendMessage("setBullet", bullet);
        }
    }
    #endregion

    #region Boss attack loop
    IEnumerator randomAttacks() //Every 10 seconds, pick a random attack to do
    {
        while (gameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(10f);

            if (goal.position == Vector3.zero) //If it's in a channel, no more atacks
                break;

            int random = Random.Range(0, 3);
            if (random == 0)
                StartCoroutine(Minions());

            else if (random == 1)
                StartCoroutine(Heal());

            else if (random == 2)
                StartCoroutine(Resurface());
        }
    }
    IEnumerator Minions()   //Summon minions behind it
    {
        Debug.Log("Minions");
        stop = true;    //Stop while spawning
        int total = enemyManager.GetComponent<EnemyManager>().getRound() / 10 + 2;  //start with 3 minions + 1 more every 10 rounds

        if (gameObject.transform.position.x < -4)   //Spawns the enemies behind itself, so different relative positions depending on what side of the screen they're on
        {
            for (int i = 0; i < total; i++)
            {
                yield return new WaitForSeconds(0.5f);  //Half-second gap between each minion
                var minion = Instantiate(prefab, new Vector3(gameObject.transform.position.x - 5, gameObject.transform.position.y + 5), Quaternion.identity);   //Maybe an animation for when they get summoned?
                minion.SendMessage("setEnemy", Minion);
                enemyManager.GetComponent<EnemyManager>().newEnemies(); //Let the Enemy manager know more enemies are being spawned

                i++;
                if (i < total)  //Spawn behind and above, then behind and below. Sometimes the total is 0 when you get to the behind and below
                {
                    yield return new WaitForSeconds(0.5f);
                    minion = Instantiate(prefab, new Vector3(gameObject.transform.position.x - 5, gameObject.transform.position.y + -5), Quaternion.identity);
                    minion.SendMessage("setEnemy", Minion);
                    enemyManager.GetComponent<EnemyManager>().newEnemies(); //Let the Enemy manager know more enemies are being spawned
                }
            }
        }
        else if (gameObject.transform.position.x > 4)   //Right side of the screen
        {
            for (int i = 0; i < total; i++)
            {
                yield return new WaitForSeconds(0.5f);
                var minion = Instantiate(prefab, new Vector3(gameObject.transform.position.x + 5, gameObject.transform.position.y + 5), Quaternion.identity);
                minion.SendMessage("setEnemy", Minion);
                enemyManager.GetComponent<EnemyManager>().newEnemies(); //Let the Enemy manager know more enemies are being spawned

                i++;
                if (i < total)
                {
                    yield return new WaitForSeconds(0.5f);
                    minion = Instantiate(prefab, new Vector3(gameObject.transform.position.x + 5, gameObject.transform.position.y + -5), Quaternion.identity);
                    minion.SendMessage("setEnemy", Minion);
                    enemyManager.GetComponent<EnemyManager>().newEnemies(); //Let the Enemy manager know more enemies are being spawned
                }
            }
        }

        stop = false;   //Resume
    }
    IEnumerator Resurface() //Dive underwater and reappear at a random spot on the other side of the screen. Could come up really far away, could come up really close
    {
        Debug.Log("Resurface");
        stop = true;    //Stop to dive under
        if (gameObject.transform.position.x < 0)    //Start on left side
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;   //Can't be shot while underwater
            //Play going-under animation
            yield return new WaitForSeconds(1f);
            gameObject.transform.position = new Vector3(Random.Range(8f, 20f), Random.Range(-10f, 10f));    //Pick random spot on right side of screen
            nearestEntrance();  //Switch the goal to the entrance that's nearest now
            yield return new WaitForSeconds(1f);
            //Play coming up animation
            gameObject.GetComponent<BoxCollider2D>().enabled = true;    //can now be shot again
        }
        else  //Start on Right Side
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            //Play going-under animation
            yield return new WaitForSeconds(1f);
            gameObject.transform.position = new Vector3(Random.Range(-20f, -8f), Random.Range(-10f, 10f));  //Random spot on left side of the screen
            nearestEntrance();
            yield return new WaitForSeconds(1f);
            //Play coming up animation
            gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }
        stop = false;   //Resume going towards raft
    }
    IEnumerator Heal()  //Heal 10% of their current health
    {
        Debug.Log("Heal");
        stop = true;    //Stop to heal
        yield return new WaitForSeconds(1f);

        int toHeal = (int)(Health * 0.1f);
        OnEnemyHurt?.Invoke(-toHeal);
        Health += toHeal; //Is 10% of their current health more balanced or their max health? The max health is always going to be more than what the current health will heal, but is that a good or bad thing?
        if (Health > enemy.getHealth()) //To prevent overhealing (or maybe we'd want them to be able to do that?
            Health = enemy.getHealth();

        yield return new WaitForSeconds(1f);

        stop = false;   //Resume
    }
    #endregion

    #region Goal Setters
    private void nearestEntrance()  //Finds the closest object tagged entrance and sets the goal to be that
    {
        GameObject[] entrances;

        entrances = GameObject.FindGameObjectsWithTag("Entrance");
        goal = null;
        float distance = Mathf.Infinity;
        Vector3 pos = movement.position;
        foreach (GameObject go in entrances)    //Iterate through all entrances, picking the closest one. Should only be 4 items, so efficiency shouldn't be a huge problem
        {
            Vector3 diff = go.transform.position - pos;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                goal = go.transform;
                distance = curDistance;
            }
        }

        if ((goal.position.x < 0 || goal.position.y > 0) && gameObject.GetComponent<DirectionalAnimations>() == null)   //Flips the sprite if it's on the left/top of the screen. If we go fully top-down with the sprites, we can get rid of this
        {
            if (noRotation)
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = !gameObject.GetComponent<SpriteRenderer>().flipX;
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().flipY = !gameObject.GetComponent<SpriteRenderer>().flipY;
            }
        }
    }
    void enteringChannel()  //Set the goal to be the centerpiece and rotate to point at it
    {
        if (Math.Abs(goal.position.x) == 7.5 || Math.Abs(goal.position.y) == 7.5)
        {
            goalGO = GameObject.FindGameObjectWithTag("Center");
            goal = goalGO.transform;    //Set new goal to the centerpiece. Could maybe just set the goal to be 0,0 for efficiency and so we don't need the centerpiece tag

            animator.SetTrigger("InChannel");

            turn();
        }
    }
    #endregion

    #region TakeDamage functions
    private void TakeDamage(int moreDamage) //Receiver for damage numbers, go to debuff function if debuffed
    {
        if (enemy.getSpecialType() == Enemy.SpecialTypes.Immune) return;

        Health -= (int) (moreDamage * getArmor());
        damage += (int) (moreDamage * getArmor());
        animator.SetTrigger("TookDamage");

        //Debug.Log("Normal: -" + (moreDamage * getArmor()) + " => " + Health + " / " + enemy.getHealth());

        OnEnemyHurt?.Invoke((int)(moreDamage * getArmor()));
        if (Health <= 0 && !dead)   //Need the dead check or it'll count multiple deaths per enemy
            death();
    }
    private void CritDamage(int moreDamage)
    {
        if (enemy.getSpecialType() == Enemy.SpecialTypes.Immune) return;

        Health -= (int)(moreDamage * getArmor());
        damage += (int)(moreDamage * getArmor());
        animator.SetTrigger("TookDamage");

        //Debug.Log("Crit: -" + (moreDamage * getArmor()) + " => " + Health + " / " + enemy.getHealth());

        OnCrit?.Invoke((int)(moreDamage * getArmor()));
        if (Health <= 0 && !dead)   //Need the dead check or it'll count multiple deaths per enemy
            death();
    }
    private void StatusDamage(int moreDamage, Color color)
    {
        if (enemy.getSpecialType() == Enemy.SpecialTypes.Immune) return;

        Health -= moreDamage;
        damage += moreDamage;
        animator.SetTrigger("TookDamage");

        //Debug.Log("Status: -" + moreDamage + " => " + Health + " / " + enemy.getHealth());

        OnStatusDamage?.Invoke((int)(moreDamage * getArmor()), color);
        if (Health <= 0 && !dead)   //Need the dead check or it'll count multiple deaths per enemy
            death();
    }
    private void death()    //Updates enemyManager count, adds scrap and XP, and deletes GameObject
    {
        enemyManager.SendMessage("addScrap", enemy.getScrap());
        enemyManager.SendMessage("addXP", enemy.getXP());

        enemyManager.SendMessage("EnemyDown");
        stop = true;
        dead = true;

        animator.SetBool("Dead", true);

        if (!goal.CompareTag("Entrance") && !goal.CompareTag("Center")) Destroy(goal.gameObject);
        //new WaitForSeconds(0.5f);  //Maybe a standard death animation length or a variable in Enemy? Is it possible to wait until an animation is done?
        Destroy(gameObject);
    }
    #endregion

    #region Attack/Heal
    private IEnumerator attack(GameObject recipient)
    {
        stop = true;

        while (recipient != null)
        {
            recipient.SendMessage("TakeDamage", (int)(enemy.getDamage() * getDamageMult()));

            if (debuffToInflict != null)
                recipient.SendMessage("InflictDebuff", debuffToInflict);

            yield return new WaitForSeconds(enemy.getAttackSpeed() * getAttackSpeed());
            yield return new WaitForEndOfFrame();
        }

        animator.SetBool("Attacking", false);
        stop = false;
    }
    private void heal(int health)   //maybe an enemy that heals other enemies?
    {
        Health += health;
        OnEnemyHurt?.Invoke(-health);
    }
    #endregion


    //For the animator
    public Transform getGoal() { return goal; } //Only used in the directional animation script

    private void Barrier(GameObject barrier) //Goes into attacking mode until the barrier is destroyed
    {
        if (barrier.GetComponent<Barriers>().getEffect() == BarrierScriptable.Effect.Blockade) {
            StartCoroutine(attack(barrier));    //Start attacking
        } else
        {
            StartCoroutine(InflictDebuff(barrier.GetComponent<Barriers>().getDebuff()));
        }

    }

    #region Debuff Managing
    private void addDebuff(Buffs debuff)    //Add a debuff to the list of debuffs
    {
        for (int i = 0; i < debuffs.Length; i++)
        {
            if (debuffs[i] == null)
            {
                debuffs[i] = debuff;
                return;
            }
        }
    }
    private void removeDebuff(Buffs debuff) //Removes a debuff from the list of debuffs currently applied
    {
        int i = 0;
        for (; i < debuffs.Length; i++)
        {
            if (debuffs[i] == debuff)
            {
                for (; i < debuffs.Length - 1; i++)
                {
                    debuffs[i] = debuffs[i + 1];
                }
                debuffs[i] = null;
            }
        }
    }
    private IEnumerator InflictDebuff(Buffs newDebuff)  //adds a debuff to the list
    {
        if (newDebuff == null) { yield break; }
        addDebuff(newDebuff);

        if (newDebuff.getStunned())
            stop = true;

        //if (getDistracted())
        //    baited(new Vector3(Random.value * gameObject.transform.position.x + gameObject.transform.position.x, Random.value * gameObject.transform.position.y + gameObject.transform.position.y));

        StartCoroutine(DOT(newDebuff.getDOT(), newDebuff.getDOTSpeed(), newDebuff.getDuration(), newDebuff.getPercentDOT(), newDebuff.getDOTColor()));

        yield return new WaitForSeconds(newDebuff.getDuration());

        stop = false;

        removeDebuff(newDebuff);
    }
    private void healthCheck()
    {
        int expectedHealth = (int) (enemy.getHealth() * enemyManager.GetComponent<EnemyManager>().getRoundScale());
        if (enemy.getType() == Enemy.Types.WaterBoss || enemy.getType() == Enemy.Types.AirborneBoss)
        {
            expectedHealth = (int)(expectedHealth * Mathf.Pow(1.15f, GameObject.Find("Managers").GetComponent<EnemyManager>().getRound() / 10f) * getHealthMult()) - damage;
        } else
            expectedHealth = (int)(expectedHealth * getHealthMult()) - damage;

        if (expectedHealth < Health)    //Bring health down when the buff wears off
        {
            int tempDamage = damage;
            StatusDamage(Health - expectedHealth, Color.red);    //Could kill the enemy, which is taken care of in the take damage function
            damage = tempDamage;
        }
    }
    private IEnumerator DOT(int damage, float speed, float duration, float percent, Color color)    //Does [damage] damage every [speed] seconds for [duration] seconds (damage shows up with [color] text)
    {
        float startTime = Time.time;
        while (Time.time < startTime + duration)
        {
            if (damage > 0)
                StatusDamage(damage, color);

            if (percent > 0)
            {
                if ((int)(percent / 100 * (enemy.getHealth() * getHealthMult())) == 0)
                    StatusDamage(1, color);
                else
                    StatusDamage((int)(percent / 100 * (enemy.getHealth() * getHealthMult())), color);
            }

            yield return new WaitForSeconds(speed);
        }
    }
    private void baited(GameObject bait) { StartCoroutine(goToBaited(bait, bait.GetComponent<Hitscan>().getAOETimer())); }
    private IEnumerator goToBaited(GameObject bait, float time)    //Temporarily changes where the enemy thinks the goal is
    {
        if (!goal.CompareTag("Center") && enemy.getType() != Enemy.Types.WaterBoss && enemy.getType() != Enemy.Types.AirborneBoss) //Won't work on enemies in the channels
        {
            goal = new GameObject().transform;

            Vector3 newGoalPosition = bait.transform.position;

            //Left/Right edges
            if (newGoalPosition.x < 7f && newGoalPosition.x > 0 && Mathf.Abs(newGoalPosition.y) > 1f && Mathf.Abs(newGoalPosition.y) < 7f) newGoalPosition.x = 7.5f;
            else if (newGoalPosition.x > -7f && newGoalPosition.x < 0 && Mathf.Abs(newGoalPosition.y) > 1f && Mathf.Abs(newGoalPosition.y) < 7f) newGoalPosition.x = -7.5f;

            //Top/Bottom edges
            if (newGoalPosition.y < 7f && newGoalPosition.y > 1f && Mathf.Abs(newGoalPosition.x) > 1f && Mathf.Abs(newGoalPosition.x) < 7f) newGoalPosition.y = 7.5f;
            else if (newGoalPosition.y > -7f && newGoalPosition.y < -1f && Mathf.Abs(newGoalPosition.x) > 1f && Mathf.Abs(newGoalPosition.x) < 7f) newGoalPosition.y = -7.5f;

            goal.position = newGoalPosition;

            yield return new WaitForSeconds(time);

            Debug.Log(goal.gameObject);
            Destroy(goal.gameObject);
            nearestEntrance();
        }

    }
    #endregion

    #region Get Functions
    private bool getDistracted()    //Tells whether one of the debuffs distracts the enemy
    {
        for (int i = 0; i < debuffs.Length && debuffs[i] != null; i++)
        {
            if (debuffs[i].getDistracted())
                return true;
        }
        return false;
    }
    private float getSpeedMult()    //Gives total speed penalty/buff (multiplicative)
    {
        float speedMult = 1;
        for (int i = 0; i < debuffs.Length && debuffs[i] != null; i++)
        {
            speedMult *= debuffs[i].getSpeed();
        }
        return speedMult;
    }
    private float getAttackSpeed()  //Gives total attack speed penalty/buff (multiplicative)
    {
        float attackSpeedMult = 1;
        for (int i = 0; i < debuffs.Length && debuffs[i] != null; i++)
        {
            attackSpeedMult *= debuffs[i].getAttackSpeed();
        }
        return attackSpeedMult;
    }
    private float getDamageMult()   //Gives total damage penalty/buff (multiplicative)
    {
        float damageMult = 1;
        for (int i = 0; i < debuffs.Length && debuffs[i] != null; i++)
        {
            damageMult *= debuffs[i].getDamage();
        }
        return damageMult;
    }
    private float getArmor()        //Gives total damage reduction/increase (multiplicative)
    {
        float armor = 1;
        for (int i = 0; i < debuffs.Length && debuffs[i] != null; i++)
        {
            armor *= debuffs[i].getArmor();
        }
        return 1f / armor;
    }
    private float getHealthMult()   //Gives total percent health increase (multiplicative)
    {
        float mult = 1;
        for (int i = 0; i < debuffs.Length && debuffs[i] != null; i++)
        {
            mult *= debuffs[i].getHealth();
        }
        return mult;
    }
    public Enemy.Types getType() { return enemy.getType(); }
    public Enemy.SpecialTypes getSpecialType() 
    {
        if (forgotten)
            return Enemy.SpecialTypes.Forgotten;
        return enemy.getSpecialType();
    }
    public float getStrength()    //A "score" for each enemy based on health, damage, and speed
    {
        return enemy.getDamage() * enemy.getHealth() * enemy.getSpeed();
    }
    public string getName() { return enemy.name; }
    #endregion

    public void OnTriggerEnter2D(Collider2D collision)  //Trigger for hitting a barrier
    {
        if (collision.CompareTag("Barrier") && enemy.getType() != Enemy.Types.Airborne && enemy.getType() != Enemy.Types.AirborneBoss)
        {
            Barrier(collision.gameObject);
        }
    }
}
