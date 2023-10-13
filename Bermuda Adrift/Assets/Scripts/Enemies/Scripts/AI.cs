using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AI : MonoBehaviour
{
    private Enemy enemy;
    private GameObject enemyManager;

    private Transform movement;

    private Transform goal;
    private GameObject goalGO;

    private bool arrived;
    private int Health;
    private bool stop;

    private bool noRotation;
    private Animator animator;

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
        if (goal != null && !stop)
        {
            movement.position = Vector3.MoveTowards(movement.position, goal.position, enemy.getSpeed() * Time.deltaTime * 0.5f);
            if (movement.position == goal.position)
                enteringChannel();
        }
    }
    public void setEnemy(Enemy newEnemy)    //Setup function when told what type of enemy to be
    {
        enemy = newEnemy;

        Health = Health = enemy.getHealth();    //Maybe add health scaling later based on the number of times the set of enemies has been picked?
        gameObject.GetComponent<Animator>().runtimeAnimatorController = enemy.getAnim();

        gameObject.GetComponent<BoxCollider2D>().size = new Vector3(enemy.getXSize(), enemy.getYSize());    //Sets size of the hitbox. Might need another variable for the offset from the center since some of the sprites aren't centered
    }
    public Enemy getEnemy() { return enemy; }   //Returns the scriptable object. I don't know if any scripts use this anymore after the refactoring
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
            yield return new WaitForSeconds(1f);    //Maybe add an attack speed variable to Enemy

            if (goalGO != null && !stop) { goalGO.SendMessage("CenterDamage", enemy.getDamage()); } //Maybe add animation here for what happens when the enemies succeed?
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
        Health -= damage;
        animator.SetTrigger("TookDamage");
        if (Health <= 0 && !stop)
        {
            death();
        }
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

    private IEnumerator Barrier(GameObject barrier)
    {
        if (barrier.GetComponent<Barriers>().getEffect() == BarrierScriptable.Effect.Blockade) {
            stop = true;
            animator.SetBool("Attacking", true);    //Start attacking animation

            while (barrier != null)
            {
                yield return new WaitForSeconds(1f);    //Standardized damage speed?

                if (barrier != null)
                    barrier.SendMessage("takeDamage", enemy.getDamage());
            }

            animator.SetBool("Attacking", false);
            stop = false;
        }

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Barrier"))
        {
            StartCoroutine(Barrier(collision.gameObject));
        }
    }
}
