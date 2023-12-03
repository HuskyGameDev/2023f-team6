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
    public static event Action OnUpgradeMenuOpen;
    public static event Action<TowerAI> OnUpgraded;

    public enum Priority { Closest, Furthest, Strongest, Fastest };
    private bool ignoreDecoys;

    private Tower tower;
    [SerializeField] private GameObject nozzle;

    [SerializeField] private GameObject bullet;
    private Bullet bulletScript;

    private int upgradeLevel = 0;
    private float damageMult;
    private float turnSpeed;
    private bool cantTurn;

    private GameObject target = null;
    private GameManager gameManager;
    private BuildManager buildManager;
    private Animator anim;

    private Buffs[] buffs;
    private GameObject[] enemiesInRange;

    private Boolean placed = false;
    private BoxCollider2D[] colliders;

    #region Setup
    private void OnEnable()
    {
        GameManager.OnRoundStart += StartRound;
        AI.OnEnemyDeath += prioritize;
    }
    private void OnDisable()
    {
        GameManager.OnRoundStart -= StartRound;
        AI.OnEnemyDeath -= prioritize;
    }

    delegate float score(GameObject g);
    score getScore;

    private void Start()
    {
        nozzle = Instantiate(nozzle, gameObject.transform.position, Quaternion.identity, gameObject.transform);
        anim = nozzle.GetComponent<Animator>();     //Starting-up animation could be the default animation which then goes into the idle animation unconditionally
        colliders = new BoxCollider2D[1];
        colliders[0] = gameObject.GetComponent<BoxCollider2D>();
        gameObject.GetComponent<SpriteRenderer>().sprite = tower.getBaseSprite();

        gameManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<GameManager>();
        buildManager = FindObjectOfType<BuildManager>();
        buffs = new Buffs[10];
        enemiesInRange = new GameObject[27];

        if (gameManager.getGameState() == GameManager.GameState.Defend || gameManager.getGameState() == GameManager.GameState.BossRound)    //Sets a new target if created during a round. Just in case someone manages to place a tower during a round
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
                if (raycastCorners() && buildManager.approvePosition(transform.position))
                {
                    OnTowerPlaced?.Invoke();
                    nozzle.GetComponent<TurretMiddleMan>().openUpgradeMenu();
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
            gameObject.transform.position = new Vector3(Mathf.Round(mouseWorldPosition.x), Mathf.Round(mouseWorldPosition.y));

            if (gameManager.getGameState() != GameManager.GameState.Idle)
            {
                OnCancel?.Invoke();
                Destroy(gameObject);
            }
            
            return;
        }

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
        cantTurn = tower.getCantTurn();
        gameObject.GetComponent<CircleCollider2D>().enabled = true;
        if (getTowerRange() >= 0)
            gameObject.GetComponent<CircleCollider2D>().radius = getTowerRange();
        else
            gameObject.GetComponent<CircleCollider2D>().enabled = false;    //Target list gets created for infinite range in the start round function

        changePriorityFunction(Priority.Closest);

        nozzle.GetComponent<Animator>().runtimeAnimatorController = tower.getAnim();    //If run at the same time as Start, could have some bugs with this reference
    }
    public void destroyTower()
    {
        int returnScrap = getTotalSpentScrap() / 2;

        gameManager.addScrap(returnScrap);
        buildManager.removePosition(transform.position);
        Destroy(gameObject);
    }
    public void openUpgradeMenu()
    {
        OnUpgradeMenuOpen?.Invoke();
    }
    public void StartRound()    //Called when each round starts
    {
        newTarget();
    }
    #endregion

    #region Upgrade Methods
    private void upgrade(bool path)  //true for path a, false for path b
    {
        if (upgradeLevel > 3)
            return;

        if (upgradeLevel == 0)  //Upgrade 1
        {
            if (!gameManager.cost(tower.U1getCost())) { return; }

            anim.SetTrigger("UpgradeA");    //Default upgrade path
            upgradeLevel++;

            bulletScript = tower.U1getBullet();
            damageMult = tower.U1getDamageMult();
            turnSpeed = tower.U1getTurnSpeed();

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

                gameManager.spendScrap(tower.UB2getCost());
            }
        }

        gameObject.GetComponent<CircleCollider2D>().enabled = true;
        if (getTowerRange() >= 0)
            gameObject.GetComponent<CircleCollider2D>().radius = getTowerRange();
        else
            gameObject.GetComponent<CircleCollider2D>().enabled = false;    //Target list gets created for infinite range in the start round function

        OnUpgraded?.Invoke(this);
    }
    public int getTotalSpentScrap()
    {
        int returnScrap = 0;
        if (upgradeLevel >= 0)  //Unupgraded
            returnScrap += tower.getCost();
        if (upgradeLevel >= 1) //1 upgrade
            returnScrap += tower.U1getCost();
        if (upgradeLevel == 2 || upgradeLevel == 4) //A1 upgrade
            returnScrap += tower.UA1getCost();
        if (upgradeLevel == 3 || upgradeLevel == 5) //B1 upgrade
            returnScrap += tower.UB1getCost();
        if (upgradeLevel == 4)  //A2 upgrade
            returnScrap += tower.UA2getCost();
        if (upgradeLevel == 5)  //B2 upgrade
            returnScrap += tower.UB2getCost();

        return returnScrap;
    }
    #endregion

    #region Targeting/firing
    private void targetPoint()  //finds where/how to point to look at target and points that way
    {
        if (cantTurn) { return; }
        if (target == null)
        {
            prioritize();
            return;
        }

        Vector3 offset = target.transform.position - transform.position;
        Quaternion output = Quaternion.LookRotation(Vector3.forward, offset);

        nozzle.transform.rotation = Quaternion.RotateTowards(nozzle.transform.rotation, output, turnSpeed * getTurnSpeed());
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
    private void newTarget()    //Set target to closest enemy in range
    {
        /*
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        target = null;

        float distance = Mathf.Infinity;
        foreach (GameObject enemy in enemies)   //Not sure how to be more efficient than iterating through all enemies
        {
            Vector3 diff = enemy.transform.position - nozzle.transform.position;
            float curDistance = diff.magnitude;
            if (getTowerRange() > 0)
            {
                if (curDistance < distance && curDistance <= getTowerRange())   //Only pick targets in a range
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
        */

        if (getTowerRange() == -1 && enemiesInRange[0] == null)
        {
            enemiesInRange = GameObject.FindGameObjectsWithTag("Enemy");
            prioritize();
        }

        //displayList();
        target = enemiesInRange[0];
        if (target == null)
            anim.SetBool("TargetFound", false);
        else
            anim.SetBool("TargetFound", true);
    }

    #region Priority functions

    private void priorityUpdate(bool Left)
    {
        if (Left)
        {
            if (getPriority() == (Priority)0)
                changePriorityFunction(Priority.Fastest);
            else
                changePriorityFunction((Priority) ( (int)getPriority() - 1));
        }
        else
        {
            if (getPriority() == Priority.Fastest)
                changePriorityFunction(0);
            else
                changePriorityFunction((Priority)((int)getPriority() + 1));
        }
    }
    private void changePriorityFunction(Priority newPriority)
    {
        if (newPriority == Priority.Closest)
            getScore = getScoreByClosest;
        else if (newPriority == Priority.Furthest)
            getScore = getScoreByFurthest;
        else if (newPriority == Priority.Strongest)
            getScore = getScoreByStrongest;
        else
            getScore = getScoreBySpeed;
    }
    float getScoreByClosest(GameObject g)
    {
        return (g.transform.position - nozzle.transform.position).magnitude;
    }
    float getScoreByFurthest(GameObject g)
    {
        return -(g.transform.position - nozzle.transform.position).magnitude;   //Give a negative number so it's sorted backwards
    }
    float getScoreByStrongest(GameObject g)
    {
        if (g.GetComponent<AI>() == null)
            return 0;
        return g.GetComponent<AI>().getStrength();
    }
    float getScoreBySpeed(GameObject g)
    {
        if (g.GetComponent<AI>() == null)
            return 0;
        return g.GetComponent<AI>().towerGetSpeed();
    }
    public Priority getPriority()
    {
        if (getScore == getScoreByClosest)
            return Priority.Closest;
        if (getScore == getScoreByFurthest)
            return Priority.Furthest;
        if (getScore == getScoreByStrongest)
            return Priority.Strongest;
        if (getScore == getScoreBySpeed)
            return Priority.Fastest;

        return 0;
    }

    private void prioritize()  //Sorts enemies by the set priority
    {
        for (int g = 0; g < enemiesInRange.Length; g++)
        {
            if (enemiesInRange[g] != null && enemiesInRange[g].name == "AOETrigger")
            {
                Debug.Log("Forgetting " + enemiesInRange[g].name);
                enemiesInRange[g] = null;
            } 
        }

        if (isEmpty()) return;

        shiftUp();

        int decoysEnd = 0;
        if (!ignoreDecoys) //Move the decoys to the front
        {
            while (enemiesInRange[decoysEnd] != null && enemiesInRange[decoysEnd].GetComponent<AI>().getSpecialType() == Enemy.SpecialTypes.Decoy)
                decoysEnd++;

            for (int i = decoysEnd + 1; i < enemiesInRange.Length && enemiesInRange[i] != null; i++) //Move decoys to the front
            {
                if (enemiesInRange[i].GetComponent<AI>().getSpecialType() == Enemy.SpecialTypes.Decoy)
                {
                    GameObject temp = enemiesInRange[decoysEnd];
                    enemiesInRange[decoysEnd] = enemiesInRange[i];
                    enemiesInRange[i] = temp;
                    decoysEnd++;
                }
            }

            for (int i = 0; i < decoysEnd; i++) //Sort the decoys
            {
                for (int j = i + 1; j < decoysEnd; j++)
                {
                    if (getScore(enemiesInRange[j]) > getScore(enemiesInRange[i]))    //Ordered by enemy strength from strongest to weakest
                    {
                        GameObject temp = enemiesInRange[j];
                        enemiesInRange[j] = enemiesInRange[i];
                        enemiesInRange[i] = temp;
                    }
                }
            }
            
            for (int i = decoysEnd; i < enemiesInRange.Length; i++) //Sort the normal enemies
            {
                for (int j = i; i < enemiesInRange.Length && enemiesInRange[i] != null; i++)
                {
                    if (getScore(enemiesInRange[j]) > getScore(enemiesInRange[i]))    //Ordered by enemy strength from strongest to weakest
                    {
                        GameObject temp = enemiesInRange[j];
                        enemiesInRange[j] = enemiesInRange[i];
                        enemiesInRange[i] = temp;
                    }
                }
            }
        }
        else                                        //Move the decoys to the end if ignoring them
        {
            //Here, decoysEnd is repurposed to be the start of the decoys
            while (enemiesInRange[decoysEnd].GetComponent<AI>().getSpecialType() != Enemy.SpecialTypes.Decoy)
                decoysEnd++;

            for (int i = decoysEnd + 1; i < enemiesInRange.Length && enemiesInRange[i] != null; i++) //Move decoys to the front
            {
                if (enemiesInRange[i].GetComponent<AI>().getSpecialType() != Enemy.SpecialTypes.Decoy)
                {
                    GameObject temp = enemiesInRange[decoysEnd];
                    enemiesInRange[decoysEnd] = enemiesInRange[i];
                    enemiesInRange[i] = temp;
                    decoysEnd++;
                }
            }

            for (int i = 0; i < decoysEnd; i++) //Sort the decoys
            {
                for (int j = i; j < decoysEnd; j++)
                {
                    if (getScore(enemiesInRange[j]) > getScore(enemiesInRange[i]))    //Ordered by enemy strength from strongest to weakest
                    {
                        GameObject temp = enemiesInRange[j];
                        enemiesInRange[j] = enemiesInRange[i];
                        enemiesInRange[i] = temp;
                    }
                }
            }

            for (int i = decoysEnd; i < enemiesInRange.Length && enemiesInRange[i] != null; i++) //Sort the normal enemies
            {
                for (int j = i; i < enemiesInRange.Length; i++)
                {
                    if (getScore(enemiesInRange[j]) > getScore(enemiesInRange[i]))    //Ordered by enemy strength from strongest to weakest
                    {
                        GameObject temp = enemiesInRange[j];
                        enemiesInRange[j] = enemiesInRange[i];
                        enemiesInRange[i] = temp;
                    }
                }
            }
        }
    }

    private void shiftUp()
    {
        if (isEmpty() || isFull()) return;

        foreach (GameObject g in enemiesInRange)
        {
            for (int i = 0; i < enemiesInRange.Length - 1; i++)
            {
                if (enemiesInRange[i] == null)
                {
                    enemiesInRange[i] = enemiesInRange[i + 1];
                    enemiesInRange[i + 1] = null;
                }
            }
        }
    }

    private bool isEmpty()
    {
        bool output = true;
        foreach(GameObject g in enemiesInRange)
        {
            if (g != null)
                output = false;
        }
        return output;
    }
    private bool isFull()
    {
        bool output = true;
        foreach (GameObject g in enemiesInRange)
        {
            if (g == null)
                output = false;
        }
        return output;
    }

    private void AddTarget(GameObject newTarget)    //Adds an enemy to the list of enemies in range (unless it's forgotten)
    {
        if (newTarget.GetComponent<AI>().getSpecialType() == Enemy.SpecialTypes.Forgotten) return;

        for (int i = 0; i < enemiesInRange.Length; i++)
        {
            if (enemiesInRange[i] == null)
            {
                enemiesInRange[i] = newTarget;
                prioritize();
                return;
            }
        }
    }

    private void forget(GameObject enemy)
    {
        for (int i = 0; i < enemiesInRange.Length; i++)
        {
            if (enemiesInRange[i] == enemy)
            {
                enemiesInRange[i] = null;
                prioritize();
                return;
            }
        }
    }
    /*
    private void displayList()
    {
        string output = "[ ";
        foreach (GameObject g in enemiesInRange)
        {
            if (g == null)
                output += "null, ";
            else if (g.name == "AOETrigger")
                output += "Shouldn't be here, ";
            else
                output += g.GetComponent<AI>().getName() + ", ";
        }
        output += " ]";
        Debug.Log(output);
    }
    */
    #endregion
    #endregion

    #region Buff Managing
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
    #endregion

    #region Get functions
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

    private bool raycastCorners()   //Checks the corners of the tower to make sure it isn't touching the water
    {
        int layerMask = 1 << 4;

        if (Physics2D.Raycast(transform.position, Vector3.up + Vector3.right, 1.4f, layerMask).collider != null)    //Top right corner
            return false;
        if (Physics2D.Raycast(transform.position, Vector3.down + Vector3.right, 1.4f, layerMask).collider != null)  //Down right corner
            return false;
        if (Physics2D.Raycast(transform.position, Vector3.up + Vector3.left, 1.4f, layerMask).collider != null)  //Up left corner
            return false;
        if (Physics2D.Raycast(transform.position, Vector3.down + Vector3.left, 1.4f, layerMask).collider != null)  //Down left corner
            return false;

        return true;
    }
    public bool getPlaced() { return placed; }
    public int getUpgradeLevel() { return upgradeLevel; }
    public Tower getTower() { return tower; }
    public float getTowerRange()
    {
        if (upgradeLevel == 0)
            return tower.getRange();
        if (upgradeLevel == 1)
            return tower.U1getRange();
        if (upgradeLevel == 2)
            return tower.UA1getRange();
        if (upgradeLevel == 3)
            return tower.UB1getRange();
        if (upgradeLevel == 4)
            return tower.UA2getRange();
        if (upgradeLevel == 5)
            return tower.UB2getRange();
        return tower.getRange();
    }
    #endregion

    public void OnTriggerEnter2D(Collider2D collision) //If the collision is an enemy, add it to the list
    {
        if (!collision.CompareTag("Enemy")) return;
        AddTarget(collision.gameObject);
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;
        forget(collision.gameObject);
    }
}
