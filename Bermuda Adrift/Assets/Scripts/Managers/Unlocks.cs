using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unlocks : MonoBehaviour
{
    public static event Action<Enemy> enemyUnlocked;

    public Enemy[] allEnemies;
    public Enemy[] allBosses;
    public Tower[] allTowers;
    public CharacterInfo[] allCharacters;
    public List<Enemy> unlockedEnemies;
    public List<Enemy> unlockedBosses;
    public List<Tower> unlockedTowers;
    public List<CharacterInfo> unlockedCharacters;
    private void OnEnable()
    {
        AI.OnUnlockEnemyDeath += unlockEnemy;
    }

    private void OnDisable()
    {
        AI.OnUnlockEnemyDeath -= unlockEnemy;
    }

    private void Awake()
    {
        allEnemies = this.GetComponent<EnemyManager>().getAllEnemies();
        allBosses = this.GetComponent<EnemyManager>().getAllBosses();

        foreach (Enemy e in allEnemies)
        {
            
        }
    }

    public void unlockEnemy(Enemy e)
    {
        if (!unlockedEnemies.Contains(e))
        {
            unlockedEnemies.Add(e);

            Debug.Log("Added Enemy: " + e.getName());

            enemyUnlocked?.Invoke(e);
        }
    }
}
