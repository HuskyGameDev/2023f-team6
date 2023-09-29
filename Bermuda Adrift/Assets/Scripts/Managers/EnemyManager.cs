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

    [SerializeField] private GameObject[] prefabs;

    private void Start()
    {
        camera = Camera.main;
        Round = 1;
    }
    public void SpawnEnemies()
    {
        total = Random.Range(2 * Round, 3 * Round);
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
