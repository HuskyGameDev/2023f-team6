using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TowerAI : MonoBehaviour
{
    public static event Action OnCancel;
    public static event Action OnTowerPlaced;
    public static event Action<GameObject> OnTowerPlacedBM; //Specifically for the build manager
    public static event Action OnUpgradeMenuOpen;
    public static event Action<TowerAI> OnUpgraded;
    public static event Action<Tower> OnDoubleUpgraded;

    public enum Priority { Closest, Furthest, Strongest, Weakest, Fastest, OnlyWater, OnlyAir, None };

    private Tower tower;
    [SerializeField] private GameObject nozzle;

    [SerializeField] private GameObject bullet;
    //[SerializeField] private Tower testingTower;
    private Bullet bulletScript;

    private int upgradeLevel = 0;
    private int lightningResistance = 0;
    private float damageMult;
    private float turnSpeed;
    private bool cantTurn;

    private GameObject target = null;
    private GameManager gameManager;
    private BuildManager buildManager;
    private Animator anim;

    private List<Buffs> buffs;
    private List<GameObject> enemiesInRange;

    private bool placed = false;
    private Priority[] extras;


    #region Setup
    private void OnEnable()
    {
        GameManager.OnRoundStart += StartRound;
        GameManager.OnRoundStart += turnOnHitboxes;
        GameManager.onRoundEnd += turnOffHitboxes;
        AI.OnEnemyDeath += prioritize;
    }
    private void OnDisable()
    {
        GameManager.OnRoundStart -= StartRound;
        GameManager.OnRoundStart -= turnOnHitboxes;
        GameManager.onRoundEnd -= turnOffHitboxes;
        AI.OnEnemyDeath -= prioritize;
    }

    delegate float score(GameObject g);
    score getScore;

    private void Start()
    {
        nozzle = Instantiate(nozzle, gameObject.transform.position, Quaternion.identity, gameObject.transform);
        anim = nozzle.GetComponent<Animator>();     //Starting-up animation could be the default animation which then goes into the idle animation unconditionally
        anim.runtimeAnimatorController = tower.getAnim();
        if (tower.getBaseSprite() == null)
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
        else
            gameObject.GetComponent<SpriteRenderer>().sprite = tower.getBaseSprite();

        gameManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<GameManager>();
        buildManager = FindObjectOfType<BuildManager>();
        buffs = new List<Buffs>();
        enemiesInRange = new List<GameObject>();

        if (gameManager.getGameState() == GameManager.GameState.Defend || gameManager.getGameState() == GameManager.GameState.BossRound)    //Sets a new target if created during a round. Just in case someone manages to place a tower during a round
        {
            rangeReset();
            newTarget();
            StartRound();
        }

        //if (tower == null) place(testingTower);
    }
    private void Update()
    {
        if (!placed)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (buildManager.approvePosition(gameObject))
                {
                    OnTowerPlaced?.Invoke();
                    OnTowerPlacedBM?.Invoke(gameObject);
                    nozzle.GetComponent<TurretMiddleMan>().openUpgradeMenu();
                    placed = true;
                    anim.Play("Setup");
                    gameManager.spendScrap(tower.getCost());
                }
                else
                {
                    AudioManager.Instance.PlaySFX("Placement Failure");
                }
            }
            else if (Input.GetMouseButtonDown(1) || Input.GetKeyDown("escape"))
            {
                OnCancel?.Invoke();
                Destroy(gameObject);
            }

            var mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0f;

            if (getDimensions() % 2 == 0)
                gameObject.transform.position = new Vector3(Mathf.Round(mouseWorldPosition.x), Mathf.Round(mouseWorldPosition.y));
            else
                gameObject.transform.position = new Vector3(Mathf.Round(mouseWorldPosition.x * 2f) / 2f, Mathf.Round(mouseWorldPosition.y * 2f) / 2f);

            if (gameManager.getGameState() != GameManager.GameState.Idle)
            {
                OnCancel?.Invoke();
                Destroy(gameObject);
            }
            
            return;
        }

        if (gameManager.getGameState() == GameManager.GameState.BossRound || gameManager.getGameState() == GameManager.GameState.Defend)
        {
            newTarget();

            if (target != null) //Turn towards target every frame
                targetPoint();

            if (getRange() != 1)
            {
                if (getTowerRange() >= 0)
                    gameObject.GetComponent<CircleCollider2D>().radius = getTowerRange() * getRange();
            }
        }
        else
            anim.SetBool("TargetFound", false);
    }
    private void placeTower() { placed = true; }

    public void place(Tower towerType)  //Setup function
    {
        tower = towerType;
        bulletScript = tower.getDefaultBullet(); //Just tell the base what tower it needs to be and it will set up everything else.

        damageMult = tower.getDamageMult();
        turnSpeed = tower.getTurnSpeed();
        cantTurn = tower.getCantTurn();
        lightningResistance = tower.getLightningResistance() + 1;

        extras = tower.getExtraPriorities();
        if (extras.Length > 0) 
        {
            sortExtras();
            changePriorityFunction(extras[0]);

            if (extras[0] == Priority.None)
                turnOnHitboxes();
        }
        else
            changePriorityFunction(Priority.Closest);

        gameObject.GetComponent<CircleCollider2D>().enabled = true;
        if (getTowerRange() >= 0)
            gameObject.GetComponent<CircleCollider2D>().radius = getTowerRange();
        else
            gameObject.GetComponent<CircleCollider2D>().enabled = false;    //Target list gets created for infinite range in the start round function

        anim = nozzle.GetComponent<Animator>();
        nozzle.GetComponent<Animator>().runtimeAnimatorController = tower.getAnim();    //If run at the same time as Start, could have some bugs with this reference
    }
    public void destroyTower()
    {
        int returnScrap = getTotalSpentScrap() / 2;

        gameManager.addScrap(returnScrap);
        buildManager.removePosition(gameObject);

        if (getPriority() == Priority.None)
            clearNearbyBuff(getBulletBuff());

        Destroy(gameObject);
    }
    public void openUpgradeMenu()
    {
        OnUpgradeMenuOpen?.Invoke();
    }
    public void StartRound()    //Called when each round starts
    {
        if (getPriority() == Priority.None)
        {
            buffNearby(getBulletBuff());
            anim.SetBool("TargetFound", true);
        }
        else
            newTarget();

        if (tower.getName().CompareTo("Shield Generator") == 0)
        {
            FindObjectOfType<Centerpiece>().SendMessage("addBarrierStrength", getTowerTurnSpeed());
            FindObjectOfType<Centerpiece>().SendMessage("addBarrierReflect", getTowerDamageMult());
        }
    }
    private void SetSize(float size)
    {
        if (size == 2)
            size = 1.5f;

        transform.localScale = Vector3.right * size + Vector3.up * size;
        nozzle.transform.localScale = Vector3.right + Vector3.up;
    }
    #endregion

    #region Upgrade Methods
    private void upgrade(bool path)  //true for path a, false for path b
    {
        if (upgradeLevel > 3)
            return;

        if (getPriority() == Priority.None)
            clearNearbyBuff(getBulletBuff());

        if (upgradeLevel == 0)  //Upgrade 1
        {
            if (!gameManager.cost(tower.U1getCost())) { return; }

            anim.Play("U1Transition");
            upgradeLevel++;

            bulletScript = tower.U1getBullet();
            damageMult = tower.U1getDamageMult();
            turnSpeed = tower.U1getTurnSpeed();
            lightningResistance = tower.U1getLightningResistance() + 1;

            gameManager.spendScrap(tower.U1getCost());

        }
        else if (upgradeLevel == 1) //Upgrade A1 or B1
        {
            if (path)   //Only spot the bool matters
            {
                if (!gameManager.cost(tower.UA1getCost())) { return; }

                anim.Play("A1Transition");
                upgradeLevel++; //Sets upgrade level to 2

                bulletScript = tower.UA1getBullet();
                damageMult = tower.UA1getDamageMult();
                turnSpeed = tower.UA1getTurnSpeed();
                lightningResistance = tower.UA1getLightningResistance() + 1;

                gameManager.spendScrap(tower.UA1getCost());
            }
            else
            {
                if (!gameManager.cost(tower.UB1getCost())) { return; }

                anim.Play("B1Transition");
                upgradeLevel += 2;  //Sets upgrade level to 3

                bulletScript = tower.UB1getBullet();
                damageMult = tower.UB1getDamageMult();
                turnSpeed = tower.UB1getTurnSpeed();
                lightningResistance = tower.UB1getLightningResistance() + 1;

                gameManager.spendScrap(tower.UB1getCost());
            }
        }
        else    //Upgrade A2 or B2
        {
            if (upgradeLevel == 2)  //Goes based off upgrade level instead of boolean so stats don't get messed up if given the wrong boolean
            {
                if (!gameManager.cost(tower.UA2getCost())) { return; }

                anim.Play("A2Transition");    //Default upgrade path
                upgradeLevel += 2;  //Fully upgraded A branch is 4

                bulletScript = tower.UA2getBullet();
                damageMult = tower.UA2getDamageMult();
                turnSpeed = tower.UA2getTurnSpeed();
                lightningResistance = tower.UA2getLightningResistance() + 1;

                tower.upgradeA2();
                if (tower.sawBothUpgrades())
                    OnDoubleUpgraded?.Invoke(tower);

                gameManager.spendScrap(tower.UA2getCost());
            }
            else
            {
                if (!gameManager.cost(tower.UB2getCost())) { return; }

                anim.Play("B2Transition");    //Default upgrade path, don't need to differentiate after the branch
                upgradeLevel += 2;  //Fully upgraded B branch is 5

                bulletScript = tower.UB2getBullet();
                damageMult = tower.UB2getDamageMult();
                turnSpeed = tower.UB2getTurnSpeed();
                lightningResistance = tower.UB2getLightningResistance() + 1;

                tower.upgradeB2();
                if (tower.sawBothUpgrades())
                    OnDoubleUpgraded?.Invoke(tower);

                gameManager.spendScrap(tower.UB2getCost());
            }
        }

        if (getPriority() == Priority.None)
            buffNearby(getBulletBuff());

        gameObject.GetComponent<CircleCollider2D>().enabled = true;
        if (getTowerRange() >= 0)
            gameObject.GetComponent<CircleCollider2D>().radius = getTowerRange();
        else
            gameObject.GetComponent<CircleCollider2D>().enabled = false;    //Target list gets created for infinite range in the start round function

        OnUpgraded?.Invoke(this);
    }
    public int getTotalSpentScrap() //Calculates the total scrap spent on placing the tower + upgrades
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
    public IEnumerator setUpgrade(int upgradeLevel)   //Sets a tower's upgrade level and makes the animations match
    {
        yield return new WaitForEndOfFrame();

        if (upgradeLevel <= 0) 
        { 
            anim.Play("Setup");
            this.upgradeLevel = 0;
        }
        if (upgradeLevel == 1) 
        {
            gameManager.addScrap(tower.U1getCost());
            this.upgradeLevel = 0;
            upgrade(true);
        }
        if (upgradeLevel == 2)
        {
            gameManager.addScrap(tower.UA1getCost());
            this.upgradeLevel = 1;
            upgrade(true);
        }
        if (upgradeLevel == 3)
        {
            gameManager.addScrap(tower.UB1getCost());
            this.upgradeLevel = 1;
            upgrade(false);
        }
        if (upgradeLevel == 4)
        {
            gameManager.addScrap(tower.UA2getCost());
            this.upgradeLevel = 2;
            upgrade(true);
        }
        if (upgradeLevel == 5)
        {
            gameManager.addScrap(tower.UB2getCost());
            this.upgradeLevel = 3;
            upgrade(true);
        }
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
        boolet.SendMessage("setCritChance", getCritChance());
    }
    private void rangeReset()
    {
        enemiesInRange.Clear();
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemiesInRange.Add(enemy);
        }
        prioritize();
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
        if (getPriority() == Priority.None) return;

        if (getTowerRange() == -1 
            //&& enemiesInRange.Count > 0 && enemiesInRange[0] == null
            )
        {
            enemiesInRange.Clear();
            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                enemiesInRange.Add(enemy);
            prioritize();
        }

        //displayList();
        if (enemiesInRange.Count <= 0 || enemiesInRange[0] == null)
        {
            target = null;
            //Debug.Log("Null target");
        }
        else if (getScore(enemiesInRange[0]) == 0)
        {
            target = null;
            //Debug.Log("Invalid Target");
        }
        else
            target = enemiesInRange[0];

        if (target == null)
            anim.SetBool("TargetFound", false);
        else
            anim.SetBool("TargetFound", true);
    }

    #region Priority functions
    private void priorityUpdate(bool Left)
    {
        if (getPriority() == Priority.None) return;

        if (Left)
        {
            if (getPriority() > Priority.Fastest)
            {
                int i = extras.Length - 1;
                for (; i > 0; i--)
                {
                    if (extras[i] > Priority.Fastest && extras[i] < getPriority())  //Should set priority to the highest priority below the current priority
                    {
                        changePriorityFunction(extras[i]);
                        break;
                    }
                }
                if (i <= 0) //If there's nothing in the extras list, it should go to Fastest
                    changePriorityFunction(Priority.Fastest);
            }
            else
            {
                if (getPriority() == (Priority)0)
                {
                    if (extras.Length > 0)
                        changePriorityFunction(extras[extras.Length - 1]);
                    else
                        changePriorityFunction(Priority.Fastest);
                }
                else
                    changePriorityFunction((Priority)((int)getPriority() - 1));
            }
        }
        else
        {
            if (getPriority() >= Priority.Fastest)  //If there are extra aiming options at the end, iterate through them until you get the next one
            {
                if (extras.Length <= 0)
                    changePriorityFunction(0);

                int i = 0;
                for (; i < extras.Length; i++)
                {
                    if (extras[i] > Priority.Fastest && extras[i] > getPriority())
                    {
                        changePriorityFunction(extras[i]);
                        break;
                    }
                }
                if (i >= extras.Length) //Should only happen if you've reached the end of the list of extras. Will break early otherwise and i won't be at the end
                    changePriorityFunction(0);
            }
            else
                changePriorityFunction((Priority)((int)getPriority() + 1));
        }
    }
    private void changePriorityFunction(Priority newPriority)
    {
        //Debug.Log(newPriority);
        if (newPriority == Priority.Closest)
            getScore = getScoreByClosest;
        else if (newPriority == Priority.Furthest)
            getScore = getScoreByFurthest;
        else if (newPriority == Priority.Strongest)
            getScore = getScoreByStrongest;
        else if (newPriority == Priority.Weakest)
            getScore = getScoreByWeakest;
        else if (newPriority == Priority.Fastest)
            getScore = getScoreBySpeed;
        else if (newPriority == Priority.OnlyWater)
            getScore = getScoreByWaterStrong;
        else if (newPriority == Priority.OnlyAir)
            getScore = getScoreByAirStrong;
        else if (newPriority == Priority.None)  //For totem pole. Makes it act differently from the other towers
            getScore = dontGetScore;
    }
    float getScoreByClosest(GameObject g)
    {
        if (g == null || g.GetComponent<AI>() == null)
        {
            return 0;
        }

        return (g.transform.position - nozzle.transform.position).magnitude;
    }
    float getScoreByFurthest(GameObject g)
    {
        if (g.GetComponent<AI>() == null)
        {
            return 0;
        }

        return -getScoreByClosest(g);   //Give a negative number so it's sorted backwards
    }
    float getScoreByStrongest(GameObject g)
    {
        if (g.GetComponent<AI>() == null)
        {
            return 0;
        }

        return g.GetComponent<AI>().getStrength();
    }
    float getScoreBySpeed(GameObject g)
    {
        if (g.GetComponent<AI>() == null)
        {
            return 0;
        }

        return g.GetComponent<AI>().towerGetSpeed();
    }
    float getScoreByWaterStrong(GameObject g)
    {
        return getScoreByStrongest(g);
    }
    float getScoreByAirStrong(GameObject g)
    {
        return getScoreByStrongest(g);
    }
    float getScoreByWeakest(GameObject g)
    {
        return -getScoreByStrongest(g);
    }
    float dontGetScore(GameObject g)
    {
        return 0;
    }
    public Priority getPriority()
    {
        if (getScore == getScoreByClosest)
            return Priority.Closest;
        if (getScore == getScoreByFurthest)
            return Priority.Furthest;
        if (getScore == getScoreByStrongest)
            return Priority.Strongest;
        if (getScore == getScoreByWeakest)
            return Priority.Weakest;
        if (getScore == getScoreBySpeed)
            return Priority.Fastest;
        if (getScore == getScoreByWaterStrong)
            return Priority.OnlyWater;
        if (getScore == getScoreByAirStrong)
            return Priority.OnlyAir;
        if (getScore == dontGetScore)
            return Priority.None;

        return 0;
    }
    private void sortExtras()   //Sorts all the extra targeting options
    {
        for (int i = 0; i < extras.Length; i++)
        {
            for (int j = i + 1; j < extras.Length; i++)
            {
                if (extras[j] < extras[i])
                {
                    Priority temp = extras[i];
                    extras[i] = extras[j];
                    extras[j] = temp;
                }
            }
        }
    }

    private void prioritize()  //Sorts enemies by the set priority
    {
        for (int i = 0; i < enemiesInRange.Count; i++)
        {
            if (enemiesInRange[i] != null && enemiesInRange[i].name.CompareTo("AOETrigger") == 0) {
                enemiesInRange.Remove(enemiesInRange[i]);
                i--;
            }
        }

        if (isEmpty()) return;

        shiftUp();

        int decoysEnd = 0;
        if (!getIgnoreDecoys()) //Move the decoys to the front
        {
            while (decoysEnd < enemiesInRange.Count && enemiesInRange[decoysEnd] != null && enemiesInRange[decoysEnd].GetComponent<AI>().getSpecialType() == Enemy.SpecialTypes.Decoy)
                decoysEnd++;

            for (int i = decoysEnd + 1; i < enemiesInRange.Count && enemiesInRange[i] != null; i++) //Move decoys to the front
            {
                if (enemiesInRange[i].GetComponent<AI>() != null && enemiesInRange[i].GetComponent<AI>().getSpecialType() == Enemy.SpecialTypes.Decoy)
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
            
            for (int i = decoysEnd; i < enemiesInRange.Count; i++) //Sort the normal enemies
            {
                for (int j = i; i < enemiesInRange.Count && enemiesInRange[i] != null; i++)
                {
                    if (enemiesInRange[j] != null && enemiesInRange[i] != null && getScore(enemiesInRange[j]) > getScore(enemiesInRange[i]))    //Ordered by enemy strength from strongest to weakest
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
            while (enemiesInRange[decoysEnd] != null && enemiesInRange[decoysEnd].GetComponent<AI>() != null && enemiesInRange[decoysEnd].GetComponent<AI>().getSpecialType() != Enemy.SpecialTypes.Decoy)
                decoysEnd++;

            for (int i = decoysEnd + 1; i < enemiesInRange.Count && enemiesInRange[i] != null; i++) //Move decoys to the front
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

            for (int i = decoysEnd; i < enemiesInRange.Count && enemiesInRange[i] != null; i++) //Sort the normal enemies
            {
                for (int j = i; i < enemiesInRange.Count; i++)
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

        //string debug = "";
        //foreach (GameObject g in enemiesInRange)
        //    debug += g.GetComponent<AI>().getName() + ", ";
        //Debug.Log(debug);
    }
    private void shiftUp()
    {
        if (isEmpty() || isFull()) return;

        for (int j = 0; j < enemiesInRange.Count; j++)
        {
            for (int i = 0; i < enemiesInRange.Count - 1; i++)
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
        enemiesInRange.Add(newTarget);
        prioritize();
    }
    private void forget(GameObject enemy)
    {
        for (int i = 0; i < enemiesInRange.Count; i++)
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

    #region Buff/Debuff Managing
    private void addBuff(Buffs debuff)    //Add a debuff to the list of debuffs
    {
        Debug.Log(tower.getName() + " receiving " + debuff.name + " debuff");
        if ((debuff.getDuration() == -1 && buffs.Contains(debuff)) || tower.getName().CompareTo("Totem Pole") == 0) return;
        Debug.Log("Not cancelled");

        buffs.Add(debuff);
    }
    private void removeBuff(Buffs debuff) //Removes a debuff from the list of debuffs currently applied
    {
        buffs.Remove(debuff);
    }
    private IEnumerator Buff(Buffs newDebuff)  //adds a debuff to the list
    {
        if (getPriority() == Priority.None) yield break;    //So totem poles don't buff themselves

        addBuff(newDebuff);

        if (newDebuff.getDuration() == -1) yield break;

        yield return new WaitForSeconds(newDebuff.getDuration());

        removeBuff(newDebuff);
    }
    private void buffNearby() { buffNearby(getBulletBuff()); }
    private void buffNearby(Buffs buff) //Applies a buff to every tower in range
    {
        Debug.Log("Buffing nearby");
        Vector3 position = transform.position;
        int towerMask = 1 << 8;

        Collider2D collider = Physics2D.Raycast(position, Vector3.up + Vector3.right, getTowerRange(), towerMask).collider;     //Top right corner
        if (collider != null) collider.transform.parent.GetComponent<TowerAI>().StartCoroutine("Buff", buff);

        collider = Physics2D.Raycast(position, Vector3.up + Vector3.left, getTowerRange(), towerMask).collider;                 //Top left corner
        if (collider != null) collider.transform.parent.GetComponent<TowerAI>().StartCoroutine("Buff", buff);

        collider = Physics2D.Raycast(position, Vector3.down + Vector3.left, getTowerRange(), towerMask).collider;               //Bottom left corner
        if (collider != null) collider.transform.parent.GetComponent<TowerAI>().StartCoroutine("Buff", buff);

        collider = Physics2D.Raycast(position, Vector3.down + Vector3.right, getTowerRange(), towerMask).collider;              //Bottom right corner
        if (collider != null) collider.transform.parent.GetComponent<TowerAI>().StartCoroutine("Buff", buff);

        collider = Physics2D.Raycast(position, Vector3.up, getTowerRange(), towerMask).collider;                                //Up
        if (collider != null) collider.transform.parent.GetComponent<TowerAI>().StartCoroutine("Buff", buff);

        collider = Physics2D.Raycast(position, Vector3.down, getTowerRange(), towerMask).collider;                              //Down
        if (collider != null) collider.transform.parent.GetComponent<TowerAI>().StartCoroutine("Buff", buff);

        collider = Physics2D.Raycast(position, Vector3.left, getTowerRange(), towerMask).collider;                              //Left
        if (collider != null) collider.transform.parent.GetComponent<TowerAI>().StartCoroutine("Buff", buff);

        collider = Physics2D.Raycast(position, Vector3.right, getTowerRange(), towerMask).collider;                             //Right
        if (collider != null) collider.transform.parent.GetComponent<TowerAI>().StartCoroutine("Buff", buff);
    }
    private void clearNearbyBuff() { clearNearbyBuff(getBulletBuff()); }
    private void clearNearbyBuff(Buffs buff)    //Removes the given buff from all towers within range
    {
        Vector3 position = transform.position;
        int towerMask = 1 << 8;

        Collider2D collider = Physics2D.Raycast(position, Vector3.up + Vector3.right, getTowerRange(), towerMask).collider;     //Top right corner
        if (collider != null) collider.transform.parent.SendMessage("removeBuff", buff);

        collider = Physics2D.Raycast(position, Vector3.up + Vector3.left, getTowerRange(), towerMask).collider;                 //Top left corner
        if (collider != null) collider.transform.parent.SendMessage("removeBuff", buff);

        collider = Physics2D.Raycast(position, Vector3.down + Vector3.left, getTowerRange(), towerMask).collider;               //Bottom left corner
        if (collider != null) collider.transform.parent.SendMessage("removeBuff", buff);

        collider = Physics2D.Raycast(position, Vector3.down + Vector3.right, getTowerRange(), towerMask).collider;              //Bottom right corner
        if (collider != null) collider.transform.parent.SendMessage("removeBuff", buff);

        collider = Physics2D.Raycast(position, Vector3.up, getTowerRange(), towerMask).collider;                                //Up
        if (collider != null) collider.transform.parent.SendMessage("removeBuff", buff);

        collider = Physics2D.Raycast(position, Vector3.down, getTowerRange(), towerMask).collider;                              //Down
        if (collider != null) collider.transform.parent.SendMessage("removeBuff", buff);

        collider = Physics2D.Raycast(position, Vector3.left, getTowerRange(), towerMask).collider;                              //Left
        if (collider != null) collider.transform.parent.SendMessage("removeBuff", buff);

        collider = Physics2D.Raycast(position, Vector3.right, getTowerRange(), towerMask).collider;                             //Right
        if (collider != null) collider.transform.parent.SendMessage("removeBuff", buff);

    }
    private void lightningStrike(float duration) { StartCoroutine(tempDisable(duration)); }
    private IEnumerator tempDisable(float duration) //Temporarily stops the tower from firing
    {
        lightningResistance--;

        if (lightningResistance > 0)
            yield break;
        
        anim.speed = 0;
        //Smoke particle effect

        yield return new WaitForSeconds(duration);

        lightningResistance = getLightningResistance() + 1;
        anim.speed = 1;
    }
    #endregion

    #region Get functions
    private float getFireRate()    //Gives total speed penalty/buff (multiplicative)
    {
        float fireRate = 1;
        for (int i = 0; i < buffs.Count && buffs[i] != null; i++)
        {
            fireRate *= buffs[i].getFireRate();
        }
        return fireRate;
    }
    private float getTurnSpeed()    //Gives total turn speed penalty/buff (multiplicative)
    {
        float turnSpeed = 1;
        for (int i = 0; i < buffs.Count && buffs[i] != null; i++)
        {
            turnSpeed *= buffs[i].getTurnSpeed();
        }
        return turnSpeed;
    }
    private float getDamage()    //Gives total damage penalty/buff (multiplicative)
    {
        float damage = 1;
        for (int i = 0; i < buffs.Count && buffs[i] != null; i++)
        {
            damage *= buffs[i].getDamage();
        }
        return damage;
    }
    private float getRange()    //Gives total damage penalty/buff (multiplicative)
    {
        if (buffs == null) return 1;

        float range = 1;
        for (int i = 0; i < buffs.Count && buffs[i] != null; i++)
        {
            range *= buffs[i].getRange();
        }
        return range;
    }
    private float getCritChance()    //Gives total damage penalty/buff (multiplicative)
    {
        float crit = 1;
        for (int i = 0; i < buffs.Count && buffs[i] != null; i++)
        {
            crit *= buffs[i].getCritChance();
        }
        return crit;
    }
    private bool getIgnoreDecoys()
    {
        bool ignoreDecoys = false;
        for (int i = 0; i < buffs.Count && buffs[i] != null; i++)
        {
            ignoreDecoys = ignoreDecoys || buffs[i].getIgnoreDecoys();
        }
        return ignoreDecoys;
    }
    public bool getPlaced() { return placed; }
    public int getUpgradeLevel() { return upgradeLevel; }
    public Tower getTower() { return tower; }
    public float getTowerRange()
    {
        if (upgradeLevel == 0)
            return tower.getRange() * getRange();
        if (upgradeLevel == 1)
            return tower.U1getRange() * getRange();
        if (upgradeLevel == 2)
            return tower.UA1getRange() * getRange();
        if (upgradeLevel == 3)
            return tower.UB1getRange() * getRange();
        if (upgradeLevel == 4)
            return tower.UA2getRange() * getRange();
        if (upgradeLevel == 5)
            return tower.UB2getRange() * getRange();
        return tower.getRange() * getRange();
    }
    private int getLightningResistance()
    {
        if (upgradeLevel == 0)
            return tower.getLightningResistance();
        if (upgradeLevel == 1)
            return tower.U1getLightningResistance();
        if (upgradeLevel == 2)
            return tower.UA1getLightningResistance();
        if (upgradeLevel == 3)
            return tower.UB1getLightningResistance();
        if (upgradeLevel == 4)
            return tower.UA2getLightningResistance();
        if (upgradeLevel == 5)
            return tower.UB2getLightningResistance();
        return tower.getLightningResistance();
    }
    public Buffs getBulletBuff()
    {
        if (upgradeLevel == 0)
            return tower.getDefaultBullet().getDebuff();
        if (upgradeLevel == 1)
            return tower.U1getBullet().getDebuff();
        if (upgradeLevel == 2)
            return tower.UA1getBullet().getDebuff();
        if (upgradeLevel == 3)
            return tower.UB1getBullet().getDebuff();
        if (upgradeLevel == 4)
            return tower.UA2getBullet().getDebuff();
        if (upgradeLevel == 5)
            return tower.UB2getBullet().getDebuff();
        return tower.getDefaultBullet().getDebuff();
    }
    public float getTowerTurnSpeed()
    {
        Debug.Log(upgradeLevel);
        if (upgradeLevel == 0)
            return tower.getTurnSpeed();
        if (upgradeLevel == 1)
            return tower.U1getTurnSpeed();
        if (upgradeLevel == 2)
            return tower.UA1getTurnSpeed();
        if (upgradeLevel == 3)
            return tower.UB1getTurnSpeed();
        if (upgradeLevel == 4)
            return tower.UA2getTurnSpeed();
        if (upgradeLevel == 5)
            return tower.UB2getTurnSpeed();
        return tower.getTurnSpeed();
    }
    public float getTowerDamageMult()
    {
        if (upgradeLevel == 0)
            return tower.getDamageMult();
        if (upgradeLevel == 1)
            return tower.U1getDamageMult();
        if (upgradeLevel == 2)
            return tower.UA1getDamageMult();
        if (upgradeLevel == 3)
            return tower.UB1getDamageMult();
        if (upgradeLevel == 4)
            return tower.UA2getDamageMult();
        if (upgradeLevel == 5)
            return tower.UB2getDamageMult();
        return tower.getDamageMult();
    }
    public int getDimensions() { return tower.getDimensions(); }
    #endregion

    private void turnOffHitboxes() { if (getPriority() != Priority.None) gameObject.GetComponent<CircleCollider2D>().enabled = false; } //Tower hitboxes mess with being able to click on other towers, so turn them off at the end of every round
    private void turnOnHitboxes() { if (getTowerRange() > 0) gameObject.GetComponent<CircleCollider2D>().enabled = true; }  //And turn them back on at the start of every round
    public void OnTriggerEnter2D(Collider2D collision) //If the collision is an enemy, add it to the list
    {
        if (!collision.CompareTag("Enemy")) return;

        //Checks for onlyWater and onlyAir done here instead of in the score functions
        if (getPriority() == Priority.OnlyWater && (collision.GetComponent<AI>().getType() == Enemy.Types.Airborne || collision.GetComponent<AI>().getType() == Enemy.Types.AirborneBoss)) return;
        if (getPriority() == Priority.OnlyAir && (collision.GetComponent<AI>().getType() == Enemy.Types.Underwater || collision.GetComponent<AI>().getType() == Enemy.Types.WaterBoss)) return;

        AddTarget(collision.gameObject);
    }
    public void OnTriggerExit2D(Collider2D collision)   //Remove an enemy from the list of targets when it exits the range
    {
        if (!collision.CompareTag("Enemy")) return;
        forget(collision.gameObject);
    }
}
