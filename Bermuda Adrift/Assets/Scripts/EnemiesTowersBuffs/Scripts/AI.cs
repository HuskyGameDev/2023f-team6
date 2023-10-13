using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AI : MonoBehaviour
{
    private Enemy enemy;
    private GameObject enemyManager;

    private Enemy Minion;
    [SerializeField] private GameObject prefab;

    private Transform movement;

    private Transform goal;
    private GameObject goalGO;

    private bool arrived;
    private int Health;
    private bool stop;

    private bool noRotation;
    private Animator animator;
    private Buffs debuff;
    private Buffs debuffed;
    [SerializeField] private Buffs defaultDebuff;

    private void Start()
    {
        //setEnemy seems to run faster than Start
        animator = gameObject.GetComponent<Animator>();
        enemyManager = GameObject.FindGameObjectWithTag("Managers");
        movement = gameObject.transform;

        nearestEntrance();  //Sets the entrance to be going towards

        if (goal != null && !noRotation)    //Face towards goal. Default is noRotation being false now. This function can be modified once we standardize the artstyle and the default direction
        {
            //Only rotates once, so if we do knockback or something we'll have to put this in the Update function or the TakeDamage function
            float z = (Mathf.Atan2(movement.position.y - goal.position.y, movement.position.x - goal.position.x) * (180 / Mathf.PI)); //Atan2 gives inverse tan in radians from current cordinates, transform takes degrees
            movement.localRotation = Quaternion.Euler(0, 0, z);  //Faces towards goal (If sprites faces left)
        }
    }
    void Update()   //Temp buttons, move, and check if the enemy has arrived at the center
    {
        if (Input.GetKeyDown("o"))  //Temp buttons, o does 1 damage, p kills everything (a sort of "skip round" button)
        {
            TakeDamage(1);
        }
        else if (Input.GetKeyDown("p"))
        {
            TakeDamage(Health);
        }

        if (!arrived)
        {
            move();

            //Add interaction with barriers

            if (Mathf.Abs(movement.position.x) + Mathf.Abs(movement.position.y) <= 1)  //Stop when they reach the center and set position to a spot depending on the lane they're in. This needs to be redone at some point, maybe use hitboxes?
            {
                arrived = true;     //This section of code will only run once
                animator.SetBool("Attacking", true);    //Start animations

                if (movement.position.x < 0) { movement.SetPositionAndRotation(new Vector3(-2, 0), movement.rotation); }        //Left lane
                else if (movement.position.x > 0) { movement.SetPositionAndRotation(new Vector3(2, 0), movement.rotation); }    //Right lane
                else if (movement.position.y > 0) { movement.SetPositionAndRotation(new Vector3(0, 2), movement.rotation); }    //Top lane
                else { movement.SetPositionAndRotation(new Vector3(0, -2), movement.rotation); }                                //Bottom Lane

                StartCoroutine(Attacking());
            }
        }
    }

    private void move() //Moves towards the goal
    {
        //Add functionality for "Distracted" debuff
        if (goal != null && !stop)
        {
            movement.position = Vector3.MoveTowards(movement.position, goal.position, enemy.getSpeed() * Time.deltaTime * 0.5f * debuffed.getSpeed());
            if (movement.position == goal.position)
                enteringChannel();
        }
    }
    public void setEnemy(Enemy newEnemy)    //Setup function when told what type of enemy to be
    {
        enemy = newEnemy;

        Health = Health = enemy.getHealth();    //Maybe add health scaling later based on the number of times the set of enemies has been picked?
        gameObject.GetComponent<Animator>().runtimeAnimatorController = enemy.getAnim();
        debuff = enemy.getDefuff();

        gameObject.GetComponent<BoxCollider2D>().size = new Vector3(enemy.getXSize(), enemy.getYSize());    //Sets size of the hitbox. Might need another variable for the offset from the center since some of the sprites aren't centered

        if (enemy.getType() == Enemy.Types.Boss)
        {
            StartCoroutine(randomAttacks());
        }
    }
    public void setMinion(Enemy newMinion) { Minion = newMinion; }  //Sets the minion when the boss spawns in. Should be random
    public Enemy getEnemy() { return enemy; }   //Returns the scriptable object. I don't know if any scripts use this anymore after the refactoring


    IEnumerator randomAttacks() //Every 10 seconds, pick a random attack to do
    {
        while (gameObject.active)
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

        Health += (int)(Health * 0.1f); //Is 10% of their current health more balanced or their max health? The max health is always going to be more than what the current health will heal, but is that a good or bad thing?
        if (Health > enemy.getHealth()) //To prevent overhealing (or maybe we'd want them to be able to do that?
            Health = enemy.getHealth();

        yield return new WaitForSeconds(1f);

        stop = false;   //Resume
    }


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
    IEnumerator Attacking() //Coroutine for attacking. Probably will include barrier attacking here
    {
        while (gameObject.active)   //Infinite loop until the enemy dies
        {
            //Attacking the center
            yield return new WaitForSeconds(enemy.getAttackSpeed() * debuffed.getAttackSpeed());

            if (goalGO != null && !stop) { goalGO.SendMessage("CenterDamage", enemy.getDamage() * debuffed.getDamage()); } //Maybe add animation here for what happens when the enemies succeed?
        }
    }
    void enteringChannel()  //Set the goal to be the centerpiece and rotate to point at it
    {
        goalGO = GameObject.FindGameObjectWithTag("Center");
        goal = goalGO.transform;    //Set new goal to the centerpiece. Could maybe just set the goal to be 0,0 for efficiency and so we don't need the centerpiece tag

        animator.SetTrigger("InChannel");

        if (!noRotation)    //Points towards new goal
        {
        float z = (Mathf.Atan2(movement.position.y, movement.position.x) * (180 / Mathf.PI)); //Atan2 gives inverse tan in radians from current cordinates, transform takes degrees
        movement.rotation = Quaternion.Euler(0, 0, z);  //Faces towards center (If sprites faces left)
        }
    }
    private void TakeDamage(int damage) //Take damage as told, then die if health is below 0
    {
        Health -= (int) (damage * debuffed.getArmor());
        animator.SetTrigger("TookDamage");
        if (Health <= 0 && !stop)
        {
            death();
        }
        Debug.Log("Took " + damage + " damage");
    }
    public void heal(int health)   //maybe an enemy that heals other enemies?
    {
        Health += health;
    }
    private void death()    //Updates enemyManager count, adds scrap and XP, and deletes GameObject
    {
        enemyManager.SendMessage("EnemyDown");
        stop = true;

        animator.SetBool("Dead", true);

        enemyManager.SendMessage("addScrap", enemy.getScrap());
        enemyManager.SendMessage("addXP", enemy.getXP());

        new WaitForSeconds(0.5f);  //Maybe a standard death animation length or a variable in Enemy? Is it possible to wait until an animation is done?
        Destroy(gameObject);
    }
    public Transform getGoal() { return goal; } //Only used in the directional animation script, so might not be needed anymore

    private IEnumerator Barrier(GameObject barrier) //Goes into attacking mode until the barrier is destroyed
    {
        if (barrier.GetComponent<Barriers>().getEffect() == BarrierScriptable.Effect.Blockade) {
            stop = true;
            animator.SetBool("Attacking", true);    //Start attacking animation

            while (barrier != null)
            {
                barrier.SendMessage("takeDamage", enemy.getDamage() * debuffed.getDamage());
                barrier.SendMessage("InflictDebuff", debuff);   //Only inflicts debuffs on barriers

                yield return new WaitForSeconds(enemy.getAttackSpeed() * debuffed.getAttackSpeed());
            }

            animator.SetBool("Attacking", false);
            stop = false;
        }

    }
    private IEnumerator InflictDebuff(Buffs newDebuff)  //Sets the "debuffed" variable and applies DOT
    {
        float startTime = Time.time;
        debuffed = newDebuff;   //Only DOT can stack
        int buffedHealth = (int) (Health * debuffed.getHealth());
        Health += buffedHealth;
        //Visual/particle effect

        while (Time.time < startTime + newDebuff.getDuration()) //Even if no DOT, still waits until the end of the duration
        {
            TakeDamage(newDebuff.getDOT());

            yield return new WaitForSeconds(newDebuff.getDOTSpeed());
        }

        Health -= buffedHealth;
        debuffed = defaultDebuff;
    }
    public void OnTriggerEnter2D(Collider2D collision)  //Trigger for hitting a barrier
    {
        if (collision.CompareTag("Barrier") && enemy.getType() != Enemy.Types.Airborne)
        {
            StartCoroutine(Barrier(collision.gameObject));
        }
    }
}
