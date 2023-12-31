using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Hitscan : MonoBehaviour
{
    private Bullet bullet;
    private int damage;
    private float timer;
    private float underwaterMult;
    private float airborneMult;
    private RuntimeAnimatorController animator;
    [SerializeField] private Buffs debuff;
    private int pierce;
    private float AOETimer;
    private Bullet.Effects effect;
    private float critChance;
    private float levelScale = 1;

    [SerializeField] private Transform sprite;

    private bool landed;
    private bool stop;

    new private Camera camera;

    private void Start()
    {
        //For some reason setBullet() seems to run either before Start or faster than Start
        
        landed = false;
        if (critChance == 0)
            setCritChance(1);

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

            Color fade = transform.GetChild(0).GetComponent<SpriteRenderer>().color;
            fade.a = AOETimer / bullet.getAOETimer();
            transform.GetChild(0).GetComponent<SpriteRenderer>().color = fade;

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
    public void setLevelScale(float levelScale) { this.levelScale = levelScale; }
    public void setBullet(Bullet newBullet)     //Bullet is told what type of bullet it is and it sets up everything from there
    {
        bullet = newBullet;

        sprite = transform.GetChild(0);
        stop = false;

        damage = bullet.getDamage();        
        animator = bullet.getAnimator();    //Animator automatically sets sprites
        timer = bullet.getTimer();
        debuff = bullet.getDebuff();
        effect = bullet.getEffect();
        underwaterMult = bullet.getUnderwaterDamage();
        airborneMult = bullet.getAirborneDamage();

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
        gameObject.GetComponent<CircleCollider2D>().radius = bullet.getScale();

        sprite.GetComponent<Animator>().runtimeAnimatorController = animator;
        sprite.localScale = new Vector2(bullet.getScale() * 2, bullet.getScale() * 2);
        sprite.rotation = Quaternion.Euler(sprite.rotation.eulerAngles * 2);
    }

    private void setEffect(Bullet.Effects newEffect) { effect = newEffect; }    //Sets type of bullet. Mainly used for shotgun

    private void setTimer(float time)
    {
        timer = time;
        if (timer == -1)
            timer = float.MaxValue;
    }

    private void UnderwaterMult(float mult) { underwaterMult *= mult; }
    private void AirborneMult(float mult) { airborneMult *= mult; }
    private void setCritChance(float chance) { critChance = chance; }
    private void initialDelay(int frames)
    {
        stop = true;
        StartCoroutine(delay(frames));
    }
    private IEnumerator delay(int offset)
    {
        for (int i = 0; i < offset; i++)
            yield return new WaitForEndOfFrame();

        stop = false;
    }

    private void dealDamage(Collider2D collision)
    {
        if (collision.name == "AOETrigger") return;
        int crit = critCalc();

        if (crit > 1)
        {
            if (collision.gameObject.GetComponent<AI>().getType() == Enemy.Types.Underwater || collision.gameObject.GetComponent<AI>().getType() == Enemy.Types.WaterBoss)
                collision.gameObject.SendMessage("CritDamage", damage * underwaterMult * crit * levelScale);

            else if (collision.gameObject.GetComponent<AI>().getType() == Enemy.Types.Airborne || collision.gameObject.GetComponent<AI>().getType() == Enemy.Types.AirborneBoss)
                collision.gameObject.SendMessage("CritDamage", damage * airborneMult * crit * levelScale);

            else
                collision.gameObject.SendMessage("CritDamage", damage);
        }
        else
        {
            if (collision.gameObject.GetComponent<AI>().getType() == Enemy.Types.Underwater || collision.gameObject.GetComponent<AI>().getType() == Enemy.Types.WaterBoss)
                collision.gameObject.SendMessage("TakeDamage", damage * underwaterMult * levelScale);

            else if (collision.gameObject.GetComponent<AI>().getType() == Enemy.Types.Airborne || collision.gameObject.GetComponent<AI>().getType() == Enemy.Types.AirborneBoss)
                collision.gameObject.SendMessage("TakeDamage", damage * airborneMult * levelScale);

            else
                collision.gameObject.SendMessage("TakeDamage", damage);
        }
    }
    private int critCalc()
    {
        if (Random.Range(0, 1f) < (critChance * 0.1))
        {
            return 2;
        }

        return 1;
    }

    private void destroyBullet() 
    {
        Destroy(gameObject); 
    }

    public void OnTriggerEnter2D(Collider2D collision)  //Makes bullet do damage and/or do its assigned AOE effect, then destroy the bullet gameObject
    {
        if (collision.gameObject.tag != "Enemy" && !(collision.CompareTag("Friendly") && bullet.getFriendlyFire())) { return; }

        pierce--;
        if (bullet.getAOE() == 0 || (landed && effect != Bullet.Effects.Bait) || effect == Bullet.Effects.None) //If a bullet has hit something, it won't do the AOE multiple times
        {
            dealDamage(collision);                                          //Basic bullet hit or shrapnel/AOE hit
            if (debuff != null && collision.name != "AOETrigger")
                collision.SendMessage("InflictDebuff", debuff);
        }
        else
        {
            sprite.GetComponent<Animator>().SetTrigger("Hit");
            gameObject.GetComponent<CircleCollider2D>().radius = bullet.getAOE();    //Grow the hitbox to match the AOE
            
            sprite.localScale = new Vector2(bullet.getAOE() * 2, bullet.getAOE() * 2);

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
                if (collision.name != "AOETrigger")
                    collision.gameObject.SendMessage("InflictDebuff", debuff);
            }
            else if (effect == Bullet.Effects.Bait)                                              //2 is the bait effect
            {
                stop = true;
                landed = true;
                collision.gameObject.SendMessage("baited", gameObject);
                if (collision.name != "AOETrigger")
                    collision.gameObject.SendMessage("InflictDebuff", debuff);  //Should just be the Baited debuff, which just distracts the enemies
            }
            else if (effect == Bullet.Effects.Explosion)                                         // 3 is an explosion that shakes the screen and inflicts the debuff
            {
                camera.SendMessage("cameraShake", 0.25f);
                
                stop = true;
                landed = true;

                dealDamage(collision); 
                //gameObject.GetComponent<SpriteRenderer>().enabled = false;  //Hide the bullet after the explosion, but leave the hitbox

                //Switch animator controller to the explosion/fire effects
                if (collision.name != "AOETrigger")
                    collision.gameObject.SendMessage("InflictDebuff", debuff);
            }
            else if (effect == Bullet.Effects.Shotgun)
            {
                stop = true;
                landed = true;

                int count = (int) bullet.getAOE();
                float maxDegree = count / 5f;
                int delayVariation = count / 2;

                for (int i = 0; i < count; i++)
                {
                    Quaternion direction = Quaternion.Euler(gameObject.transform.rotation.eulerAngles + (Vector3.forward * Random.Range(-maxDegree, maxDegree)));

                    var pellet = Instantiate(gameObject, gameObject.transform.position, direction);

                    pellet.SendMessage("setBullet", bullet);
                    pellet.SendMessage("setTimer", -1);
                    pellet.SendMessage("setEffect", Bullet.Effects.None);
                    pellet.GetComponent<CircleCollider2D>().radius = bullet.getScale();

                    pellet.SendMessage("initialDelay", Random.Range(0, delayVariation));
                }

                Destroy(gameObject);

                /*
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
                */
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
    public float getAOETimer() { return AOETimer; }
}
