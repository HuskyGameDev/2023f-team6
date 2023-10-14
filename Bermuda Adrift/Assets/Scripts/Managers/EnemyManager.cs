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

    private Camera camera;  //Not sure what the warning is, but when I changed the name it broke several things
    private int total;
    private int Round;
    private int loopSpot;

    [SerializeField] private GameObject prefab;

    private Enemy[] enemySet;   //The set that's actually used to assign enemies
    [SerializeField] private Enemy[] set1;      //The sets that contain enemies that can be assigned
    [SerializeField] private Enemy[] set2;
    [SerializeField] private Enemy[] set3;
    [SerializeField] private Enemy[] minions;
    [SerializeField] private Enemy[] Bosses;

    private void Start()
    {
        camera = Camera.main;
        Round = 1;
        loopSpot = 1;
        enemySet = set1; //Always start with set 1
    }
   
    public void SpawnEnemies()  //Spawns all the enemies at the start of the round
    {

        if (Round % 10 == 0)    //Checks if it's a boss round currently. Maybe check the game state or loopCount instead to be a bit more efficient?
        {
            total = 1;  //Only spawns 1 boss. Maybe on later rounds we can spawn multiple
            enemySet = Bosses;

            //int i = (Round - 10) / 10;    //If we wanted to go in a specific order through the bosses

            var boss = Instantiate(prefab, new Vector3(posNeg() * leftBound(), Random.Range(lowerBound(), -lowerBound())), Quaternion.identity);    //Creates boss enemy on left or right of the screen

            boss.SendMessage("setEnemy", enemySet[randomEnemy()]);  //Sets the boss to a random one in the set

            boss.SendMessage("setMinion", minions[randomEnemy() % minions.Length]);

            //Change the enemy set for the next 10 rounds
            int i = Random.Range(0, 3);
            if (i == 0)
                enemySet = set1;
            else if (i == 1)
                enemySet = set2;
            else
                enemySet = set3;

            loopSpot = 1;   //Reset the loopSpot to 1, which resets the number of enemies spawning each round
        } 
        
        else
        {
            total = loopSpot * 3;   //Very basic scaling of enemy difficulty. Maybe use an array or something to keep track of how many times each set is chosen and add scaling that way?
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

        onEnemySpawn?.Invoke(total);
    }
    private int randomEnemy()   //Chooses a random enemy from the enemySet based on the rarity. Actual rarity is slightly different than the rarity in the enemy file
    {
        for (int i = 0; i < enemySet.Length; i++)
        {
            float random = Random.Range(0f, 1f);
            if (enemySet[i].getRarity() >= random)
                return i;
        }
        return 0;   //If none are chosen, pick the first enemy in the enemySet
    }
    private void EnemyDown()    //Updates the total enemies and ends the round if there are none left
    {
        total--;
        onEnemyDeath?.Invoke(total);    // Event that triggers when enemy dies
        if (total <= 0)     //Doesn't actually count the enemies on screen. Enemies should be getting tracked with the total in SpawnEnemies or added with newEnemies if they are spawned another way
        {
            Debug.Log("End round");
            gameObject.SendMessage("endRound");
            Round++;
            loopSpot++;
            onRoundEnd?.Invoke(Round);
        }
        //Add Score points
    }
    
    private int posNeg()    //Randomly returns either -1 or 1
    {
        int i = (int)Random.Range(0, 2);
        if (i == 0)
            return -1;
        else
            return 1;
    }
    private float lowerBound()  //Returns the y coordinate of the lower bound of where the enemies spawn
    {
        return (camera.transform.position.y - camera.orthographicSize) * 1.15f;   //Uses camera orthographic size to calculate "off-screen" no matter how zoomed in the camera is
    }
    private float leftBound()   //Returns the x coordinate of the left bound of where the enemies spawn
    {
        return (camera.transform.position.x - camera.orthographicSize) * 2f;    //Uses camera orthographic size to calculate "off-screen" no matter how zoomed in the camera is. Has more of a multiplier 
    }
    public int getRound()   //Returns round number
    {
        return Round;
    }
    public void newEnemies() { total++; }   //Updates the enemy count when spawning enemies in a way other than SpawnEnemies
}
