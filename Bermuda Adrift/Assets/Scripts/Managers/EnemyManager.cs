using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private Camera camera;
    private int Round;
    private enum Types { Underwater, Surface, Airborne, Center };
    [SerializeField] private GameObject prefab;
    private Vector2 spawnPoint;

    private void Start()
    {
        camera = Camera.main;
        Round = 1;
    }
    private void Update()
    {
        if (Input.GetKeyDown("f"))
        {
            EnemySpawns(Types.Airborne);
        }
    }

    private void EnemySpawns(Types type)
    {
        int total = (int) Random.Range(Round * 2, Round * 3);
        Debug.Log(total);
        int i = 0;
        for (; i < total / 2; i++)     //Left and Right side enemies
        {
            //Set prefab to be equal to random enemy of the specified type
            float x = leftBound() * posNeg();
            float y = Random.Range(lowerBound(), -lowerBound());
            Instantiate(prefab, new Vector2(x,y), Quaternion.identity);
        }
        for (; i < total; i++)         //Top and bottom enemies
        {
            //Set prefab to be equal to random enemy of the specified type
            float x = Random.Range(leftBound(), -leftBound());
            float y = lowerBound() * posNeg();
            Instantiate(prefab, new Vector2(x, y), Quaternion.identity);
        }

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
        return (camera.transform.position.x - camera.orthographicSize) * 1.5f;
    }
}
