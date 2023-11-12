using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TowerAI : MonoBehaviour
{
    public static event Action OnCancel;
    public static event Action OnTowerPlaced;

    private Tower tower;
    [SerializeField] private GameObject nozzle;

    [SerializeField] private GameObject bullet;
    private Bullet bulletScript;

    private int upgradeLevel = 0;
    private float damageMult;
    private float turnSpeed;
    private float range;

    private GameObject target = null;
    private GameManager gameManager;
    private Animator anim;

    private Buffs[] buffs;

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

    private void Update()
    {
        if (!placed)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if ((Mathf.Abs(transform.position.x) <= 6 && Mathf.Abs(transform.position.x) > 1) && (Mathf.Abs(transform.position.y) <= 6 && Mathf.Abs(transform.position.y) > 1) && gameManager.gameObject.GetComponent<BuildManager>().approvePosition(transform.position))
                {
                    OnTowerPlaced?.Invoke();
                    placed = true;
                    anim.SetTrigger("Placed");
                    gameManager.spendScrap(tower.getCost());
                }
            }
            else if (Input.GetMouseButtonDown(1) || Input.GetKeyDown("escape"))
            {
                OnCancel?.Invoke();
                Destroy(gameObject);
            }

            var mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0f;
            gameObject.transform.position = new Vector3(Mathf.Round(mouseWorldPosition.x * 0.5f) * 2f, Mathf.Round(mouseWorldPosition.y * 0.5f) * 2f);

            if (gameManager.getGameState() != GameManager.GameState.Idle)
            {
                OnCancel?.Invoke();
                Destroy(gameObject);
            }
            
            return;
        }

        //if (Input.GetKeyDown("u") && gameManager.getGameState() == GameManager.GameState.Idle) { upgrade(true); }

        if (target != null) //Turn towards target every frame
            targetPoint();

        if (gameManager.getGameState() == GameManager.GameState.BossRound || gameManager.getGameState() == GameManager.GameState.Defend)
            newTarget();
    }

    public void place(Tower towerType)  //Setup function
    {
        tower = towerType;
        bulletScript = tower.getDefaultBullet(); //Just tell the base what tower it needs to be and it will set up everything else.

        damageMult = tower.getDamageMult();
        turnSpeed = tower.getTurnSpeed();
        range = tower.getRange();


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

        nozzle.transform.rotation = Quaternion.Lerp(nozzle.transform.rotation, output, turnSpeed * getTurnSpeed());
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
        boolet.SendMessage("Mult", damageMult * getDamage());  //And how much damage it does
    }

    #region Upgrade Methods
    private void upgrade(bool path)  //true for path a, false for path b
    {
        if (upgradeLevel >= 3)
            return;

        if (upgradeLevel == 0)  //Upgrade 1
        {
            if (!gameManager.cost(tower.U1getCost())) { return; }

            anim.SetTrigger("UpgradeA");    //Default upgrade path
            upgradeLevel++;

            bulletScript = tower.U1getBullet();
            damageMult = tower.U1getDamageMult();
            turnSpeed = tower.U1getTurnSpeed();
            range = tower.U1getRange();

            gameManager.spendScrap(tower.U1getCost());

        }
        else if (upgradeLevel == 1) //Upgrade A1 or B1
        {
            if (path)   //Only spot the bool matters
            {
                if (!gameManager.cost(tower.UA1getCost())) { return; }

                anim.SetTrigger("UpgradeA");
                upgradeLevel++; //Sets upgrade level to 2

                bulletScript = tower.UA1getBullet();
                damageMult = tower.UA1getDamageMult();
                turnSpeed = tower.UA1getTurnSpeed();
                range = tower.UA1getRange();

                gameManager.spendScrap(tower.UA1getCost());
            }
            else
            {
                if (!gameManager.cost(tower.UB1getCost())) { return; }

                anim.SetTrigger("UpgradeB");
                upgradeLevel += 2;  //Sets upgrade level to 3

                bulletScript = tower.UB1getBullet();
                damageMult = tower.UB1getDamageMult();
                turnSpeed = tower.UB1getTurnSpeed();
                range = tower.UB1getRange();

                gameManager.spendScrap(tower.UB1getCost());
            }
        }
        else    //Upgrade A2 or B2
        {
            if (upgradeLevel == 2)  //Goes based off upgrade level instead of boolean so stats don't get messed up if given the wrong boolean
            {
                if (!gameManager.cost(tower.UA2getCost())) { return; }

                anim.SetTrigger("UpgradeA");    //Default upgrade path
                upgradeLevel += 2;  //Fully upgraded A branch is 4

                bulletScript = tower.UA2getBullet();
                damageMult = tower.UA2getDamageMult();
                turnSpeed = tower.UA2getTurnSpeed();
                range = tower.UA2getRange();

                gameManager.spendScrap(tower.UA2getCost());
            }
            else
            {
                if (!gameManager.cost(tower.UB2getCost())) { return; }

                anim.SetTrigger("UpgradeA");    //Default upgrade path, don't need to differentiate after the branch
                upgradeLevel += 2;  //Fully upgraded B branch is 5

                bulletScript = tower.UB2getBullet();
                damageMult = tower.UB2getDamageMult();
                turnSpeed = tower.UB2getTurnSpeed();
                range = tower.UB2getRange();

                gameManager.spendScrap(tower.UB2getCost());
            }
        }
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
            if (range > 0)
            {
                if (curDistance < distance && curDistance < range)   //Only pick targets in a range
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
    public int getUpgradeLevel() { return upgradeLevel; }
}