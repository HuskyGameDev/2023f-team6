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

    private GameObject[] prefabs;
    [SerializeField] private GameObject[] set1;
    [SerializeField] private GameObject[] set2;
    [SerializeField] private GameObject[] set3;
    [SerializeField] private GameObject[] Bosses;

    private void Start()
    {
        camera = Camera.main;
        Round = 1;
        loopSpot = 1;
        prefabs = set1; //Always start with set 1
    }
    public void SpawnEnemies()
    {

        if (Round % 10 == 0)
        {
            prefabs = Bosses;
            int i = (Round - 10) / 10;
            while (i >= prefabs.Length)      //Cycle through bosses predictably (so we can put stronger bosses later)
            {
                i -= prefabs.Length;
            }

            Instantiate(prefabs[i], new Vector3(posNeg() * leftBound(), Random.Range(lowerBound(), -lowerBound())), Quaternion.identity);

            //Change the enemy set for the next 10 rounds
            i = Random.Range(0, 3);
            if (i == 0)
                prefabs = set1;
            else if (i == 1)
                prefabs = set2;
            else
                prefabs = set3;

            loopSpot = 1;
        } else
        {
            total = loopSpot * 3;
            int i = 0;
            for (; i < total / 2; i++)
            {
                Instantiate(prefabs[randomEnemy()], new Vector3(Random.Range(leftBound(), -leftBound()), posNeg() * lowerBound()), Quaternion.identity);    //Top/Bottom enemies
            }
            for (; i < total; i++)
            {
                Instantiate(prefabs[randomEnemy()], new Vector3(posNeg() * leftBound(), Random.Range(lowerBound(), -lowerBound())), Quaternion.identity);   //Left/Right Enemies
            }
        }
    }
    private int randomEnemy()
    {
        for (int i = 0; i < prefabs.Length; i++)
        {
            float random = Random.Range(0f, 1f);
            if (prefabs[i].GetComponent<AI>().getEnemy().getRarity() >= random)
                return i;
        }
        return 0;
    }
    private void EnemyDown()
    {
        total--;
        onEnemyDeath?.Invoke(total);
        if (total <= 0)
        {
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
}
