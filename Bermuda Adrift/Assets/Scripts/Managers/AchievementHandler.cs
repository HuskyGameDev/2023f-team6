using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementHandler : MonoBehaviour
{
    [SerializeField] private Player pilot;
    [SerializeField] private Player oldMan;
    private void OnEnable()
    {
        AI.OnUnlockEnemyDeath += unlock;
        IslandManager.islandDiscovered += unlock;
    }
    private void OnDisable()
    {
        AI.OnUnlockEnemyDeath -= unlock;
        IslandManager.islandDiscovered -= unlock;
    }

    void unlock(Enemy enemy) 
    {
        if (enemy.name.CompareTo("BossE_ShadowSelf") == 0)
        {
            oldMan.unlock();
            Debug.Log("Old man unlocked!");
        }
    }
    void unlock(Island island) 
    {
        if (island.name.CompareTo("I_CrashedPlane") == 0)
            pilot.unlock();
    }
}
