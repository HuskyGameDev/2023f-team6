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

    bool noAttacksThisRound = false;

    float multikillLeeway = 0.25f;
    float multikillStartTime;

    float idleTime;
    float bossTime;

    GameManager gameManager;

    #region Setup
    private void OnEnable()
    {
        AI.OnUnlockEnemyDeath += unlockOldMan;
        AI.OnUnlockEnemyDeath += unlockTorpedoLauncher;
        AI.OnUnlockEnemyDeath += unlockBallista;
        AI.OnEnemyDeath += unlockLightningRod;
        AI.onMeleeKill += unlockExplosiveBarricade;
        AI.OnUnlockEnemyDeath += unlockReinforcedCenterpiece;
        AI.OnEnemyDeath += Pirate1000;
        AI.OnEnemyDeath += Pilot1000;
        AI.OnEnemyDeath += OldMan1000;
        AI.onMajorDamage += unlockFloatingMine;

        IslandManager.islandDiscovered += unlockPilot;
        IslandManager.islandDiscovered += unlockBarrierCenterpiece;
        IslandManager.islandDiscovered += DevoutFollower;

        TowerAI.OnUpgraded += unlockTotemPole;

        BuildManager.onRaftFull += unlockScrapTurret;
        BuildManager.onBlueprintAdded += unlockShieldGenerator;
        BuildManager.onFullyUpgradedAll += ImmovableObject;

        GameManager.onRoundEnd += resetNoAttackTracker;
        GameManager.onScrapCollect += unlockPlatformBarricade;
        GameManager.OnGameEnd += unlockRegeneratingCenterpiece;
        GameManager.onRoundEnd += Round100;

        Barriers.onGlobalBarrierDamaged += BarrierDamage;

        LogbookManager.logbookFull += FullLogbook;
    }
    private void OnDisable()
    {
        AI.OnUnlockEnemyDeath -= unlockOldMan;
        AI.OnUnlockEnemyDeath -= unlockTorpedoLauncher;
        AI.OnUnlockEnemyDeath -= unlockBallista;
        AI.OnEnemyDeath -= unlockLightningRod;
        AI.onMeleeKill -= unlockExplosiveBarricade;
        AI.OnUnlockEnemyDeath -= unlockReinforcedCenterpiece;
        AI.OnEnemyDeath -= Pirate1000;
        AI.OnEnemyDeath -= Pilot1000;
        AI.OnEnemyDeath -= OldMan1000;
        AI.onMajorDamage -= unlockFloatingMine;

        IslandManager.islandDiscovered -= unlockPilot;
        IslandManager.islandDiscovered -= unlockBarrierCenterpiece;
        IslandManager.islandDiscovered -= DevoutFollower;

        TowerAI.OnUpgraded -= unlockTotemPole;

        BuildManager.onRaftFull -= unlockScrapTurret;
        BuildManager.onBlueprintAdded -= unlockShieldGenerator;
        BuildManager.onFullyUpgradedAll -= ImmovableObject;

        GameManager.onRoundEnd -= resetNoAttackTracker;
        GameManager.onScrapCollect -= unlockPlatformBarricade;
        GameManager.OnGameEnd -= unlockRegeneratingCenterpiece;
        GameManager.onRoundEnd -= Round100;
        GameManager.onRoundEnd -= unlockReinforcedBarricade;

        Barriers.onGlobalBarrierDamaged -= BarrierDamage;

        LogbookManager.logbookFull -= FullLogbook;
    }
    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    private void Update()
    {
        if (!idleAchieved)  //Machine Gun
        {
            if (FindObjectOfType<Movement>() != null && FindObjectOfType<Movement>().isIdle())
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
            if (FindObjectOfType<Movement>() != null && FindObjectOfType<Movement>().isIdle())
            {
                bossTime += Time.deltaTime;
            }
        }
    }

    void resetNoAttackTracker()
    {
        if (gameManager.gameObject.GetComponent<EnemyManager>().getRound() % 10 != 0 || gameManager.gameObject.GetComponent<EnemyManager>().getRound() < 10)
            return;
        if (noAttacksThisRound)
            unlockTriCannon();

        bossTime = 0;
    }

    private void OnLevelWasLoaded(int level)
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    #endregion

    #region Tracking

    #region Characters
    void unlockOldMan(Enemy enemy) 
    {
        //Kill the Shadow Self
        if (unlockFromEnemyDeath(enemy, "Shadow Self", "The Man in the Mirror"))
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
        if (unlockFromEnemyDeath(enemy, "Shark", "Fin soup"))
            AI.OnUnlockEnemyDeath -= unlockTorpedoLauncher;
    }
    void unlockBallista(Enemy enemy) 
    {
        //Kill 20 decoy enemies
        if (enemy.getSpecialType() == Enemy.SpecialTypes.Decoy)
            decoysKilled++;

        if (decoysKilled >= 20)
        {
            unlockAchievement("Gotcha!");
            AI.OnUnlockEnemyDeath -= unlockBallista;
        }
    }
    void unlockScrapTurret() 
    {
        //Fill the raft with towers
        unlockAchievement("Full Arsenal");

        BuildManager.onRaftFull -= unlockScrapTurret;
    }
    void unlockTotemPole(TowerAI towerAI)
    {
        //Fully upgrade a tower
        if (towerAI.getUpgradeLevel() > 3)
            unlockAchievement("Even Further Beyond");

        TowerAI.OnUpgraded -= unlockTotemPole;
    }
    void unlockShieldGenerator(int count)
    {
        //Own 10 blueprints
        if (count >= 10)
        {
            unlockAchievement("Why are you hoarding?");
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
            unlockAchievement("Unstoppable Force");
            AI.OnEnemyDeath -= unlockLightningRod;
        }
    }
    void unlockMachineGun()
    {
        //Be idle for 5 minutes
        unlockAchievement("Tea Time");

        idleAchieved = true;
    }
    void unlockTriCannon()
    {
        //Beat a boss without attacking
        if (gameManager.gameObject.GetComponent<EnemyManager>().getRound() < 10) return;

        noAttackBossAchieved = true;
        unlockAchievement("Not even worth my time");
    }
    #endregion

    #region Barricades
    void unlockReinforcedBarricade()
    {
        //Kill a boss in 10 seconds
        if (gameManager.gameObject.GetComponent<EnemyManager>().getRound() % 10 != 0 || gameManager.gameObject.GetComponent<EnemyManager>().getRound() < 10)
            return;
        if (bossTime > 10)
        {
            bossTime = 0;
            return;
        }
        Debug.Log("Unlocking reinforced Barricade");
        timedBossAchieved = true;
        unlockAchievement("In and Out");
    }
    void unlockExplosiveBarricade()
    {
        //Kill a boss with a melee attack
        unlockAchievement("Swordmaster");

        AI.onMeleeKill -= unlockExplosiveBarricade;
    }

    //Can't be implemented until skills are implemented
    void unlockFishNets()
    {
        //Unlock a skill
    }
    void unlockFloatingMine()
    {
        //Do 1,000 damage in 1 hit
        unlockAchievement("Thats a lotta damage");

        AI.onMajorDamage -= unlockFloatingMine;
    }
    void unlockPlatformBarricade(int i)
    {
        //Collect 100,000 scrap at once
        if (gameManager.getScrap() >= 100000)
        {
            unlockAchievement("We're Rich!");
            GameManager.onScrapCollect -= unlockPlatformBarricade;
        }
    }
    #endregion

    #region Centerpieces
    void unlockReinforcedCenterpiece(Enemy enemy)
    {
        //Beat the final boss
        if (unlockFromEnemyDeath(enemy, "The Maestro", "Well done"))
            AI.OnUnlockEnemyDeath -= unlockReinforcedCenterpiece;
    }
    void unlockBarrierCenterpiece(Island island)
    {
        //Death island
        if (island.name.CompareTo("I_DeathIsland") == 0)
        {
            unlockAchievement("Unlucky");
            IslandManager.islandDiscovered -= unlockBarrierCenterpiece;
        }
    }
    void unlockRegeneratingCenterpiece()
    {
        //Die on the first round
        if (gameManager.gameObject.GetComponent<EnemyManager>().getRound() == 1)
        {
            unlockAchievement("Skill Issue");
            GameManager.OnGameEnd -= unlockRegeneratingCenterpiece;
        }
    }
    #endregion

    #region Extras
    void ImmovableObject()
    {
        //Fully upgrade every tower on both paths in 1 run
        unlockAchievement("Immovable Object");

        BuildManager.onFullyUpgradedAll -= ImmovableObject;
    }
    void DevoutFollower(Island island)   //Name should probably change
    {
        //Interact with an obelisk
        if (island.name.CompareTo("I_Obelisk") == 0)
        {
            unlockAchievement("Devout Follower");

            IslandManager.islandDiscovered -= DevoutFollower;
        }
    }
    void Pirate1000() 
    {
        pirateCount++;

        if (pirateCount >= 1000)
        {
            unlockAchievement("New Captain");
            AI.OnEnemyDeath -= Pirate1000;
        }
    }
    void Pilot1000()
    {
        pilotCount++;

        if (pilotCount >= 1000)
        {
            unlockAchievement("Kamikaze Director");
            AI.OnEnemyDeath -= Pilot1000;
        }
    }
    void OldMan1000()
    {
        oldManCount++;

        if (oldManCount >= 1000)
        {
            unlockAchievement("Endless Looping Memories");
            AI.OnEnemyDeath -= OldMan1000;
        }
    }
    void Round100()
    {
        if (gameManager.gameObject.GetComponent<EnemyManager>().getRound() >= 100)
        {
            unlockAchievement("Completionist");
            GameManager.OnGameEnd -= unlockRegeneratingCenterpiece;
        }
    }

    //Can't be implemented until skills are implemented
    void FullSkillTree() { }
    void BarrierDamage(int damage)
    {
        //Block 100,000 damage with barriers
        barrierDamage += damage;

        if (barrierDamage >= 100000)
        {
            unlockAchievement("Removable Object");
            Barriers.onGlobalBarrierDamaged -= BarrierDamage;
        }
    }
    void FullLogbook()
    {
        unlockAchievement("Journalist");

        LogbookManager.logbookFull -= FullLogbook;
    }
    #endregion

    #endregion

    #region Unlocking
    bool unlockFromEnemyDeath(Enemy enemy, string enemyName, string achName)
    {
        if (enemy.getName().CompareTo(enemyName) != 0)
            return false;

        unlockAchievement(getAchievment(achName));
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
        //Debug.Log(FindObjectOfType<AchievementHandler>() + ", " + toBeUnlocked + ", " + toBeUnlocked.getUnlocked() + ", silent");

        if (toBeUnlocked.getAssociatedTower() != null)
            toBeUnlocked.getAssociatedTower().setUnlocked(true);
        else if (toBeUnlocked.getAssociatedBarrier() != null)
            toBeUnlocked.getAssociatedBarrier().setUnlock(true);
        else if (toBeUnlocked.getAssociatedCharacter() != null)
            toBeUnlocked.getAssociatedCharacter().setUnlock(true);
        else if (toBeUnlocked.getAssociatedCenterpiece() != null)
            toBeUnlocked.getAssociatedCenterpiece().setUnlock(true);
    }
    void unlockAchievementSilent(string name) { unlockAchievementSilent(getAchievment(name)); }
    void unlockAchievement(Achievement toBeUnlocked) 
    {
        //Debug.Log(FindObjectOfType<AchievementHandler>() + ", " + toBeUnlocked);
        if (toBeUnlocked != null && FindObjectOfType<AchievementHandler>() != null)
            FindObjectOfType<AchievementHandler>().SendMessage("unlockAchievement", toBeUnlocked);
    }
    void unlockAchievement(string name) { unlockAchievement(getAchievment(name)); }
    #endregion

    #region Save system
    public void LoadData(S_O_Saving saver)
    {
        Locks_n_Logs lnl;

        #region Characters
        lnl = saver.Man_In_The_Mirror;
        if (lnl.unlocked)
        {
            unlockAchievementSilent("The Man in the Mirror");
            AI.OnUnlockEnemyDeath -= unlockOldMan;
        }

        lnl = saver.Land_Ho;
        if (lnl.unlocked)
        {
            unlockAchievementSilent("Land Ho!");
            IslandManager.islandDiscovered -= unlockPilot;
        }
        #endregion

        #region Towers
        lnl = saver.Fin_Soup;
        if (lnl.unlocked)
        {
            unlockAchievementSilent("Fin Soup");
            AI.OnUnlockEnemyDeath -= unlockTorpedoLauncher;
        }

        lnl = saver.Gotcha;
        if (lnl.unlocked)
        {
            unlockAchievementSilent("Gotcha!");
            AI.OnUnlockEnemyDeath -= unlockBallista;
        }
        else
            decoysKilled = saver.decoysKilled;

        lnl = saver.Full_Arsenal;
        if (lnl.unlocked)
        {
            unlockAchievementSilent("Full Arsenal");
            BuildManager.onRaftFull -= unlockScrapTurret;
        }

        lnl = saver.Even_Further_Beyond;
        if (lnl.unlocked)
        {
            unlockAchievementSilent("Even Further Beyond!");
            TowerAI.OnUpgraded -= unlockTotemPole;
        }

        lnl = saver.Why_Are_You_Hoarding;
        if (lnl.unlocked)
        {
            unlockAchievementSilent("Ideas Galore");
            BuildManager.onBlueprintAdded -= unlockShieldGenerator;
        }

        lnl = saver.Unstoppable_Force;
        if (lnl.unlocked)
        {
            unlockAchievementSilent("Unstoppable Force");
            AI.OnEnemyDeath -= unlockLightningRod;
        }

        lnl = saver.Tea_Time;
        if (lnl.unlocked)
        {
            unlockAchievementSilent("Tea Time");
            idleAchieved = true;
        }

        lnl = saver.Not_Even_Worth_The_Time;
        if (lnl.unlocked)
        {
            unlockAchievementSilent("Not Even Worth My Time");
            noAttackBossAchieved = true;
        }
        #endregion

        #region Barricades
        lnl = saver.In_And_Out;
        if (lnl.unlocked)
        {
            unlockAchievementSilent("In and Out");
            timedBossAchieved = true;
        }

        lnl = saver.Swordmaster;
        if (lnl.unlocked)
        {
            unlockAchievementSilent("Swordmaster");
            AI.onMeleeKill -= unlockExplosiveBarricade;
        }

        lnl = saver.Seedling;
        if (lnl.unlocked)
        {
            unlockAchievementSilent("Seedling");
        }

        lnl = saver.Thats_A_Lotta_Damage;
        if (lnl.unlocked)
        {
            unlockAchievementSilent("That's a Lotta Damage");
            AI.onMajorDamage -= unlockFloatingMine;
        }

        lnl = saver.Were_Rich;
        if (lnl.unlocked)
        {
            unlockAchievementSilent("We're Rich!");
            GameManager.onScrapCollect -= unlockPlatformBarricade;
        }
        #endregion

        #region Centerpieces
        lnl = saver.Well_Done;
        if (lnl.unlocked)
        {
            unlockAchievementSilent("Well Done");
            AI.OnUnlockEnemyDeath -= unlockReinforcedCenterpiece;
        }

        lnl = saver.Unlucky;
        if (lnl.unlocked)
        {
            unlockAchievementSilent("Unlucky");
            IslandManager.islandDiscovered -= unlockBarrierCenterpiece;
        }

        lnl = saver.Skill_Issue;
        if (lnl.unlocked)
        {
            unlockAchievementSilent("Skill Issue");
            GameManager.OnGameEnd -= unlockRegeneratingCenterpiece;
        }
        #endregion

        #region Extras
        lnl = saver.Immovable_Object;
        if (lnl.unlocked)
        {
            unlockAchievementSilent("Immovable Object");
            BuildManager.onFullyUpgradedAll -= ImmovableObject;
        }

        lnl = saver.Devout_Follower;
        if (lnl.unlocked)
        {
            unlockAchievementSilent("Devout Follower");
            IslandManager.islandDiscovered -= DevoutFollower;
        }

        lnl = saver.New_Captain;
        if (lnl.unlocked)
        {
            unlockAchievementSilent("New Captain");
            AI.OnEnemyDeath -= Pirate1000;
        }
        else
            pirateCount = saver.pirateCount;

        lnl = saver.Kamikaze_Director;
        if (lnl.unlocked)
        {
            unlockAchievementSilent("Kamikaze Director");
            AI.OnEnemyDeath -= Pilot1000;
        }
        else
            pilotCount = saver.pilotCount;

        lnl = saver.Looping_Memories;
        if (lnl.unlocked)
        {
            unlockAchievementSilent("Endless Looping Memories");
            AI.OnEnemyDeath -= OldMan1000;
        }
        else
            oldManCount = saver.oldManCount;

        lnl = saver.Completionist;
        if (lnl.unlocked)
        {
            unlockAchievementSilent("Completionist");
            GameManager.OnGameEnd -= unlockRegeneratingCenterpiece;
        }

        lnl = saver.World_Tree;
        if (lnl.unlocked)
        {
            unlockAchievementSilent("World Tree");
        }

        lnl = saver.Removable_Object;
        if (lnl.unlocked)
        {
            unlockAchievementSilent("Removable Object");
            Barriers.onGlobalBarrierDamaged -= BarrierDamage;
        }
        else
            barrierDamage = saver.barrierDamage;

        lnl = saver.Journalist;
        if (lnl.unlocked)
        {
            unlockAchievementSilent("Journalist");
            LogbookManager.logbookFull -= FullLogbook;
        }
        #endregion

        decoysKilled = saver.decoysKilled;
        pirateCount = saver.pirateCount;
        pilotCount = saver.pilotCount;
        oldManCount = saver.oldManCount;
        barrierDamage = saver.barrierDamage;
    }
    public void SaveData(S_O_Saving saver)
    {
        foreach (Achievement ach in achievements)
        {
            saver.getAchievementSave(ach.name).unlocked = ach.getUnlocked();
        }

        saver.decoysKilled = decoysKilled;
        saver.pirateCount = pirateCount;
        saver.pilotCount = pilotCount;
        saver.oldManCount = oldManCount;
        saver.barrierDamage = barrierDamage;
    }
    #endregion
}
