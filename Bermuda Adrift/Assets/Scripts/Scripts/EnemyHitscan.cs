using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitscan : MonoBehaviour
{
    private Bullet bullet;
    private bool stop;
    bool landed;

    int damage;
    RuntimeAnimatorController animator;
    float timer;
    Buffs debuff;
    Bullet.Effects effect;
    float underwaterMult;
    float airborneMult;
    float homing;
    int pierce;
    float AOETimer;
    float distanceTraveled;

    private void Start()
    {
        landed = false;
        transform.GetChild(0).localRotation = transform.rotation;
    }

    private void setBullet(Bullet newBullet)
    {
        bullet = newBullet;

        Transform sprite = transform.GetChild(0);

        stop = false;

        damage = bullet.getDamage();
        animator = bullet.getAnimator();    //Animator automatically sets sprites
        timer = bullet.getTimer();
        debuff = bullet.getDebuff();
        effect = bullet.getEffect();
        underwaterMult = bullet.getUnderwaterDamage();
        airborneMult = bullet.getAirborneDamage();
        homing = bullet.getHomingStrength();

        pierce = bullet.getPierce();
        if (pierce == -1)
            pierce = int.MaxValue;

        timer = bullet.getTimer();
        if (timer == -1)        //-1 timer is no timer/max timer. The bullet will hit something or go off screen long before the timer runs out
            timer = float.MaxValue;

        AOETimer = bullet.getAOETimer();
        if (AOETimer == -1)
            AOETimer = float.MaxValue;

        gameObject.transform.localScale = new Vector3(bullet.getScale(), bullet.getScale());    //Size of the bullet
        gameObject.GetComponent<CircleCollider2D>().radius = bullet.getScale() * 1.5f;

        sprite.GetComponent<Animator>().runtimeAnimatorController = animator;
        sprite.localScale = new Vector2(bullet.getScale() * 2, bullet.getScale() * 2);

        if (bullet.hasTypes())
        {
            if (bullet.getEffect() == Bullet.Effects.Shotgun)
            {
                stop = true;
                landed = true;

                int count = (int)bullet.getAOE();
                float maxDegree = count / 5f;
                int delayVariation = count / 2;

                for (int i = 0; i < count; i++)
                {
                    Quaternion direction = Quaternion.Euler(gameObject.transform.rotation.eulerAngles + (Vector3.forward * Random.Range(-maxDegree, maxDegree)));

                    var pellet = Instantiate(gameObject, gameObject.transform.position, direction);

                    Bullet randomBullet = bullet.getBulletAtIndex(Random.Range(0, bullet.getNumOfTypes()));

                    pellet.SendMessage("setBullet", randomBullet);
                    pellet.GetComponent<CircleCollider2D>().radius = bullet.getScale();

                    pellet.SendMessage("initialDelay", Random.Range(0, delayVariation));
                }

                Destroy(gameObject);
            }
            else
            {
                setBullet(bullet.getBulletAtIndex(Random.Range(0, bullet.getNumOfTypes())));
            }
        }
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

            distanceTraveled += (transform.up * bullet.getProjectileSpeed() * Time.deltaTime).magnitude;

            if (Mathf.Abs(gameObject.transform.position.x) >= Mathf.Abs((Camera.main.transform.position.x - Camera.main.orthographicSize) * 2f) 
                || 
                Mathf.Abs(gameObject.transform.position.y) >= Mathf.Abs(Camera.main.transform.position.y - Camera.main.orthographicSize * 1.15f))
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

        collision.SendMessage("bulletTakeDamage", bullet.getDamage());
        Destroy(gameObject);
    }
}
