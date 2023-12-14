using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementHandler : MonoBehaviour
{
    [SerializeField] private Player oldMan;
    private void OnEnable()
    {
        AI.OnUnlockEnemyDeath += unlock;
    }
    private void OnDisable()
    {
        AI.OnUnlockEnemyDeath -= unlock;
    }

    void unlock(Enemy enemy) 
    {
        if (enemy.name.CompareTo("BossE_ShadowSelf") == 0)
        {
            oldMan.unlock();
            Debug.Log("Old man unlocked!");
        }
    }
}
