using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    //You could maybe combine the AI and BosAI scripts together and rely on the Types Enum in Enemy to tell what functions it can use
    private Enemy enemy;
    private GameObject enemyManager;

    private Transform movement;

    private Transform goal;
    private GameObject goalGO;

    private bool arrived;
    private int Health;
    private bool stop;

    private bool noRotation;    //Will probably be removed later, but for now default is false
    [SerializeField] private GameObject prefab; //Used to spawn minions
    private Enemy Minion;

    private Animator animator;

    private void Start()
    {
        enemyManager = GameObject.FindGameObjectWithTag("Managers");
        movement = gameObject.transform;
        animator = gameObject.GetComponent<Animator>();

        nearestEntrance();

        if (goal != null && !noRotation)    //Point at goal
        {
            float z = (Mathf.Atan2(movement.position.y - goal.position.y, movement.position.x - goal.position.x) * (180 / Mathf.PI)); //Atan2 gives inverse tan in radians from current cordinates, transform takes degrees
            movement.localRotation = Quaternion.Euler(0, 0, z);  //Faces towards goal (If sprites faces left)
        }
    }
    void Update()   //Temp keys, move, and check if they've reached the center
    {
        if (Input.GetKeyDown("o"))  //Temp keys, should probably get assigned to a UI thing sometime soon
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

            if (Mathf.Abs(movement.position.x) + Mathf.Abs(movement.position.y) <= 1)  //Stop when they reach the center
            {
                arrived = true; //Only runs once
                animator.SetBool("Attacking", true);

                //Only need to check left vs right lane because it should only be spawning in from the left or right
                if (movement.position.x < 0) { movement.SetPositionAndRotation(new Vector3(-2, 0), movement.rotation); }    //Left Lane
                else { movement.SetPositionAndRotation(new Vector3(2, 0), movement.rotation); } //Right Lane

                StartCoroutine(Attacking());
            }
        }
    }
 
    private void move() //Moves towards the goal
    {
        if (goal != null && !stop)
        {
            movement.position = Vector3.MoveTowards(movement.position, goal.position, enemy.getSpeed() * Time.deltaTime * 0.5f);
            if (movement.position == goal.position)
                enteringChannel();
        }
    }
    public void setEnemy(Enemy newEnemy)    //Setup for the enemy
    {
        enemy = newEnemy;

        Health = enemy.getHealth() + ((GameObject.FindGameObjectWithTag("Managers").GetComponent<EnemyManager>().getRound() / 10 - 1) * (enemy.getHealth() / 2));   //Health scales with round number
        Debug.Log("Health: " + Health);
        gameObject.GetComponent<Animator>().runtimeAnimatorController = enemy.getAnim();

        gameObject.GetComponent<BoxCollider2D>().size = new Vector3(enemy.getXSize(), enemy.getYSize());    //Set hitbox size

        StartCoroutine(randomAttacks());
    }
    public void setMinion(Enemy newMinion) { Minion = newMinion; }  //Sets the minion when the boss spawns in. Should be random
    public Enemy getEnemy() { return enemy; }   //I don't think this is used anywhere anymore
    private void nearestEntrance()  //Sets target object to the nearest entrance
    {
        GameObject[] entrances;
        entrances = GameObject.FindGameObjectsWithTag("Entrance");
        goal = null;
        float distance = Mathf.Infinity;
        Vector3 pos = movement.position;
        foreach (GameObject go in entrances)    //Find closest entrance by iterating through all of them
        {
            Vector3 diff = go.transform.position - pos;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                goal = go.transform;
                distance = curDistance;
            }
        }

        gameObject.GetComponent<SpriteRenderer>().flipX = true; //Default to flipped
        if ((goal.position.x < 0 || goal.position.y > 0) && gameObject.GetComponent<DirectionalAnimations>() == null)   //Flip if on the left side
        {
            if (noRotation)
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = false;
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().flipY = !gameObject.GetComponent<SpriteRenderer>().flipY;
            }
        }
    }

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
        } else  //Start on Right Side
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

    IEnumerator Attacking() //Repeatedly attacking the centerpiece
    {
        while (gameObject.active)
        {
            //Attacking the center
            yield return new WaitForSeconds(1f);    //Attack speed variable in Enemy?

            if (goalGO != null && !stop)
            {
                goalGO.SendMessage("CenterDamage", enemy.getDamage());
            }
        }
    }
    void enteringChannel()  //set goal to be the centerpiece
    {
        goalGO = GameObject.FindGameObjectWithTag("Center");
        goal = goalGO.transform;

        animator.SetTrigger("InChannel");

        if (!noRotation)    //Rotate towards center
        {
            float z = (Mathf.Atan2(movement.position.y, movement.position.x) * (180 / Mathf.PI)); //Atan2 gives inverse tan in radians from current cordinates, transform takes degrees
            movement.rotation = Quaternion.Euler(0, 0, z);  //Faces towards center (If sprites faces left)
        }
    }
    private void TakeDamage(int damage) //Take a certain amount of damage, and die if health is less than 0
    {
        Health -= damage;
        //Debug.Log(Health);

        animator.SetTrigger("TookDamage");
        if (Health <= 0 && !stop)   //Might be a bug where it doesn't die if it's doing an attack, might need to put something in the update function
        {
            death();
        }
    }
    private void death()    //Update enemy manager, play animation, add XP and Scrap, delete GameObject
    {
        enemyManager.SendMessage("EnemyDown");
        stop = true;

        animator.SetBool("Dead", true);
        enemyManager.SendMessage("addXP", enemy.getXP());
        enemyManager.SendMessage("addScrap", enemy.getScrap());
        new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
    public Transform getGoal() { return goal; } //Might not be needed
}
