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

    private bool landed = false;
    private bool stop = false;

    private Camera camera;

    private void Start()
    {
        //For some reason setBullet() seems to run either before Start or faster than Start

        camera = Camera.main;   //Used for camera shake effects;

        gameObject.transform.Rotate(-gameObject.transform.rotation.eulerAngles / 2);    //Need to rotate a little more to fire out the end of the barrel. Could maybe be included in the calculation that spawns the bullets
    }
    private void Update()
    {
        countdown();

        Onwards();
    }

    private void countdown()    //Counts down a timer every frame
    {
        BoxCollider2D trigger;

        if (timer <= 0)
        {
            trigger = GameObject.Find("AOETrigger").GetComponent<BoxCollider2D>();  //Reference a hitbox offscreen to "hit".

            if (trigger == null)
                Destroy(gameObject);    //In case the offscreen hitbox isn't in the scene
            else
                OnTriggerEnter2D(trigger);
        }
        else
            timer -= Time.deltaTime;
    }

    private void Onwards()  //If it's not told to stop, continue (locally) upwards
    {
        if (!stop)
        {
            gameObject.transform.Translate(transform.up * bullet.getProjectileSpeed() * Time.deltaTime);

            if (Mathf.Abs(gameObject.transform.position.x) >= Mathf.Abs((camera.transform.position.x - camera.orthographicSize) * 2f) || Mathf.Abs(gameObject.transform.position.y) >= Mathf.Abs(camera.transform.position.y - camera.orthographicSize * 1.15f))
            {
                Destroy(gameObject);    //Delete the bullet if 
            }
        }
    }

    public void Mult(int mult) { damage *= mult; }  //Multiplies base damage of the bullet by the damage multiplier of the tower when it's created

    public void setBullet(Bullet newBullet)     //Bullet is told what type of bullet it is and it sets up everything from there
    { 
        bullet = newBullet;

        damage = bullet.getDamage();        
        animator = bullet.getAnimator();    //Animator automatically sets sprites
        timer = bullet.getTimer();
        debuff = bullet.getDebuff();
        pierce = bullet.getPierce();

        timer = bullet.getTimer();
        if (timer == -1)        //-1 timer is no timer/max timer. The bullet will hit something or go off screen long before the timer runs out
            timer = float.MaxValue;

        gameObject.transform.localScale = new Vector3(bullet.getScale(), bullet.getScale());    //Size of the bullet
        gameObject.GetComponent<Animator>().runtimeAnimatorController = animator;
    }

    public void OnTriggerEnter2D(Collider2D collision)  //Makes bullet do damage and/or do its assigned AOE effect, then destroy the bullet gameObject
    {
        if (collision.gameObject.tag == "Enemy")
        {
            pierce--;
            if (bullet.getAOE() == 0 || landed) //If a bullet has hit something, it won't do the AOE multiple times
            {
                collision.gameObject.SendMessage("TakeDamage", damage);                         //Basic bullet hit or shrapnel/AOE hit
                if (debuff != null)
                    collision.SendMessage("InflictDebuff", debuff);
            }
            else
            {
                gameObject.GetComponent<CircleCollider2D>().radius = bullet.getAOE();

                if (bullet.getEffect() == 0)                                                    //0 - Basic bullet hit with shrapnel
                {
                    stop = true;
                    //Play shrapnel animation
                    landed = true;
                    collision.gameObject.SendMessage("TakeDamage", damage);
                    if (debuff != null)
                        collision.gameObject.SendMessage("InflictDebuff", debuff);
                } 
                else if (bullet.getEffect() == 1)                                             //1 - Explosion that shakes the screen and leaves lasting AOE that damages one more time after a second
                {
                    camera.SendMessage("cameraShake", 0.25f);
                    //Play explosion animation

                    stop = true;
                    landed = true;

                    collision.gameObject.SendMessage("TakeDamage", damage);     //Hide the bullet after the explosion, but leave the hitbox
                    gameObject.GetComponent<SpriteRenderer>().enabled = false;

                    //Switch animator controller to the explosion/fire effects
                    collision.gameObject.SendMessage("InflictDebuff", debuff);
                } 
                else if (bullet.getEffect() == 2)                                             //2 is the bait effect
                {
                    if (stop)
                    {
                        collision.gameObject.SendMessage("baited", gameObject);
                    }
                    else
                    {
                        stop = true;
                        collision.gameObject.SendMessage("baited", gameObject.transform.position);
                        //Play bait-spreading animation
                        gameObject.GetComponent<SpriteRenderer>().enabled = false;  //Make invisible
                        new WaitForSeconds(5f);
                    }
                }

            }

            new WaitForEndOfFrame();
            if (pierce <= 0)
                Destroy(gameObject);    //After all AOE stuff, the bullet is deleted. If there is something requiring a lasting hitbox, turn the sprite invisible or something
        }
    }
}
