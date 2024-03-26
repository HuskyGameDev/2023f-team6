using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    public static event Action<int> onRoundEnd;
    public static event Action<int> onEnemyDeath;
    public static event Action<int> allEnemiesSpawned;

    new private Camera camera;  //Not sure what the warning is, but when I changed the name it broke several things
    private int total;
    private int Round;
    private int loopSpot;

    [SerializeField] private GameObject prefab;

    [SerializeField] private Enemy[] allEnemies;
    [SerializeField] private Enemy[] Bosses;
    [SerializeField] private Enemy theMaestro;

    private List<Enemy> enemySet;

    private float eliteChance;

    private void Start()
    {
        camera = Camera.main;
        Round = 1;
        loopSpot = 1;

        enemySet = randomizedSet();
        setDifficulty();
        //displayEnemySet();
    }

    private List<Enemy> randomizedSet() //Gives a randomized list of a bunch of enemies. All the enemies' rarities add up to at least 1
    {
        List<Enemy> newSet = new List<Enemy>();

        float totalRarity = 0;
        while(totalRarity < 1)
        {
            Enemy randomEnemy = allEnemies[Random.Range(0, allEnemies.Length)];
            if (Round > 100 && Random.Range(0, 1f) < 0.1)   //After round 100, 10% chance for bosses to spawn as normal enemies
            {
                randomEnemy = Bosses[Random.Range(0, Bosses.Length)];
            }

            while (newSet.Contains(randomEnemy)) randomEnemy = allEnemies[Random.Range(0, allEnemies.Length)];

            if (Round >= randomEnemy.getRoundLimit())
            {
                newSet.Add(randomEnemy);
                totalRarity += randomEnemy.getRarity();
            }
        }

        return newSet;
    }
    private void displayEnemySet()
    {
        string output = "";
        foreach (Enemy e in enemySet)
            output += e.name + "(" + e.getRarity() + "), ";
        Debug.Log(output);
    }
    public void SpawnEnemies()  //Spawns all the enemies at the start of the round
    {

        if (Round % 10 == 0)    //Checks if it's a boss round currently. Maybe check the game state or loopCount instead to be a bit more efficient?
        {
            total = Round / 100 + 1;  //Only spawns 1 boss. After Round 100 start spawning 2, then 3 on round 200, etc

            if (Round == 100)
                total = 1;

            //int i = (Round - 10) / 10;    //If we wanted to go in a specific order through the bosses


            for (int i = 0; i < total; i++)
            {
                var boss = Instantiate(prefab, new Vector3(posNeg() * leftBound(), Random.Range(lowerBound(), -lowerBound())), Quaternion.identity);    //Creates boss enemy on left or right of the screen

                boss.SendMessage("setEnemy", Bosses[(Round / 10 - 1) % Bosses.Length]);  //Progress through the bosses set in order
            }

            loopSpot = 1;   //Reset the loopSpot to 1, which resets the number of enemies spawning each round

            enemySet = randomizedSet();
        } 
        
        else
        {
            total = (loopSpot * 3) + (Round / 10);   //Very basic scaling of enemy difficulty. Maybe use an array or something to keep track of how many times each set is chosen and add scaling that way?
            int i = 0;
            for (; i < total / 2; i++)
            {
                var tempEnemy = Instantiate(prefab, new Vector3(Random.Range(leftBound(), -leftBound()), posNeg() * lowerBound()), Quaternion.identity);    //Top/Bottom enemies
                Enemy newEnemy = enemySet[randomEnemy()];

                if (Random.Range(0, 1f) <= eliteChance)
                    tempEnemy.SendMessage("setEliteEnemy", newEnemy);
                else
                    tempEnemy.SendMessage("setEnemy", newEnemy);
            }
            for (; i < total; i++)
            {
                var tempEnemy = Instantiate(prefab, new Vector3(posNeg() * leftBound(), Random.Range(lowerBound(), -lowerBound())), Quaternion.identity);   //Left/Right Enemies
                Enemy newEnemy = enemySet[randomEnemy()];
                tempEnemy.SendMessage("setEnemy", newEnemy);

                if (Random.Range(0, 1f) <= eliteChance)
                    tempEnemy.SendMessage("setEliteEnemy", newEnemy);
            }
        }

        allEnemiesSpawned?.Invoke(total);
    }
    public void summonFinalBoss()
    {
        BroadcastMessage("finalBossRound");

        var boss = Instantiate(prefab, new Vector3(posNeg() * leftBound(), Random.Range(lowerBound(), -lowerBound())), Quaternion.identity);    //Creates boss enemy on left or right of the screen

        boss.SendMessage("setEnemy", theMaestro);  //Spawn final boss

        total = 1;
    }
    private int randomEnemy()   //Chooses a random enemy from the enemySet based on the rarity. Actual rarity is slightly different than the rarity in the enemy file
    {
        for (int i = 0; i < enemySet.Capacity; i++)
        {
            float random = Random.Range(0f, 1f);
            if (trueRarityCalc(i) >= random)
                return i;
        }
        return 0;   //If none are chosen, pick the first enemy in the enemySet
    }
    private float trueRarityCalc(int index) //Goal is something like 10% rarity, will do the math to make that the actual probability
    {
        if (index == 0)
            return enemySet[index].getRarity();

        float previousRarities = 1 - enemySet[0].getRarity();

        for (int i = 1; i < index; i++)
        {
            previousRarities *= trueRarityRecursive(i);
        }
        //goal = chance to get that far * true rarity

        return enemySet[index].getRarity() / previousRarities;
    }
    private float trueRarityRecursive(int index)
    {
        if (index <= 0) return 1 - enemySet[index].getRarity();

        else return 1 - (enemySet[index].getRarity() / trueRarityDenominator(index));
    }
    private float trueRarityDenominator(int index)
    {
        float output = trueRarityRecursive(0);
        for (int i = 1; i < index; i++)
        {
            output *= trueRarityRecursive(index - i);
        }
        return output;
    }
    private void EnemyDown()    //Updates the total enemies and ends the round if there are none left
    {
        total--;
        onEnemyDeath?.Invoke(total);    // Event that triggers when enemy dies
        if (total <= 0)     //Doesn't actually count the enemies on screen. Enemies should be getting tracked with the total in SpawnEnemies or added with newEnemies if they are spawned another way
        {
            //Debug.Log("End round");
            gameObject.SendMessage("endRound");
            Round++;
            loopSpot++;
            onRoundEnd?.Invoke(Round);
        }
        //Add Score points?
    }

    public void setDifficulty()
    {
        GameManager.Difficulty diff = FindObjectOfType<GameManager>().getDifficulty();

        if (diff == GameManager.Difficulty.Easy)
            eliteChance = 0;
        else if (diff == GameManager.Difficulty.Medium)
            eliteChance = 0.1f;
        else if (diff == GameManager.Difficulty.Hard)
            eliteChance = 0.2f;
    }
    
    private int posNeg()    //Randomly returns either -1 or 1
    {
        int i = Random.Range(0, 2);
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
    public int getRound()   //Returns the round number
    {
        return Round;
    }
    public void newEnemies() { total++; }   //Updates the enemy count when spawning enemies in a way other than SpawnEnemies

    public int getTotal() {
        return total;
    }
    public void setRound(int newRound) {
        Round = newRound;
    }
    public void setTotal(int newTotal) {
        total = newTotal;
    }
    public float getRoundScale()
    {
        //(Too hard) For the first 10 rounds, 2^(2 log x)
        //if (Round <= 10)
        //    return (int) Mathf.Round(Mathf.Pow(2f, 2 * Mathf.Log10(Round)));

        //1.09^x
        return Mathf.Round(Mathf.Pow(1.09f, Round));
    }
    public Enemy getUpcomingBoss()
    {
        int upcomingBossRound = (int) (Round / 10f);
            
        return Bosses[upcomingBossRound % Bosses.Length];
    }

    public Enemy[] getAllEnemies() { return allEnemies; }
    public Enemy[] getAllBosses() { return Bosses; }
}
