using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    private Transform movement;
    private BoxCollider2D hitbox;
    private bool inChannel;
    private bool arrived;
    private float speed;
    [SerializeField]
    private Transform goal;
    private void Start()
    {
        speed = gameObject.GetComponent<EnemyBase>().getSpeed();
        movement = gameObject.transform;

        nearestEntrance();

        float z = (Mathf.Atan2(movement.position.y - goal.position.y, movement.position.x - goal.position.x) * (180 / Mathf.PI)); //Atan2 gives inverse tan in radians from current cordinates, transform takes degrees
        movement.rotation = Quaternion.Euler(0, 0, z);  //Faces towards goal (If sprites faces left)
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
            movement.position = Vector3.MoveTowards(movement.position, goal.position, speed * Time.deltaTime);
            if (movement.position == goal.position)
                enteringChannel();
        } else
        {
            //Attacking the center
            //Play attacking animation
            gameObject.SendMessage("CenterDamage", 1);
        }
    }

    void enteringChannel()
    {
        goal = GameObject.FindGameObjectWithTag("Center").transform;
        float z = (Mathf.Atan2(movement.position.y, movement.position.x) * (180 / Mathf.PI)); //Atan2 gives inverse tan in radians from current cordinates, transform takes degrees
        movement.rotation = Quaternion.Euler(0, 0, z);  //Faces towards center (If sprites faces left)
    }

    void OnCollisionEnter2D(Collision2D hit)
    {
        Debug.Log("Hit the center");
        if (hit.gameObject.tag == "Center")    //Stop moving and start attacking
            arrived = true;
    }
}
