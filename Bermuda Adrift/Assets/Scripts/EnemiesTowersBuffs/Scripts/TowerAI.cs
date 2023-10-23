using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TowerAI : MonoBehaviour
{
    [SerializeField] private Tower tower;   //Shouldn't need to be serialized after the placing is set up
    [SerializeField] private GameObject nozzle;

    [SerializeField] private GameObject bullet;
    [SerializeField] private Bullet bulletScript;   //Same as the tower

    private GameObject target = null;
    private GameManager gameManager;
    private Animator anim;

    private Buffs[] buffs;

    [SerializeField] Tower testTower;

    private Boolean placed = false;
    private BoxCollider2D[] colliders;

    private bool temp = false;

    private void Start()
    {
        nozzle = Instantiate(nozzle, gameObject.transform.position, Quaternion.identity, gameObject.transform);
        anim = nozzle.GetComponent<Animator>();     //Starting-up animation could be the default animation which then goes into the idle animation unconditionally
        colliders = new BoxCollider2D[1];
        colliders[0] = gameObject.GetComponent<BoxCollider2D>();
        gameObject.GetComponent<SpriteRenderer>().sprite = tower.getBaseSprite();

        gameManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<GameManager>();
        buffs = new Buffs[10];
        
        if (gameManager.getGameState() == GameManager.GameState.Defend || gameManager.getGameState() == GameManager.GameState.BossRound)    //Sets a new target if created during a round. Could be removed if we can only place outside of a round
        {
            newTarget();
            StartRound();
        }
    }

    public void OnMouseDown(){
        if ((Mathf.Abs(transform.position.x) <= 6 && Mathf.Abs(transform.position.x) > 1) && (Mathf.Abs(transform.position.y) <= 6 && Mathf.Abs(transform.position.y) > 1)) { 
            placed = true;
            anim.SetTrigger("Placed");
            gameManager.spendScrap(tower.getCost());
        }
    }

    private void Update()
    {
        if (!placed)
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnMouseDown();
            }
            else if (Input.GetMouseButtonDown(1))
                Destroy(gameObject);

            var mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0f;
            gameObject.transform.position = new Vector3(Mathf.Round(mouseWorldPosition.x), Mathf.Round(mouseWorldPosition.y));

            if (gameManager.getGameState() != GameManager.GameState.Idle)
                Destroy(gameObject);
            
            return;
        }

        if (Input.GetKeyDown("u") && gameManager.getGameState() == GameManager.GameState.Idle) { upgrade(true); }

        if (target != null) //Turn towards target every frame
            targetPoint();

        if (gameManager.getGameState() == GameManager.GameState.BossRound || gameManager.getGameState() == GameManager.GameState.Defend)
            newTarget();
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
        anim.SetTrigger("TargetFound");
    }

    private void targetPoint()  //finds where/how to point to look at target and points that way
    {
        Vector3 offset = target.transform.position - transform.position;
        Quaternion output = Quaternion.LookRotation(Vector3.forward, offset).normalized;

        nozzle.transform.rotation = Quaternion.Lerp(nozzle.transform.rotation, output, tower.getTurnSpeed() * getTurnSpeed());
    }

    private void fire()    //Coroutine for creating the bullets as long as there's a target
    {
        anim.speed = getFireRate();

        if (gameManager.getGameState() != GameManager.GameState.BossRound && gameManager.getGameState() != GameManager.GameState.Defend)    //If not in a round
            anim.SetBool("TargetFound", false);
        else
            anim.SetBool("TargetFound", true);

        Quaternion bulletRotation = Quaternion.Euler(nozzle.transform.rotation.eulerAngles - (nozzle.transform.rotation.eulerAngles / 2));
        var boolet = Instantiate(bullet, nozzle.transform.position, bulletRotation);     //Create bullet

        boolet.SendMessage("setBullet", bulletScript);      //Tell the bullet what kind of bullet it needs to be
        boolet.SendMessage("Mult", tower.getDamageMult() * getDamage());  //And how much damage it does
    }

    #region Upgrade Methods
    private void upgrade(bool path)  //true for path a, false for path b
    {
        if (!gameManager.cost(300) || temp || tower.name != "T_Machine Gun")
            return;

        temp = true;

        //Maybe just change to another tower?
        if (path)
            anim.SetTrigger("UpgradeA");    //Will need functionality for changing bullets, stats, etc, only have an animation for now
        else
            anim.SetTrigger("UpgradeB");
        gameManager.spendScrap(300);
    }
    #endregion

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

    //Buff Managing
    private void addBuff(Buffs debuff)    //Add a debuff to the list of debuffs
    {
        for (int i = 0; i < buffs.Length; i++)
        {
            if (buffs[i] == null)
            {
                buffs[i] = debuff;
                return;
            }
        }
    }
    private void removeBuff(Buffs debuff) //Removes a debuff from the list of debuffs currently applied
    {
        int i = 0;
        for (; i < buffs.Length; i++)
        {
            if (buffs[i] == debuff)
            {
                for (; i < buffs.Length - 1; i++)
                {
                    buffs[i] = buffs[i + 1];
                }
                buffs[i] = null;
            }
        }
    }
    private IEnumerator Buff(Buffs newDebuff)  //adds a debuff to the list
    {
        addBuff(newDebuff);

        yield return new WaitForSeconds(newDebuff.getDuration());

        removeBuff(newDebuff);
    }


    //Get functions for buffs
    private float getFireRate()    //Gives total speed penalty/buff (multiplicative)
    {
        float fireRate = 1;
        for (int i = 0; i < buffs.Length && buffs[i] != null; i++)
        {
            fireRate *= buffs[i].getFireRate();
        }
        return fireRate;
    }
    private float getTurnSpeed()    //Gives total speed penalty/buff (multiplicative)
    {
        float turnSpeed = 1;
        for (int i = 0; i < buffs.Length && buffs[i] != null; i++)
        {
            turnSpeed *= buffs[i].getTurnSpeed();
        }
        return turnSpeed;
    }
    private float getDamage()    //Gives total speed penalty/buff (multiplicative)
    {
        float damage = 1;
        for (int i = 0; i < buffs.Length && buffs[i] != null; i++)
        {
            damage *= buffs[i].getDamage();
        }
        return damage;
    }

    public bool getPlaced() { return placed; }
}
