using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    public static event Action<int> onRoundEnd;
    public static event Action<int> onEnemyDeath;
    public static event Action<int> onEnemySpawn;

    private Camera camera;
    private int total;
    private int Round;
    private int loopSpot;

    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject bossPrefab;
    private Enemy[] enemySet;
    [SerializeField] private Enemy[] set1;
    [SerializeField] private Enemy[] set2;
    [SerializeField] private Enemy[] set3;
    [SerializeField] private Enemy[] minions;
    [SerializeField] private Enemy[] Bosses;

    private void Start()
    {
        camera = Camera.main;
        Round = 10;
        loopSpot = 1;
        enemySet = set1; //Always start with set 1
    }
    public void SpawnEnemies()
    {

        if (Round % 10 == 0)
        {
            total = 1;
            enemySet = Bosses;
            int i = (Round - 10) / 10;
            i = i % enemySet.Length;

            var boss = Instantiate(bossPrefab, new Vector3(posNeg() * leftBound(), Random.Range(lowerBound(), -lowerBound())), Quaternion.identity);

            boss.SendMessage("setEnemy", enemySet[i]);

            boss.SendMessage("setMinion", minions[randomEnemy() % minions.Length]);

            //Change the enemy set for the next 10 rounds
            i = Random.Range(0, 3);
            if (i == 0)
                enemySet = set1;
            else if (i == 1)
                enemySet = set2;
            else
                enemySet = set3;

            loopSpot = 1;
        } else
        {
            total = loopSpot * 3;
            int i = 0;
            for (; i < total / 2; i++)
            {
                var tempEnemy = Instantiate(prefab, new Vector3(Random.Range(leftBound(), -leftBound()), posNeg() * lowerBound()), Quaternion.identity);    //Top/Bottom enemies
                tempEnemy.SendMessage("setEnemy", enemySet[randomEnemy()]);

            }
            for (; i < total; i++)
            {
                var tempEnemy = Instantiate(prefab, new Vector3(posNeg() * leftBound(), Random.Range(lowerBound(), -lowerBound())), Quaternion.identity);   //Left/Right Enemies
                tempEnemy.SendMessage("setEnemy", enemySet[randomEnemy()]);
            }
        }
    }
    private int randomEnemy()
    {
        for (int i = 0; i < enemySet.Length; i++)
        {
            float random = Random.Range(0f, 1f);
            if (enemySet[i].getRarity() >= random)
                return i;
        }
        return 0;
    }
    private void EnemyDown()
    {
        total--;
        //Debug.Log(total + " enemies left");
        onEnemyDeath?.Invoke(total);
        if (total <= 0)
        {
            Debug.Log("End round");
            gameObject.SendMessage("endRound");
            Round++;
            loopSpot++;
            onRoundEnd?.Invoke(Round);
        }
        //Add Score points
    }

    private int posNeg()
    {
        int i = (int)Random.Range(0, 2);
        if (i == 0)
            return -1;
        else
            return 1;
    }

    private float lowerBound()
    {
        return camera.transform.position.y - camera.orthographicSize * 1.15f;
    }

    private float leftBound()
    {
        return (camera.transform.position.x - camera.orthographicSize) * 2f;
    }

    public int getRound()
    {
        return Round;
    }

    public void newEnemies()
    {
        total ++;
    }
}
