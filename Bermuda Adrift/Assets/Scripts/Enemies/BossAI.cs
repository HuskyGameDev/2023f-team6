using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    public Enemy enemy;

    private Transform movement;
    private bool arrived;
    private Transform goal;
    private GameObject goalGO;
    private int Health;
    private bool stop;
    private GameObject enemyManager;

    [SerializeField] private bool noRotation;
    [SerializeField] private GameObject MinionPrefab;
    private Animator animator;

    private void Start()
    {
        enemyManager = GameObject.FindGameObjectWithTag("Managers");
        movement = gameObject.transform;
        animator = gameObject.GetComponent<Animator>();

        Health = enemy.getHealth() + ((enemyManager.GetComponent<EnemyManager>().getRound() / 10 - 1) * (enemy.getHealth() / 2));   //Health scales with round number
        //if (healthSlider != null)
        //    healthSlider.maxValue = enemy.getHealth();

        nearestEntrance();

        if (goal != null && !noRotation)
        {
            float z = (Mathf.Atan2(movement.position.y - goal.position.y, movement.position.x - goal.position.x) * (180 / Mathf.PI)); //Atan2 gives inverse tan in radians from current cordinates, transform takes degrees
            movement.localRotation = Quaternion.Euler(0, 0, z);  //Faces towards goal (If sprites faces left)
        }

        StartCoroutine(randomAttacks());
    }

    public Enemy getEnemy() { return enemy; }

    private void nearestEntrance()
    {
        GameObject[] entrances;
        entrances = GameObject.FindGameObjectsWithTag("Entrance");
        goal = null;
        float distance = Mathf.Infinity;
        Vector3 pos = movement.position;
        foreach (GameObject go in entrances)
        {
            Vector3 diff = go.transform.position - pos;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                goal = go.transform;
                distance = curDistance;
            }
        }

        gameObject.GetComponent<SpriteRenderer>().flipX = true;
        if ((goal.position.x < 0 || goal.position.y > 0) && gameObject.GetComponent<DirectionalAnimations>() == null)
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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("a"))
        {
            TakeDamage(1);
        }
        else if (Input.GetKeyDown("p"))
        {
            TakeDamage(Health);
        }

        if (!arrived)
        {
            if (goal != null && !stop)
            {
                movement.position = Vector3.MoveTowards(movement.position, goal.position, enemy.getSpeed() * Time.deltaTime * 0.5f);
                if (movement.position == goal.position)
                    enteringChannel();
            }

            //Add interaction with barriers

            if (Mathf.Abs(movement.position.x) + Mathf.Abs(movement.position.y) <= 1)  //Stop when they reach the center
            {
                arrived = true;
                animator.SetBool("Attacking", true);

                if (movement.position.x < 0)
                {
                    movement.SetPositionAndRotation(new Vector3(-1, 0), movement.rotation);
                }
                else if (movement.position.x > 0)
                {
                    movement.SetPositionAndRotation(new Vector3(1, 0), movement.rotation);
                }
                else if (movement.position.y > 0)                 //Goes to 0 on x when on top and bottom channels
                {
                    movement.SetPositionAndRotation(new Vector3(0, 1), movement.rotation);
                }
                else if (movement.position.y < 0)
                {
                    movement.SetPositionAndRotation(new Vector3(0, -1), movement.rotation);
                }

                StartCoroutine(Attacking());
            }
        }
    }


    IEnumerator randomAttacks()
    {
        while (gameObject.active)
        {
            yield return new WaitForSeconds(10f);

            if (Mathf.Abs(gameObject.transform.position.x) < 8)
                break;

            int random = Random.Range(0, 3);
            if (random == 0)
                StartCoroutine(Minions());

            else if (random == 1)
                StartCoroutine(heal());

            else if (random == 2)
                StartCoroutine(Resurface());
        }
    }

    IEnumerator Minions()
    {
        Debug.Log("Minions");
        stop = true;
        int total = enemyManager.GetComponent<EnemyManager>().getRound() / 10 + 2;

        if (gameObject.transform.position.x < -4)
        {
            for (int i = 0; i < total; i++)
            {
                yield return new WaitForSeconds(0.5f);
                Instantiate(MinionPrefab, new Vector3(gameObject.transform.position.x - 5, gameObject.transform.position.y + 5), Quaternion.identity);
                enemyManager.GetComponent<EnemyManager>().newEnemies();

                i++;
                if (i < total)
                {
                    yield return new WaitForSeconds(0.5f);
                    Instantiate(MinionPrefab, new Vector3(gameObject.transform.position.x - 5, gameObject.transform.position.y + -5), Quaternion.identity);
                    enemyManager.GetComponent<EnemyManager>().newEnemies();
                }
            }
        }
        else if (gameObject.transform.position.x > 4)
        {
            for (int i = 0; i < total; i++)
            {
                yield return new WaitForSeconds(0.5f);
                Instantiate(MinionPrefab, new Vector3(gameObject.transform.position.x + 5, gameObject.transform.position.y + 5), Quaternion.identity);

                i++;
                if (i < total)
                {
                    yield return new WaitForSeconds(0.5f);
                    Instantiate(MinionPrefab, new Vector3(gameObject.transform.position.x + 5, gameObject.transform.position.y + -5), Quaternion.identity);
                }
            }
        }

        stop = false;
    }

    IEnumerator Resurface()
    {
        Debug.Log("Resurface");
        stop = true;
        if (gameObject.transform.position.x < 0)
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            //Play going-under animation
            yield return new WaitForSeconds(1f);
            gameObject.transform.position = new Vector3(Random.Range(8f, 20f), Random.Range(-10f, 10f));
            nearestEntrance();
            yield return new WaitForSeconds(1f);
            //Play coming up animation
            gameObject.GetComponent<BoxCollider2D>().enabled = true;
        } else
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            //Play going-under animation
            yield return new WaitForSeconds(1f);
            gameObject.transform.position = new Vector3(Random.Range(-20f, -8f), Random.Range(-10f, 10f));
            nearestEntrance();
            yield return new WaitForSeconds(1f);
            //Play coming up animation
            gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }
        stop = false;
    }

    IEnumerator heal()
    {
        Debug.Log("Heal");
        stop = true;
        yield return new WaitForSeconds(1f);

        Health += (int)(Health * 0.1f);

        yield return new WaitForSeconds(1f);

        stop = false;
    }


    IEnumerator Attacking()
    {
        while (gameObject.active)
        {
            //Attacking the center
            //Play attacking animation
            yield return new WaitForSeconds(1f);

            if (goalGO != null && !stop)
            {
                goalGO.SendMessage("CenterDamage", enemy.getDamage());
            }
        }
    }

    void enteringChannel()
    {
        goalGO = GameObject.FindGameObjectWithTag("Center");
        goal = goalGO.transform;

        animator.SetTrigger("InChannel");

        if (!noRotation)
        {
            float z = (Mathf.Atan2(movement.position.y, movement.position.x) * (180 / Mathf.PI)); //Atan2 gives inverse tan in radians from current cordinates, transform takes degrees
            movement.rotation = Quaternion.Euler(0, 0, z);  //Faces towards center (If sprites faces left)
        }
    }

    private void TakeDamage(int damage)
    {
        Health -= damage;
        //Debug.Log(Health);

        animator.SetTrigger("TookDamage");

        //if (healthSlider != null) healthSlider.value = Health;
        //onEnemyTakeDmg?.Invoke(damage);
        if (Health <= 0 && !stop)
        {
            death();
        }
    }

    private void death()
    {
        enemyManager.SendMessage("EnemyDown");
        stop = true;

        animator.SetBool("Dead", true);
        //Add XP
        //Add Scrap
        StartCoroutine(DeathAnim());
    }

    IEnumerator DeathAnim()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    public Transform getGoal()
    {
        return goal;
    }
}
