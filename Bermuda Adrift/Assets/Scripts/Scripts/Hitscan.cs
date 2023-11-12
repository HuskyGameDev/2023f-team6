using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitscan : MonoBehaviour
{
    private Bullet bullet;
    private int damage;
    private float timer;
    private RuntimeAnimatorController animator;
    [SerializeField] private Buffs debuff;
    private int pierce;
    private float AOETimer;
    private Bullet.Effects effect;

    [SerializeField] private Transform sprite;

    private bool landed;
    private bool stop;

    private Camera camera;

    private void Start()
    {
        //For some reason setBullet() seems to run either before Start or faster than Start
        stop = false;
        landed = false;

        camera = Camera.main;   //Used for camera shake effects;
    }
    private void Update()
    {
        countdown();

        Onwards();
    }

    private void countdown()    //Counts down a timer every frame
    {
        BoxCollider2D trigger;

        if (timer <= 0 && !landed)
        {
            trigger = GameObject.Find("AOETrigger").GetComponent<BoxCollider2D>();  //Reference a hitbox offscreen to "hit".

            if (trigger == null)
                Destroy(gameObject);    //In case the offscreen hitbox isn't in the scene
            else
                OnTriggerEnter2D(trigger);
        }
        else
            timer -= Time.deltaTime;

        if (landed)
        {
            AOETimer -= Time.deltaTime;
            if (AOETimer <= 0)
                Destroy(gameObject);
        }
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

    public void Mult(float mult) { damage = (int) (damage * mult); }  //Multiplies base damage of the bullet by the damage multiplier of the tower when it's created

    public void setBullet(Bullet newBullet)     //Bullet is told what type of bullet it is and it sets up everything from there
    {
        bullet = newBullet;

        sprite = transform.GetChild(0);

        damage = bullet.getDamage();        
        animator = bullet.getAnimator();    //Animator automatically sets sprites
        timer = bullet.getTimer();
        debuff = bullet.getDebuff();
        effect = bullet.getEffect();

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
        sprite.GetComponent<Animator>().runtimeAnimatorController = animator;
        sprite.rotation = Quaternion.Euler(sprite.rotation.eulerAngles * 2);
    }

    private void setEffect(Bullet.Effects newEffect) { effect = newEffect; }    //Sets type of bullet. Mainly used for shotgun

    private void setTimer(float time)
    {
        timer = time;
        if (timer == -1)
            timer = float.MaxValue;
    }

    private void dealDamage(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<AI>().getType() == Enemy.Types.Underwater || collision.gameObject.GetComponent<AI>().getType() == Enemy.Types.WaterBoss)
            collision.gameObject.SendMessage("TakeDamage", damage * bullet.getUnderwaterDamage());

        else if (collision.gameObject.GetComponent<AI>().getType() == Enemy.Types.Airborne || collision.gameObject.GetComponent<AI>().getType() == Enemy.Types.AirborneBoss)
            collision.gameObject.SendMessage("TakeDamage", damage * bullet.getAirborneDamage());

        else
            collision.gameObject.SendMessage("TakeDamage", damage);
    }

    private void destroyBullet() 
    {
        Destroy(gameObject); 
    }

    public void OnTriggerEnter2D(Collider2D collision)  //Makes bullet do damage and/or do its assigned AOE effect, then destroy the bullet gameObject
    {
        if (collision.gameObject.tag != "Enemy" && !(collision.CompareTag("Friendly") && bullet.getFriendlyFire())) { return; }

        pierce--;
        if (bullet.getAOE() == 0 || landed || effect == Bullet.Effects.None) //If a bullet has hit something, it won't do the AOE multiple times
        {
            dealDamage(collision);                                          //Basic bullet hit or shrapnel/AOE hit
            if (debuff != null)
                collision.SendMessage("InflictDebuff", debuff);
        }
        else
        {
            sprite.GetComponent<Animator>().SetTrigger("Hit");
            gameObject.GetComponent<CircleCollider2D>().radius = bullet.getAOE();    //Grow the hitbox to match the AOE
            
            if (effect == Bullet.Effects.Shrapnel)                                               //0 - Basic bullet hit with shrapnel
            {
                stop = true;
                //Play shrapnel animation
                landed = true;

                dealDamage(collision);

                if (debuff != null)
                    collision.gameObject.SendMessage("InflictDebuff", debuff);
            }
            else if (effect == Bullet.Effects.DebuffDamage)                                      //1 - Does damage in an area and Inflicts a debuff
            {
                stop = true;
                landed = true;

                dealDamage(collision);
                //gameObject.GetComponent<SpriteRenderer>().enabled = false;

                //Switch animator controller to the explosion/fire effects
                collision.gameObject.SendMessage("InflictDebuff", debuff);
            }
            else if (effect == Bullet.Effects.Bait)                                              //2 is the bait effect
            {
                    stop = true;
                    collision.gameObject.SendMessage("baited", gameObject);
                    //Play bait-spreading animation
                    //gameObject.GetComponent<SpriteRenderer>().enabled = false;  //Make invisible
                    collision.gameObject.SendMessage("InflictDebuff", debuff);  //Should just be the Baited debuff, which just distracts the enemies
            }
            else if (effect == Bullet.Effects.Explosion)                                         // 3 is an explosion that shakes the screen and inflicts the debuff
            {
                camera.SendMessage("cameraShake", 0.25f);
                sprite.localScale = new Vector2(bullet.getAOE() * 2, bullet.getAOE() * 2);
                
                stop = true;
                landed = true;

                dealDamage(collision); 
                //gameObject.GetComponent<SpriteRenderer>().enabled = false;  //Hide the bullet after the explosion, but leave the hitbox

                //Switch animator controller to the explosion/fire effects
                collision.gameObject.SendMessage("InflictDebuff", debuff);
            }
            else if (effect == Bullet.Effects.Shotgun)
            {
                stop = true;
                landed = true;

                int count = (int) bullet.getAOE();
                int i = 0;

                if (count % 2 == 1)
                {
                    count--;
                    var pellet = Instantiate(gameObject, gameObject.transform.position, gameObject.transform.rotation);

                    pellet.SendMessage("setBullet", bullet);
                    pellet.SendMessage("setTimer", -1);
                    pellet.SendMessage("setEffect", Bullet.Effects.None);
                    pellet.GetComponent<CircleCollider2D>().radius = 1;
                }

                for (; i < count / 2; i++)
                {
                    Quaternion direction = Quaternion.Euler(gameObject.transform.rotation.eulerAngles - (Vector3.forward * ((15f / count / 2) * (i + 1))));
                    var pellet = Instantiate(gameObject, gameObject.transform.position, direction);

                    pellet.SendMessage("setBullet", bullet);
                    pellet.SendMessage("setTimer", -1);
                    pellet.SendMessage("setEffect", Bullet.Effects.None);
                    pellet.GetComponent<CircleCollider2D>().radius = 1;
                }

                for (; i < count; i++)
                {
                    Quaternion direction = Quaternion.Euler(gameObject.transform.rotation.eulerAngles + (Vector3.forward * ((15f / count / 2) * (count - i))));
                    var pellet = Instantiate(gameObject, gameObject.transform.position, direction);

                    pellet.SendMessage("setBullet", bullet);
                    pellet.SendMessage("setTimer", -1);
                    pellet.SendMessage("setEffect", Bullet.Effects.None);
                    pellet.GetComponent<CircleCollider2D>().radius = 1;
                }
            }

        }

        StartCoroutine(destroyCheck());
    }

    private IEnumerator destroyCheck()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        if ((pierce <= 0 || AOETimer <= 0) && effect == Bullet.Effects.None)
            Destroy(gameObject);    //After all AOE stuff, the bullet is deleted. If there is something requiring a lasting hitbox, turn the sprite invisible or something
    }
}
