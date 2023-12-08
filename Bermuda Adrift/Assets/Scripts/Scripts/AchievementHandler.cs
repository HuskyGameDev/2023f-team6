using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementHandler : MonoBehaviour
{
    [SerializeField] private Player oldMan;
    private void OnEnable()
    {
        AI.OnBossDeath += unlock;
    }
    private void OnDisable()
    {
        AI.OnBossDeath -= unlock;
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
