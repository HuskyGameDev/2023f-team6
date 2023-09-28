using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AI : MonoBehaviour
{
    public Enemy enemy;

    private Transform movement;
    private bool arrived;
    private Transform goal;
    private GameObject goalGO;

    [SerializeField] private Slider healthSlider;

    private void Start()
    {
        movement = gameObject.transform;
        if (healthSlider != null) healthSlider.maxValue = enemy.getHealth();

        nearestEntrance();

        float z = (Mathf.Atan2(movement.position.y - goal.position.y, movement.position.x - goal.position.x) * (180 / Mathf.PI)); //Atan2 gives inverse tan in radians from current cordinates, transform takes degrees
        movement.localRotation = Quaternion.Euler(0, 0, z);  //Faces towards goal (If sprites faces left)
    }

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
    }

    // Update is called once per frame
    void Update()
    {
        if (!arrived)
        {
            movement.position = Vector3.MoveTowards(movement.position, goal.position, enemy.getSpeed() * Time.deltaTime * 0.5f);
            if (movement.position == goal.position)
                enteringChannel();

            //Add interaction with barriers

            if (Mathf.Abs(movement.position.x) + Mathf.Abs(movement.position.y) <= 1)  //Stop when they reach the center
            {
                arrived = true;

                if (movement.position.x < 0)
                {
                    movement.SetPositionAndRotation(new Vector3(-1, 0), movement.rotation);
                } else if (movement.position.x > 0)
                {
                    movement.SetPositionAndRotation(new Vector3(1, 0), movement.rotation);
                } else if (movement.position.y > 0)                 //Goes to 0 on x when on top and bottom channels
                {
                    movement.SetPositionAndRotation(new Vector3(0, 1), movement.rotation);
                } else if (movement.position.y < 0)
                {
                    movement.SetPositionAndRotation(new Vector3(0, -1), movement.rotation);
                }

                StartCoroutine(Attacking());
            }
        }
    }

    IEnumerator Attacking()
    {
        while (gameObject.active)
        {
            //Attacking the center
            //Play attacking animation
            yield return new WaitForSeconds(1f);

            goalGO.SendMessage("CenterDamage", enemy.getDamage());
        }
    }

    void enteringChannel()
    {
        goalGO = GameObject.FindGameObjectWithTag("Center");
        goal = goalGO.transform;
        float z = (Mathf.Atan2(movement.position.y, movement.position.x) * (180 / Mathf.PI)); //Atan2 gives inverse tan in radians from current cordinates, transform takes degrees
        movement.rotation = Quaternion.Euler(0, 0, z);  //Faces towards center (If sprites faces left)
    }

    private void TakeDamage(int damage)
    {
        enemy.takeDamage(damage);
        //if (healthSlider != null) healthSlider.value = Health;
        //onEnemyTakeDmg?.Invoke(damage);
        if (enemy.getHealth() <= 0)
            death();
    }

    private void death()
    {
        //Play death animation
        Destroy(gameObject);
        //Add XP
        //Add Scrap
    }
}
