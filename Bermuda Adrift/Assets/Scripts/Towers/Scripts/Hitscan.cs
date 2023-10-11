using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitscan : MonoBehaviour
{
    private Bullet bullet;
    private int damage;
    private float timer;
    private RuntimeAnimatorController animator;

    private bool landed = false;
    private bool stop = false;

    private Camera camera;

    private void Start()
    {
        camera = Camera.main;

        gameObject.transform.Rotate(-gameObject.transform.rotation.eulerAngles / 2);
        gameObject.GetComponent<Animator>().runtimeAnimatorController = animator;
    }
    private void Update()
    {
        if (timer <= 0)
        {
            BoxCollider2D trigger = GameObject.FindGameObjectWithTag("Enemy").GetComponent<BoxCollider2D>();
            if (trigger == null)
                Destroy(gameObject);
            else
                OnTriggerEnter2D(trigger);
        } else
            timer -= Time.deltaTime;


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

    public void setBullet(Bullet newBullet) 
    { 
        bullet = newBullet;

        damage = bullet.getDamage();
        gameObject.GetComponent<SpriteRenderer>().sprite = bullet.getSprite();
        animator = bullet.getAnimator();
        timer = bullet.getTimer();

        if (bullet.getTimer() == -1)
            timer = float.MaxValue;
        else
            timer = bullet.getTimer();

        gameObject.transform.localScale = new Vector3(bullet.getScale(), bullet.getScale()); 
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (bullet.getAOE() == 0 || landed)
            {
                collision.gameObject.SendMessage("TakeDamage", damage);                         //Basic bullet hit
            }
            else
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

                if (bullet.getEffect() == 0)                                                    //0 - Basic bullet hit with shrapnel
                {
                    stop = true;
                    //Play shrapnel animation
                    landed = true;
                    collision.gameObject.SendMessage("TakeDamage", damage);
                } else if (bullet.getEffect() == 1)                                             //1 - Explosion that shakes the screen and leaves lasting AOE that damages one more time after a second
                {
                    Debug.Log("Explosion");
                    camera.SendMessage("cameraShake", 0.25f);
                    stop = true;
                    landed = true;
                    collision.gameObject.SendMessage("TakeDamage", damage);

                    new WaitForSeconds(1f);
                    collision.gameObject.SendMessage("TakeDamage", damage);
                }

            }

            new WaitForEndOfFrame();
            Destroy(gameObject);
        }
    }
}
