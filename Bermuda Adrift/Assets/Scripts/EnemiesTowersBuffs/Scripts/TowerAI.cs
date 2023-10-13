using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAI : MonoBehaviour
{
    [SerializeField] private Tower tower;   //Shouldn't need to be serialized after the placing is set up
    [SerializeField] private GameObject nozzle;

    [SerializeField] private GameObject bullet;
    [SerializeField] private Bullet bulletScript;   //Same as the tower

    private GameObject target = null;
    private GameManager gameManager;
    private Animator anim;

    private Buffs buff;
    [SerializeField] private Buffs noBuff;

    //[SerializeField] Tower testTower;

    private void Start()
    {
        nozzle = Instantiate(nozzle, gameObject.transform.position, Quaternion.identity, gameObject.transform);
        anim = nozzle.GetComponent<Animator>();     //Starting-up animation could be the default animation which then goes into the idle animation unconditionally

        gameManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<GameManager>();
        buff = noBuff;
        
        if (gameManager.getGameState() == GameManager.GameState.Defend || gameManager.getGameState() == GameManager.GameState.BossRound)    //Sets a new target if created during a round. Could be removed if we can only place outside of a round
        {
            newTarget();
            StartRound();
        }
    }

    private void Update()
    {
        //if (Input.GetKeyDown("x")) { place(testTower); }  //Testing place function

        if (target != null) //Turn towards target every frame
        {
            targetPoint();
        }

        if (gameManager.getGameState() == GameManager.GameState.BossRound)  //Constantly target the closest enemy in a boss round (so they'll aim at minions)
        {
            newTarget();
        }
    }

    public void place(Tower towerType)  //Setup function
    {
        tower = towerType;
        bulletScript = tower.getDefaultBullet(); //Just tell the base what tower it needs to be and it will set up everything else.


        nozzle.GetComponent<Animator>().runtimeAnimatorController = tower.getAnim();    //If run at the same time as Start, could have some bugs with this reference
    }

    public void StartRound()    //Called when each round starts
    {
        if (target == null)     //Should be null always, might not be needed
            newTarget();
        StartCoroutine(firing());
    }

    private void targetPoint()  //finds where/how to point to look at target and points that way
    {
        Vector3 offset = target.transform.position - transform.position;
        Quaternion output = Quaternion.LookRotation(Vector3.forward, offset).normalized;

        nozzle.transform.rotation = Quaternion.Lerp(nozzle.transform.rotation, output, tower.getTurnSpeed() * buff.getTurnSpeed());
    }

    private IEnumerator firing()    //Coroutine for creating the bullets as long as there's a target
    {
        if (gameManager.getGameState() == GameManager.GameState.Defend || gameManager.getGameState() == GameManager.GameState.BossRound)    //If the round is still going, pick a new target
        {
            newTarget();
            anim.SetTrigger("TargetFound");
        }
        while (target != null && target.active)     //Will keep firing on one target until that target is dead
        {
            var boolet = Instantiate(bullet, nozzle.transform.position, nozzle.transform.rotation);     //Create bullet
            yield return new WaitForEndOfFrame();   //Wait for bullet's Start function to finish (might not be needed)
            boolet.SendMessage("setBullet", bulletScript);      //Tell the bullet what kind of bullet it needs to be
            boolet.SendMessage("Mult", tower.getDamageMult() * buff.getDamage());  //And how much damage it does

            yield return new WaitForSeconds(tower.getFireRate() * buff.getFireRate());   //For some reason the tower won't turn until this is done. Maybe something to do with it being a Coroutine?
        }
        target = null;  //Reset so it can be reassigned or for the end of the round
        if (gameManager.getGameState() == GameManager.GameState.Defend || gameManager.getGameState() == GameManager.GameState.BossRound)
            StartCoroutine(firing());
        anim.SetTrigger("TargetLost");  //Should only happen when there are no targets left
    }

    private void enemyKilled()      //Received whenever the target enemy dies so it picks a new target. Might be unnescessary
    {
        if (gameManager.getGameState() == GameManager.GameState.Defend || gameManager.getGameState() == GameManager.GameState.BossRound)
        {
            newTarget();
        }
        else
            target = null; 
    }

    private void newTarget()    //Set target to closest enemy in range
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        float distance = Mathf.Infinity;
        foreach (GameObject enemy in enemies)   //Not sure how to be more efficient than iterating through all enemies
        {
            Vector3 diff = enemy.transform.position - nozzle.transform.position;
            float curDistance = diff.sqrMagnitude;
            if (tower.getRange() > 0)
            {
                if (curDistance < distance && curDistance < tower.getRange())   //Only pick targets in a range
                {
                    target = enemy;
                    distance = curDistance;
                }
            } else       //If range is less than 0 (usually -1), infinite range
            {
                if (curDistance < distance)
                {
                    target = enemy;
                    distance = curDistance;
                }
            }
        }
    }

    private void Buff(Buffs newBuff)
    {
        buff = newBuff;
        new WaitForSeconds(buff.getDuration());
        buff = noBuff;
    }
}
