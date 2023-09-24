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

    private GameObject[] prefabs;
    [SerializeField] private GameObject[] airborne;
    [SerializeField] private GameObject[] surface;
    [SerializeField] private GameObject[] underwater;

    private Vector2 spawnPoint;

    private void Start()
    {
        camera = Camera.main;
        Round = 1;
    }

    private void EnemySpawns()
    {
        float x;
        float y;
        int type;
        total = (int) Random.Range(Round * 2, Round * 3);
        onEnemySpawn?.Invoke(total);
        int i = 0;
        for (; i < total / 2; i++)     //Left and Right side enemies
        {
            type = (int)Random.Range(0, 3);     //Randomize type of enemy
            if (type == 0)
                prefabs = airborne;
            else if (type == 1)
                prefabs = surface;
            else
                prefabs = underwater;

            x = leftBound() * posNeg();
            y = Random.Range(lowerBound(), -lowerBound());  //Off-screen
            Instantiate(prefabs[randomEnemy()], new Vector2(x,y), Quaternion.identity);
        }
        for (; i < total; i++)         //Top and bottom enemies
        {
            type = (int)Random.Range(0, 3);     //Randomize type of enemy
            if (type == 0)
                prefabs = airborne;
            else if (type == 1)
                prefabs = surface;
            else
                prefabs = underwater;

            x = Random.Range(leftBound(), -leftBound());    //Off-screen
            y = lowerBound() * posNeg();
            Instantiate(prefabs[randomEnemy()], new Vector2(x, y), Quaternion.identity);
        }

    }

    private int randomEnemy()
    {
        //Pick a random enemy of a given type from the rarity
        for(int i = 0; i < prefabs.Length; i++)
        {
            if (Random.Range(0, 1) <= prefabs[i].GetComponent<EnemyBase>().getRarity())
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
}
