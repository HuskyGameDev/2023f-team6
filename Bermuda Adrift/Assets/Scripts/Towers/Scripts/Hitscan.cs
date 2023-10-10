using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitscan : MonoBehaviour
{
    [SerializeField] private Bullet bullet;
    private int damage;
    private Camera camera;
    [SerializeField] private GameObject aoePrefab;
    private bool landed = false;
    private bool stop = false;
    private void Start()
    {
        camera = Camera.main;
        damage = bullet.getDamage();
        //aoe = GameObject.Find("AOEHitbox");
        gameObject.transform.Rotate(-gameObject.transform.rotation.eulerAngles / 2);
    }
    private void Update()
    {
        if (!stop)
        {
            gameObject.transform.Translate(transform.up * bullet.getProjectileSpeed() * Time.deltaTime);

            if (Mathf.Abs(gameObject.transform.position.x) >= Mathf.Abs((camera.transform.position.x - camera.orthographicSize) * 2f) || Mathf.Abs(gameObject.transform.position.y) >= Mathf.Abs(camera.transform.position.y - camera.orthographicSize * 1.15f))
            {
                Destroy(gameObject);
            }
        }
    }

    public void Mult(int mult) 
    { 
        damage *= mult;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (bullet.getAOE() == 0 || landed)
            {
                collision.gameObject.SendMessage("TakeDamage", damage);
            } else
            {
                /*var Hitbox = Instantiate(aoePrefab, gameObject.transform);
                Debug.Log(Hitbox);
                new WaitForEndOfFrame();
                Debug.Log(Hitbox);
                float[] toGo = { bullet.getAOE(), 2, bullet.getEffect(), bullet.getDamage() };
                Hitbox.
                    SendMessage("AOEHit", toGo);
                */
                gameObject.GetComponent<CircleCollider2D>().radius = bullet.getAOE();

                if (bullet.getEffect() == 0)
                {
                    stop = true;
                    //Play shrapnel animation
                    landed = true;
                    collision.gameObject.SendMessage("TakeDamage", damage);
                }

            }

            new WaitForEndOfFrame();
            Destroy(gameObject);
        }
    }
}
