using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Pathfinding;

public class AI : MonoBehaviour
{
    public event Action<int> OnEnemyHurt;
    public event Action<int> OnCrit;
    public event Action<int, Color> OnStatusDamage;
    public static event Action OnEnemyDeath;
    public static event Action OnDecoyDeath;
    public static event Action<Enemy> OnUnlockEnemyDeath;
    public static event Action onMeleeKill;
    public static event Action onMajorDamage;

    public event Action<int> SetupHealthBar;

    private Enemy enemy;
    private EnemyManager enemyManager;

    [SerializeField] private GameObject prefab;

    [SerializeField] private GameObject extra;
    [SerializeField] private Bullet bullet;

    private string[] impactSounds = {"Enemy Impact 1", "Enemy Impact 2", "Enemy Impact 3", "Enemy Impact 4" };

    private int newMaxHealth;

    private bool arrived;
    private int Health;
    private int damage;
    private bool stop;
    private bool dead;
    private bool forgotten = false;
    private float wall;

    [SerializeField] private bool noRotation;
    private Animator animator;
    private Buffs debuffToInflict;
    private Buffs[] debuffs;

    private float nextWaypointDistance = 0.5f;
    private Path path;
    private int currentWaypoint;
    private Vector3 currentWaypointPosition;
    private Seeker seeker;
    private bool airborne;
    private int scrapValue;
    private float speed;
    private bool elite;


    #region Setup and Update
    public void setEnemy(Enemy newEnemy)    //Setup function when told what type of enemy to be
    {
        enemyManager = FindObjectOfType<EnemyManager>();
        enemy = newEnemy;
        debuffs = new Buffs[10];

        Health = (int) (enemy.getHealth() * enemyManager.getRoundScale());
        newMaxHealth = Health;
        gameObject.GetComponent<Animator>().runtimeAnimatorController = enemy.getAnim();
        debuffToInflict = enemy.getDefuff();

        if (enemy.getExtra() != null)
            extra = enemy.getExtra();
        if (enemy.getBullet() != null)
            bullet = enemy.getBullet();

        gameObject.GetComponent<BoxCollider2D>().size = new Vector3(enemy.getXSize(), enemy.getYSize());    //Sets size of the hitbox. Might need another variable for the offset from the center since some of the sprites aren't centered

        scrapValue = enemy.getScrap();
        speed = enemy.getSpeed();

        if (enemy.getType() == Enemy.Types.WaterBoss || enemy.getType() == Enemy.Types.AirborneBoss && enemyManager.getRound() % 10 == 0)    //Boss health scaling during boss rounds, but not when a normal enemy
        {    
            Health = (int) (enemy.getHealth() * Mathf.Pow(1.25f, enemyManager.getRound() / 10f));
            newMaxHealth = Health;
        }

        if (enemy.getAvailableAttacks().Length > 0)
            StartCoroutine(randomAttacks());

        SetupHealthBar?.Invoke(Health);

        if (enemy.getType() == Enemy.Types.Airborne || enemy.getType() == Enemy.Types.AirborneBoss)
        {
            airborne = true;
            Destroy(seeker);
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = -1;
        }
        else
        {
            airborne = false;

            seeker = gameObject.GetComponent<Seeker>();
            seeker.StartPath(transform.position, nearestEntrance(), OnPathComplete);
        }
    }
    private void Start()    //Set up what setEnemy didn't
    {
        //setEnemy seems to run faster than Start, so there should be nothing set here that's set in setEnemy
        animator = gameObject.GetComponent<Animator>();
        debuffs = new Buffs[10];
        wall = 1;
    }
    void Update()   //Temp buttons, move, and check if the enemy has arrived at the center
    {
        healthCheck();    //Constantly update health to deal with health buffs being added/wearing off

        if (path == null && !airborne)
        {
            seeker = gameObject.GetComponent<Seeker>();
            seeker.StartPath(transform.position, nearestEntrance(), OnPathComplete);
        }

        if (!arrived)
        {
            move();

            if (path != null && currentWaypoint >= path.vectorPath.Count && isEndpoint())  //Stop when they reach the center
            {
                nowArriving();
            }
        }
    }
    private void setEliteEnemy(Enemy newEnemy)
    {
        Debug.Log("Elite Spawned: " + enemy.getName());
        //More Health, Damage, Speed, and Scrap value
        elite = true;

        enemyManager = FindObjectOfType<EnemyManager>();
        enemy = newEnemy;
        debuffs = new Buffs[10];

        //Health = (int)(enemy.getHealth() * enemyManager.getRoundScale() * 1.25f);   //More Health
        Health = (int)(enemy.getHealth() * enemyManager.getRoundScale());   //Not more Health
        newMaxHealth = Health;
        gameObject.GetComponent<Animator>().runtimeAnimatorController = enemy.getAnim();
        debuffToInflict = enemy.getDefuff();

        if (enemy.getExtra() != null)
            extra = enemy.getExtra();
        if (enemy.getBullet() != null)
            bullet = enemy.getBullet();

        gameObject.GetComponent<BoxCollider2D>().size = new Vector3(enemy.getXSize(), enemy.getYSize());    //Sets size of the hitbox. Might need another variable for the offset from the center since some of the sprites aren't centered

        scrapValue = (int) (enemy.getScrap() * 1.25f);  //Extra Scrap value
        speed = enemy.getSpeed() * 1.25f;   //Extra speed

        if (enemy.getType() == Enemy.Types.WaterBoss || enemy.getType() == Enemy.Types.AirborneBoss && enemyManager.getRound() % 10 == 0)    //Boss health scaling during boss rounds, but not when a normal enemy
        {
            Health = (int)(enemy.getHealth() * Mathf.Pow(1.25f, enemyManager.getRound() / 10f) * 1.25f);    //Extra health
            newMaxHealth = Health;
        }

        if (enemy.getAvailableAttacks().Length > 0)
            StartCoroutine(randomAttacks());

        SetupHealthBar?.Invoke(Health);

        if (enemy.getType() == Enemy.Types.Airborne || enemy.getType() == Enemy.Types.AirborneBoss)
        {
            airborne = true;
            Destroy(seeker);
        }
        else
        {
            airborne = false;

            seeker = gameObject.GetComponent<Seeker>();
            seeker.StartPath(transform.position, nearestEntrance(), OnPathComplete);
        }

        //Start particle effects
    }
    #endregion

