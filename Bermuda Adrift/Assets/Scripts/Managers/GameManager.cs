using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton Setup
    public static GameManager Instance { get; private set; }

    public int XP;
    public int scrap;

    // If there is an instance, and it's not me, destroy myself.

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion Setup

    public static event Action<int> onScrapCollect;
    public static event Action<int> onXPCollect;
    public static event Action onRoundEnd;

    public enum GameState
    {
        Idle,
        Strategize,
        Defend,
        BossRound,
        GameOver
    }

    private GameState state;
   
    private void Start()
    {
        state = GameState.Idle;
    }
    
    private void Update()
    {
        //f to start a round
        if (Input.GetKeyDown("f"))  //Should probably be relaced by a UI element sometime soon
        {
            startRound();
        }
    }

    public void startRound()   //Sets up everything when a round starts
    {
        if (state == GameState.Idle)    //Only does anything when we're not in a round
        {
            if (gameObject.GetComponent<EnemyManager>().getRound() % 10 == 0)   //Checks if it's a boss round
                state = GameState.BossRound;
            else
                state = GameState.Defend;

            gameObject.SendMessage("SpawnEnemies");     //Sends a message to the EnemyManager

            //Broadcast "StartRound" to all towers
            GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
            foreach (GameObject tower in towers)    //Not sure if there's a more efficient way to do things than this
            {
                tower.SendMessage("StartRound");
            }
        } else
            Debug.Log("Not idling");    //Maybe disable the button when we get it until the round is over?
    }

    private void endRound() //Does everything that needs doing at the end of a round (received from enemyManager)
    {
        onRoundEnd?.Invoke();
        //Enable next-round button
        //Enable tower placing system
        //Both could just be affected by game state
        state = GameState.Idle;
    }

    public GameState getGameState() { return state; }   //Returns gameState; referenced a lot

    public void addScrap(int newScrap) { scrap += newScrap; onScrapCollect?.Invoke(scrap); }   //Received from enemy AI in death()
    
    public void addXP(int newXP) { XP += newXP; onXPCollect?.Invoke(XP); }     //Both don't do anything yet

    public bool cost(int cost)  //If you can afford it, place it, but if you can't, return false
    {
        if (cost < scrap)
        {
            scrap -= cost;
            return true;
        }
        return false;
    }
}
