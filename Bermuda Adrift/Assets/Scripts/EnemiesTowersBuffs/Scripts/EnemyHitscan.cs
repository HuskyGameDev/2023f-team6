using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitscan : MonoBehaviour
{
    private Bullet bullet;
    private bool stop;
    Camera camera;

    private void setBullet(Bullet bullet)
    {
        this.bullet = bullet;
        camera = Camera.main;
        gameObject.transform.localScale = new Vector3(bullet.getScale(), bullet.getScale());    //Size of the bullet
    }
    private void Update()
    {
        Onwards();
    }

    private void Onwards()  //If it's not told to stop, continue (locally) upwards
    {
        if (!stop)
        {
            gameObject.transform.Translate(transform.up * bullet.getProjectileSpeed() * Time.deltaTime);

            if (Mathf.Abs(gameObject.transform.position.x) >= Mathf.Abs((camera.transform.position.x - camera.orthographicSize) * 2f) || Mathf.Abs(gameObject.transform.position.y) >= Mathf.Abs(camera.transform.position.y - camera.orthographicSize * 1.15f))
            {
                Destroy(gameObject);    //Delete the bullet if off screen
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Center") && !collision.CompareTag("Friendly"))
            return;

        else if (collision.CompareTag("Friendly"))
        {
            //Play destroyed-bullet effect maybe?
            Destroy(gameObject);
        }

        collision.SendMessage("TakeDamage", bullet.getDamage());
        Destroy(gameObject);
    }
}