    #region Movement functions
    private void OnPathComplete(Path p)
    {
        if (!p.error && !airborne)
        {
            path = p;
            currentWaypoint = 0;
            nextWaypointDistance = 0.5f;
            currentWaypointPosition = path.vectorPath[currentWaypoint];
        }
    }
    private void move() //Moves towards the goal
    {
        //goal != null && goal.position != null &&
        if (!stop)
        {
            Vector3 direction = (currentWaypointPosition - transform.position).normalized;
            gameObject.transform.position += direction * speed * Time.deltaTime * 0.5f * getSpeedMult() * wall;

            float distance = Vector2.Distance(transform.position, currentWaypointPosition);

            if ((enemy.getType() == Enemy.Types.Airborne || enemy.getType() == Enemy.Types.AirborneBoss) && distance < 2)
            {
                stop = true;
                nowArriving();
            }

            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
                if (currentWaypoint >= path.vectorPath.Count)
                {
                    nextWaypointDistance = 0;
                    currentWaypointPosition = path.vectorPath[path.vectorPath.Count - 1];
                }
                else
                    currentWaypointPosition = path.vectorPath[currentWaypoint];
            }
        }
    }
    private void nowArriving()  //Sets the position that the enemy will be attacking the center from
    {
        arrived = true;     //This section of code will only run once
        //stop = true;
        animator.SetBool("Attacking", true);    //Start animations

        StartCoroutine(attack(FindObjectOfType<Centerpiece>().gameObject));
    }
    public void Stop() { stop = true; }
    public void Go() { stop = false; }
    #endregion

    #region Attack loop
    IEnumerator randomAttacks() //Every 10 seconds, pick a random attack to do
    {
        List<Enemy.Attack> availableAttacks = new List<Enemy.Attack>();
        foreach (Enemy.Attack a in enemy.getAvailableAttacks()) //Can add multiple versions of the same attack to make it more likely
            availableAttacks.Add(a);

        while (gameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(10f);

            if ( (float) Health / newMaxHealth < enemy.phase2TriggerHealth())
            {
                animator.SetTrigger("Phase Change");
                foreach (Enemy.Attack a in enemy.getPhase2Attacks())
                    availableAttacks.Add(a);
            }

            if (gameObject.activeInHierarchy)
            {
                int chosenIndex = Random.Range(0, availableAttacks.Count);
                Enemy.Attack chosenAttack = availableAttacks[chosenIndex];

                if (!stop)
                    interpretAttack(chosenAttack);
            }
        }
    }
    void interpretAttack(Enemy.Attack attack)
    {
        Debug.Log(attack);

        if (attack == Enemy.Attack.Minions)
        {
            if (animExists("Minions"))
                animator.SetTrigger("Minions");
            else
                StartCoroutine(Minions());
        }

        if (attack == Enemy.Attack.Resurface)
        {
            if (animExists("Resurface"))
                animator.SetTrigger("Resurface");
            else
                StartCoroutine(Resurface());
        }

        if (attack == Enemy.Attack.Heal)
        {
            if (animExists("Heal"))
                animator.SetTrigger("Heal");
            else
                StartCoroutine(Heal());
        }

        if (attack == Enemy.Attack.Projectile)
        {
            if (animExists("Projectile"))
                animator.SetTrigger("Projectile");
            else
                StartCoroutine(Bullets());
        }

        if (attack == Enemy.Attack.Lightning)
        {
            if (animExists("Lightning"))
                animator.SetTrigger("Lightning");
            else
                StartCoroutine(LightningStrike());
        }

        if (attack == Enemy.Attack.Wall)
        {
            if (animExists("Wall"))
                animator.SetTrigger("Wall");
            else
                StartCoroutine(Wall());
        }

        if (attack == Enemy.Attack.Jump)
        {
            if (animExists("Jump"))
                animator.SetTrigger("Jump");
            else
                StartCoroutine(Jump());
        }
    }
    bool animExists(string name)    //Returns true if an animation state exists in the animator
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == name)
            {
                return true;
            }
        }

        return false;
    }

    #region Attack Coroutines
    IEnumerator Minions()   //Summon minions behind it
    {
        animator.SetTrigger("Minions");

        stop = true;    //Stop while spawning

        int total;
        if (enemyManager.getRound() < 80)
            total = enemyManager.getRound() / 10 + 2;  //start with 3 minions + 1 more every 10 rounds
        else
            total = 10;

        if (gameObject.transform.position.x < 0)   //Spawns the enemies behind itself, so different relative positions depending on what side of the screen they're on
        {
            for (int i = 0; i < total; i++)
            {
                yield return new WaitForSeconds(0.5f);  //Half-second gap between each minion
                var minion = Instantiate(prefab, new Vector3(gameObject.transform.position.x - 5, gameObject.transform.position.y + 5), Quaternion.identity);   //Maybe an animation for when they get summoned?
                minion.SendMessage("setEnemy", enemy.getMinion());
                enemyManager.newEnemies(); //Let the Enemy manager know more enemies are being spawned

                i++;
                if (i < total)  //Spawn behind and above, then behind and below. Sometimes the total is 0 when you get to the behind and below
                {
                    yield return new WaitForSeconds(0.5f);
                    minion = Instantiate(prefab, new Vector3(gameObject.transform.position.x - 5, gameObject.transform.position.y + -5), Quaternion.identity);
                    minion.SendMessage("setEnemy", enemy.getMinion());
                    enemyManager.newEnemies(); //Let the Enemy manager know more enemies are being spawned
                }
            }
        }
        else if (gameObject.transform.position.x > 0)   //Right side of the screen
        {
            for (int i = 0; i < total; i++)
            {
                yield return new WaitForSeconds(0.5f);
                var minion = Instantiate(prefab, new Vector3(gameObject.transform.position.x + 5, gameObject.transform.position.y + 5), Quaternion.identity);
                minion.SendMessage("setEnemy", enemy.getMinion());
                enemyManager.newEnemies(); //Let the Enemy manager know more enemies are being spawned

                i++;
                if (i < total)
                {
                    yield return new WaitForSeconds(0.5f);
                    minion = Instantiate(prefab, new Vector3(gameObject.transform.position.x + 5, gameObject.transform.position.y + -5), Quaternion.identity);
                    minion.SendMessage("setEnemy", enemy.getMinion());
                    enemyManager.newEnemies(); //Let the Enemy manager know more enemies are being spawned
                }
            }
        }

        stop = false;   //Resume
    }
    IEnumerator Resurface() //Dive underwater and reappear at a random spot on the other side of the screen. Could come up really far away, could come up really close
    {
        animator.SetTrigger("Resurface");
        stop = true;    //Stop to dive under

        if (gameObject.transform.position.x < 0)    //Start on left side
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;   //Can't be shot while underwater
            yield return new WaitForSeconds(1f);
            gameObject.transform.position = new Vector3(Random.Range(8f, 20f), Random.Range(-10f, 10f));    //Pick random spot on right side of screen
            nearestEntrance();  //Switch the goal to the entrance that's nearest now
            yield return new WaitForSeconds(1f);

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
        if (gameObject.GetComponent<Seeker>() != null)
            updatePath();

        stop = false;   //Resume going towards raft
    }
    IEnumerator Heal()  //Heal 10% of their current health
    {
        animator.SetTrigger("Heal");
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
    IEnumerator Bullets()
    {
        stop = true;

        yield return new WaitForSeconds(1);
        animator.SetTrigger("Projectile");

        Vector3 offset = FindObjectOfType<Centerpiece>().transform.position - transform.position;
        Quaternion output = Quaternion.LookRotation(Vector3.forward, offset).normalized;
        var boolet = Instantiate(extra, gameObject.transform.position, Quaternion.Euler(output.eulerAngles - (output.eulerAngles / 2)));
        boolet.SendMessage("setBullet", bullet);

        stop = false;
    }
    IEnumerator LightningStrike()
    {
        stop = true;

        yield return new WaitForSeconds(1);

        StartCoroutine(strike());

        stop = false;
    }
    IEnumerator strike()
    {
        TowerAI target = FindObjectOfType<BuildManager>().getTowerOfType("Lightning Rod");

        animator.SetTrigger("Lightning");

        if (target == null)
        {
            target = FindObjectOfType<BuildManager>().getRandomTower();
        }

        if (target == null)
            yield break;

        GameObject lightningBolt = GameObject.Find("Lightning");

        lightningBolt.transform.position = target.transform.position;
        target.SendMessage("lightningStrike", 5f);

        for (int i = 0; i < 5; i++)
            lightningBolt.transform.GetChild(i).gameObject.SetActive(false);


        lightningBolt.transform.GetChild(0).gameObject.SetActive(false);
        for (int i = 1; i < 5; i++)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            lightningBolt.transform.GetChild(i - 1).gameObject.SetActive(false);
            lightningBolt.transform.GetChild(i).gameObject.SetActive(true);
        }

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < 5; i++)
            lightningBolt.transform.GetChild(i).gameObject.SetActive(false);
    }
    IEnumerator Wall()
    {
        wall = 0f;

        animator.SetTrigger("Wall");
        yield return new WaitForSeconds(5f);

        wall = 1f;
    }
    IEnumerator Jump()
    {
        wall = 5f;

        animator.SetTrigger("Jump");
        yield return new WaitForSeconds(5f);

        wall = 1f;
    }
    #endregion

    #region Direct Attacks
    public void MinionAttack() //Summons 2 minions
    {
        if (gameObject.transform.position.x < 0)
        {
            var minion = Instantiate(prefab, new Vector3(gameObject.transform.position.x - 5, gameObject.transform.position.y + 5), Quaternion.identity);   //Maybe an animation for when they get summoned?
            minion.SendMessage("setEnemy", enemy.getMinion());
            enemyManager.newEnemies(); //Let the Enemy manager know more enemies are being spawned


            minion = Instantiate(prefab, new Vector3(gameObject.transform.position.x - 5, gameObject.transform.position.y + -5), Quaternion.identity);
            minion.SendMessage("setEnemy", enemy.getMinion());
            enemyManager.newEnemies(); //Let the Enemy manager know more enemies are being spawned
        }
        else
        {
            var minion = Instantiate(prefab, new Vector3(gameObject.transform.position.x + 5, gameObject.transform.position.y + 5), Quaternion.identity);   //Maybe an animation for when they get summoned?
            minion.SendMessage("setEnemy", enemy.getMinion());
            enemyManager.newEnemies(); //Let the Enemy manager know more enemies are being spawned


            minion = Instantiate(prefab, new Vector3(gameObject.transform.position.x + 5, gameObject.transform.position.y + -5), Quaternion.identity);
            minion.SendMessage("setEnemy", enemy.getMinion());
            enemyManager.newEnemies(); //Let the Enemy manager know more enemies are being spawned
        }
    }
    public void ResurfaceAttackPt1()   //Disable Hitbox
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = false;   //Can't be shot while underwater
    }
    public void ResurfaceAttackPt2()   //Enable hitbox and move
    {
        if (gameObject.transform.position.x < 0)
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = true;
            gameObject.transform.position = new Vector3(Random.Range(8f, 20f), Random.Range(-10f, 10f));    //Pick random spot on right side of screen
            nearestEntrance();
        }
        else
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = true;
            gameObject.transform.position = new Vector3(Random.Range(-20f, -8f), Random.Range(-10f, 10f));  //Random spot on left side of the screen
            nearestEntrance();
        }
        updatePath();
    }
    public void HealAttack()
    {
        int toHeal = (int)(Health * 0.1f);
        OnEnemyHurt?.Invoke(-toHeal);
        Health += toHeal; //Is 10% of their current health more balanced or their max health? The max health is always going to be more than what the current health will heal, but is that a good or bad thing?
        if (Health > enemy.getHealth()) //To prevent overhealing (or maybe we'd want them to be able to do that?
            Health = enemy.getHealth();
    }
    public void ProjectileAttack()
    {
        Vector3 offset = FindObjectOfType<Centerpiece>().transform.position - transform.position;
        Quaternion output = Quaternion.LookRotation(Vector3.forward, offset).normalized;
        var boolet = Instantiate(extra, gameObject.transform.position, Quaternion.Euler(output.eulerAngles - (output.eulerAngles / 2)));
        boolet.SendMessage("setBullet", bullet);
    }
    public void LightningStrikeAttack()
    {
        StartCoroutine(strike());
    }
    public void WallStart() { wall = 0f; }
    public void WallEnd() { wall = 1; }
    public void JumpStart() { wall = 5; }
    public void JumpEnd() { wall = 1; }
    #endregion

    #endregion

    #region Goal Setters
    private void updatePath()   //Updates the path when the goal possibly changed
    {
        seeker.StartPath(transform.position, nearestEntrance(), OnPathComplete);
    }
    private Vector3 nearestEntrance()  //Finds the closest object tagged entrance and sets the goal to be that
    {
        if (enemy.getType() == Enemy.Types.Airborne || enemy.getType() == Enemy.Types.AirborneBoss)
        {
            currentWaypointPosition = FindObjectOfType<Centerpiece>().transform.position;
            return currentWaypointPosition;
        }

        Vector3 goal = Vector3.zero;

        GameObject[] entrances = GameObject.FindGameObjectsWithTag("Entrance");
        if (entrances.Length <= 0)
            return goal;

        float distance = Mathf.Infinity;
        Vector3 pos = gameObject.transform.position;
        foreach (GameObject go in entrances)    //Iterate through all entrances, picking the closest one. Should only be 4 items, so efficiency shouldn't be a huge problem
        {
            Vector3 diff = go.transform.position - pos;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                goal = go.transform.position;
                distance = curDistance;
            }
        }

        return goal;
    }
    private bool isEndpoint()   //Returns true if the enemy is at the end of a channel, returns false if it's a bait locatione
    {
        GameObject[] entrances = GameObject.FindGameObjectsWithTag("Entrance");

        GameObject goal = entrances[0];

        float distance = Mathf.Infinity;
        Vector3 pos = gameObject.transform.position;
        foreach (GameObject go in entrances)    //Iterate through all entrances, picking the closest one. Should only be 4 items, so efficiency shouldn't be a huge problem
        {
            Vector3 diff = go.transform.position - pos;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                goal = go;
                distance = curDistance;
            }
        }
        if (goal.layer == 1)
            return true;
        else
            return false;
    }
    #endregion

    #region TakeDamage functions
    private void TakeDamage(int moreDamage) //Receiver for damage numbers, go to debuff function if debuffed
    {
        if (enemy.getSpecialType() == Enemy.SpecialTypes.Immune) return;

        AudioManager.Instance.PlaySFX(impactSounds[Random.Range(0, impactSounds.Length)]);

        Health -= (int) (moreDamage * getArmor() * wall);
        damage += (int) (moreDamage * getArmor() * wall);
        animator.SetTrigger("TookDamage");

        if (moreDamage * getArmor() * wall >= 1000)
            onMajorDamage?.Invoke();

        //if (enemy.getType() == Enemy.Types.WaterBoss)
        //    Debug.Log("Took " + (int)(moreDamage * getArmor()) + " damage, now at " + Health + " / " + newMaxHealth);

        //Debug.Log("Normal: -" + (moreDamage * getArmor()) + " => " + Health + " / " + enemy.getHealth());

        if (!forgotten) //Not even damage popups appear when an enemy is forgotten
            OnEnemyHurt?.Invoke((int)(moreDamage * getArmor() * wall));

        if (Health <= 0 && !dead)   //Need the dead check or it'll count multiple deaths per enemy
            death();
    }
    private void CritDamage(int moreDamage)
    {
        if (enemy.getSpecialType() == Enemy.SpecialTypes.Immune) return;

        AudioManager.Instance.PlaySFX(impactSounds[Random.Range(0, impactSounds.Length)]);

        Health -= (int)(moreDamage * getArmor() * wall);
        damage += (int)(moreDamage * getArmor() * wall);
        animator.SetTrigger("TookDamage");

        if (moreDamage * getArmor() * wall >= 1000)
            onMajorDamage?.Invoke();

        if (!forgotten) //Not even damage popups appear when an enemy is forgotten
            OnCrit?.Invoke((int)(moreDamage * getArmor() * wall));

        if (Health <= 0 && !dead)   //Need the dead check or it'll count multiple deaths per enemy
            death();
    }
    private void TakeDamageMelee(int moreDamage) //Receiver for damage numbers, go to debuff function if debuffed
    {
        if (enemy.getSpecialType() == Enemy.SpecialTypes.Immune) return;

        AudioManager.Instance.PlaySFX(impactSounds[Random.Range(0, impactSounds.Length)]);

        Health -= (int)(moreDamage * getArmor() * wall);
        damage += (int)(moreDamage * getArmor() * wall);
        animator.SetTrigger("TookDamage");

        if (moreDamage * getArmor() * wall >= 1000)
            onMajorDamage?.Invoke();

        //if (enemy.getType() == Enemy.Types.WaterBoss)
        //    Debug.Log("Took " + (int)(moreDamage * getArmor()) + " damage, now at " + Health + " / " + newMaxHealth);

        //Debug.Log("Normal: -" + (moreDamage * getArmor()) + " => " + Health + " / " + enemy.getHealth());

        if (!forgotten) //Not even damage popups appear when an enemy is forgotten
            OnEnemyHurt?.Invoke((int)(moreDamage * getArmor() * wall));

        if (Health <= 0 && !dead)   //Need the dead check or it'll count multiple deaths per enemy
        {
            if (enemy.getType() == Enemy.Types.WaterBoss || enemy.getType() == Enemy.Types.AirborneBoss)
                onMeleeKill?.Invoke();
            death();
        }
    }
    private void CritDamageMelee(int moreDamage)
    {
        if (enemy.getSpecialType() == Enemy.SpecialTypes.Immune) return;

        AudioManager.Instance.PlaySFX(impactSounds[Random.Range(0, impactSounds.Length)]);

        Health -= (int)(moreDamage * getArmor() * wall);
        damage += (int)(moreDamage * getArmor() * wall);
        animator.SetTrigger("TookDamage");

        if (moreDamage * getArmor() * wall >= 1000)
            onMajorDamage?.Invoke();

        if (!forgotten) //Not even damage popups appear when an enemy is forgotten
            OnCrit?.Invoke((int)(moreDamage * getArmor() * wall));

        if (Health <= 0 && !dead)   //Need the dead check or it'll count multiple deaths per enemy
        {
            if (enemy.getType() == Enemy.Types.WaterBoss || enemy.getType() == Enemy.Types.AirborneBoss)
                onMeleeKill?.Invoke();
            death();
        }
    }
    private void StatusDamage(int moreDamage, Color color)
    {
        if (enemy.getSpecialType() == Enemy.SpecialTypes.Immune) return;

        Health -= moreDamage;
        damage += moreDamage;
        animator.SetTrigger("TookDamage");

        //Debug.Log("Status: -" + moreDamage + " => " + Health + " / " + enemy.getHealth());

        if (!forgotten) //Not even damage popups appear when an enemy is forgotten
            OnStatusDamage?.Invoke(moreDamage, color);

        if (Health <= 0 && !dead)   //Need the dead check or it'll count multiple deaths per enemy
            death();
    }
    private void death()    //Updates enemyManager count, adds scrap and XP, and deletes GameObject
    {
        if (dead) return;

        enemyManager.SendMessage("addScrap", scrapValue);
        enemyManager.SendMessage("addXP", enemy.getXP());

        enemyManager.SendMessage("EnemyDown");
        stop = true;
        dead = true;

        animator.SetBool("Dead", true);

        //if (!goal.CompareTag("Entrance") && !goal.CompareTag("Center")) Destroy(goal.gameObject);

        OnEnemyDeath?.Invoke();

        if (enemy.getSpecialType() == Enemy.SpecialTypes.Decoy)
            OnDecoyDeath?.Invoke();

        OnUnlockEnemyDeath?.Invoke(enemy);

        Destroy(gameObject);
    }
    #endregion

    #region Attack/Heal
    private IEnumerator attack(GameObject recipient)
    {
        stop = true;

        while (recipient != null)
        {
            if (elite)
                recipient.SendMessage("TakeDamage", ((int)(enemy.getDamage() * getDamageMult() * 1.25f), this));
            else
                recipient.SendMessage("TakeDamage", ((int)(enemy.getDamage() * getDamageMult()), this));

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

    public Vector3 getGoal() { return FindObjectOfType<Centerpiece>().transform.position; } //Only used in the directional animation script

    private void Barrier(GameObject barrier) //Goes into attacking mode until the barrier is destroyed
    {
        if (barrier.GetComponent<Barriers>().getEffect() != BarrierScriptable.Effect.Effect) {
            StartCoroutine(attack(barrier));    //Start attacking
        } else
        {
            StartCoroutine(InflictDebuff(barrier.GetComponent<Barriers>().getDebuff()));
        }

    }

    #region Debuff Managing
    private void Forget()   //50-50 chance to either instakill or become invisible to players
    {
        if (forgotten) return;

        if (enemy.getType() == Enemy.Types.WaterBoss || enemy.getType() == Enemy.Types.AirborneBoss) return;

        if (Random.Range(0, 2) == 0)
        {
            death();
            return;
        }

        GetComponent<SpriteRenderer>().enabled = false;
        Destroy(transform.GetChild(0).gameObject);
        forgotten = true;
    }
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

        if (newDebuff.getStunned() && enemy.getType() != Enemy.Types.WaterBoss && enemy.getType() != Enemy.Types.AirborneBoss)
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
        int expectedHealth = (int)(newMaxHealth * getHealthMult()) - damage;

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

        if (Mathf.Abs(transform.position.x) > (Camera.main.transform.position.x + Camera.main.orthographicSize) * 1.5f || Mathf.Abs(transform.position.y) > Camera.main.transform.position.y + Camera.main.orthographicSize) //Extra speed boost until fully on screen
            speedMult = 2;

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
    public Enemy.SpecialTypes getSpecialType() { return enemy.getSpecialType(); }

    #region Tower Priority Get Functions
    public float getStrength()    //A "score" for each enemy based on health, damage, and speed
    {
        return enemy.getDamage() * enemy.getHealth() * enemy.getSpeed();
    }
    public float towerGetSpeed() { return enemy.getSpeed(); }   //Doesn't factor in buffs
    #endregion
    public string getName() { return enemy.name; }
    public bool getForgotten() { return forgotten; }
    #endregion

    public void OnTriggerEnter2D(Collider2D collision)  //Trigger for hitting a barrier
    {
        if (collision.CompareTag("Barrier") && enemy.getType() != Enemy.Types.Airborne && enemy.getType() != Enemy.Types.AirborneBoss)
        {
            Barrier(collision.gameObject);
        }
    }
}
