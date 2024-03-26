using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementTracker : MonoBehaviour, IDataPersistence
{
    [SerializeField] private Achievement[] achievements;

    int decoysKilled;
    int multiKill;
    int pirateCount;
    int pilotCount;
    int oldManCount;
    int barrierDamage;

    bool idleAchieved;
    bool noAttackBossAchieved;
    bool timedBossAchieved;

    bool noAttacksThisRound;

    float multikillLeeway = 0.25f;
    float multikillStartTime;

    float idleTime;
    float bossTime;
    float bossIdleTime;

    GameManager gameManager;

    private void OnEnable()
    {
        AI.OnUnlockEnemyDeath += unlockOldMan;
        AI.OnUnlockEnemyDeath += unlockTorpedoLauncher;
        AI.OnUnlockEnemyDeath += unlockBallista;
        AI.OnEnemyDeath += unlockLightningRod;

        IslandManager.islandDiscovered += unlockPilot;

        TowerAI.OnUpgraded += unlockTotemPole;

        BuildManager.onRaftFull += unlockScrapTurret;
        BuildManager.onBlueprintAdded += unlockShieldGenerator;

        GameManager.onRoundEnd += resetNoAttackTracker;
    }
    private void OnDisable()
    {

    }


    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    private void Update()
    {
        if (!idleAchieved)  //Machine Gun
        {
            if (FindObjectOfType<Movement>().isIdle())
            {
                idleTime += Time.deltaTime;

                if (idleTime >= (5 * 60))
                    unlockMachineGun();
            }
            else
            {
                idleTime = 0;
            }
        }

        if (!noAttackBossAchieved)  //Tri Cannon
        {
            if (gameManager.getGameState() == GameManager.GameState.BossRound)
            {
                if (!FindObjectOfType<Attack>().idleAttacks())
                {
                    noAttacksThisRound = false;
                }
            }
        }

        if (!timedBossAchieved) //Reinforced Barricade
        {
            if (FindObjectOfType<Movement>().isIdle())
            {
                bossTime += Time.deltaTime;

                if (bossTime <= 10)
                    unlockReinforcedBarricade();
            }
        }
    }

    void resetNoAttackTracker()
    {
        if (noAttacksThisRound)
            unlockTriCannon();

        bossTime = 0;
    }

    #region Tracking

    #region Characters
    void unlockOldMan(Enemy enemy) 
    {
        //Kill the Shadow Self
        if (unlockFromEnemyDeath(enemy, "Shadow Self"))
            AI.OnUnlockEnemyDeath -= unlockOldMan;
    }
    void unlockPilot(Island island) 
    {
        //Find an island
        unlockAchievement(getAchievment("Land Ho!"));

        IslandManager.islandDiscovered -= unlockPilot;
    }
    #endregion

    #region Towers
    void unlockTorpedoLauncher(Enemy enemy) 
    {
        //Kill a shark
        if (unlockFromEnemyDeath(enemy, "Shark"))
            AI.OnUnlockEnemyDeath -= unlockTorpedoLauncher;
    }
    void unlockBallista(Enemy enemy) 
    {
        //Kill 20 decoy enemies
        if (enemy.getName().CompareTo("Hamster") == 0)
            decoysKilled++;

        if (decoysKilled >= 20)
        {
            unlockAchievement(getAchievment("Gotcha!"));
            AI.OnUnlockEnemyDeath -= unlockBallista;
        }
    }
    void unlockScrapTurret() 
    {
        //Fill the raft with towers
        unlockAchievement(getAchievment("Full Arsenal"));

        BuildManager.onRaftFull -= unlockScrapTurret;
    }
    void unlockTotemPole(TowerAI towerAI)
    {
        //Fully upgrade a tower
        if (towerAI.getUpgradeLevel() > 3)
            unlockAchievement(getAchievment("Full Arsenal"));

        TowerAI.OnUpgraded -= unlockTotemPole;
    }
    void unlockShieldGenerator(int count)
    {
        //Own 10 blueprints
        if (count >= 10)
        {
            unlockAchievement(getAchievment("Why are you hoarding?"));
            BuildManager.onBlueprintAdded -= unlockShieldGenerator;
        }
    }
    void unlockLightningRod()
    {
        //Kill 10 enemies in 1 hit
        if (multikillStartTime + multikillLeeway < Time.time)
        {
            multiKill = 0;
            multikillStartTime = Time.time;
        }

        multiKill++;

        if (multiKill >= 5)
        {
            unlockAchievement(getAchievment("Unstoppable Force"));
            AI.OnEnemyDeath -= unlockLightningRod;
        }
    }
    void unlockMachineGun()
    {
        //Be idle for 5 minutes
        unlockAchievement(getAchievment("Tea Time"));

        idleAchieved = true;
    }
    void unlockTriCannon()
    {
        //Beat a boss without attacking
    }
    #endregion

    #region Barricades
    void unlockReinforcedBarricade()
    {
        //Kill a boss in 10 seconds
    }
    void unlockExplosiveBarricade()
    {
        //Kill a boss with a melee attack
    }
    void unlockFishNets()
    {
        //Unlock a skill
    }
    void unlockFloatingMine()
    {
        //Do 1,000 damage in 1 hit
    }
    void unlockPlatformBarricade()
    {
        //Collect 1,000,000 scrap at once
    }
    #endregion

    #region Centerpieces
    void unlockReinforcedCenterpiece()
    {
        //Beat the final boss
    }
    void unlockBarrierCenterpiece()
    {
        //Death island
    }
    void unlockRegeneratingCenterpiece()
    {
        //Die on the first round
    }
    #endregion

    #region Extras
    void ImmovableObject()
    {
        //Fully upgrade every tower on both paths (in 1 run?)
    }
    void DevoutFollower()
    {
        //Pray to a cthulu statue
    }
    void Pirate1000() { }
    void Pilot1000() { }
    void OldMan1000() { }
    void Round100() { }
    void FullSkillTree() { }
    void BarrierDamage()
    {
        //Block 100,000 damage with barriers
    }
    void SingleMinded()
    {
        //Fill the raft with 1 type of tower
    }
    void FullLogbook() { }
    #endregion


    #endregion

    #region Unlocking
    bool unlockFromEnemyDeath(Enemy enemy, string enemyName)
    {
        if (enemy.getName().CompareTo(enemyName) != 0)
            return false;

        unlockAchievement(getAchievment(name));
        return true;
    }

    Achievement getAchievment(string name)
    {
        foreach (Achievement ach in achievements)
        {
            if (ach.getName().CompareTo(name) == 0)
                return ach;
        }
        Debug.Log("No achievement named " + name);
        return null;
    }
    void unlockAchievementSilent(Achievement toBeUnlocked)   //No popups, for loading
    {
        if (toBeUnlocked.getAssociatedTower() != null)
            toBeUnlocked.getAssociatedTower().setUnlocked(true);
        else if (toBeUnlocked.getAssociatedBarrier() != null)
            toBeUnlocked.getAssociatedBarrier().setUnlock(true);
        else if (toBeUnlocked.getAssociatedCharacter() != null)
            toBeUnlocked.getAssociatedCharacter().setUnlock(true);
    }
    void unlockAchievement(Achievement toBeUnlocked) { FindObjectOfType<AchievementHandler>().SendMessage("unlockAchievement", toBeUnlocked); }
    #endregion

    #region Save system
    public void LoadData(S_O_Saving saver)
    {
        foreach (Achievement ach in achievements)
        {
            ach.setUnlocked(saver.getAchievementSave(ach.name).unlocked);

            //Load running counts
            //Remove event triggers
        }
    }
    public void SaveData(S_O_Saving saver)
    {
        foreach (Achievement ach in achievements)
        {
            saver.getAchievementSave(ach.name).unlocked = ach.getUnlocked();

            //Save running counts
        }
    }
    #endregion
}
