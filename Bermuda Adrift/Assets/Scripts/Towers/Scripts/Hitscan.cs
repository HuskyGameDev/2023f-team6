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
            trigger = GameObject.FindGameObjectWithTag("Enemy").GetComponent<BoxCollider2D>();  //Reference a random enemy's hitbox. Might need a static hitbox with the enemy tag to avoid bugs with a timed explosion hitting an enemy outside its range

            if (trigger == null)
                Destroy(gameObject);    //No point doing an explosion if the round is over. If we do the set hitbox, we could remove this and guarantee an explosion
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

        timer = bullet.getTimer();
        if (timer == -1)        //-1 timer is no timer/max timer. The bullet will hit something or go off screen long before the timer runs out
            timer = float.MaxValue;

        gameObject.transform.localScale = new Vector3(bullet.getScale(), bullet.getScale());
        gameObject.GetComponent<Animator>().runtimeAnimatorController = animator;
    }

    public void OnTriggerEnter2D(Collider2D collision)  //Makes bullet do damage and/or do its assigned AOE effect, then destroy the bullet gameObject
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (bullet.getAOE() == 0 || landed) //If a bullet has hit something, it won't do the AOE multiple times
            {
                collision.gameObject.SendMessage("TakeDamage", damage);                         //Basic bullet hit or shrapnel/AOE hit
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
                } else if (bullet.getEffect() == 1)                                             //1 - Explosion that shakes the screen and leaves lasting AOE that damages one more time after a second
                {
                    Debug.Log("Explosion");
                    camera.SendMessage("cameraShake", 0.25f);
                    //Play explosion animation

                    stop = true;
                    landed = true;

                    collision.gameObject.SendMessage("TakeDamage", damage);     //Hide the bullet after the explosion, but leave the hitbox
                    gameObject.GetComponent<SpriteRenderer>().enabled = false;

                    new WaitForSeconds(1f);
                    collision.gameObject.SendMessage("TakeDamage", damage);     //More testing needed to see if this actually works (add visuals? Maybe create a gameobject that is JUST visuals?)
                }

            }

            new WaitForEndOfFrame();
            Destroy(gameObject);    //After all AOE stuff, the bullet is deleted. If there is something requiring a lasting hitbox, turn the sprite invisible or something
        }
    }
}
