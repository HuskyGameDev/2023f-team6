using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirborneAI : MonoBehaviour
{
    [SerializeField]
    private Transform movement;
    private BoxCollider2D hitbox;
    private bool moving;
    private float speed;

    private float x;
    private float y;
    private float z;
    private void Start()
    {
        moving = true;
        speed = gameObject.GetComponent<EnemyBase>().getSpeed();

        movement = gameObject.transform;
        z = (Mathf.Atan2(movement.position.y, movement.position.x) * (180 / Mathf.PI)); //Atan2 gives inverse tan in radians from current cordinates, transform takes degrees
        movement.rotation = Quaternion.Euler(0, 0, z);  //Faces towards center (If sprites faces left)
    }
    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            x = -movement.position.x / (movement.position.x * movement.position.x + movement.position.y * movement.position.y);  //Move a distance of 1 * speed
            y = -movement.position.y / (movement.position.x * movement.position.x + movement.position.y * movement.position.y);

            movement.Translate(new Vector3(x * speed, y * speed));
        } else
        {
            //Attacking the center
            //Play attacking animation
            gameObject.SendMessage("CenterDamage", 1);
        }
    }

    private void OnCollisionEnter2D(Collision2D hit)
    {
        Debug.Log("Hit the center");
        if (hit.gameObject.tag == "Center")    //Stop moving and start attacking
            moving = false;
    }
}
