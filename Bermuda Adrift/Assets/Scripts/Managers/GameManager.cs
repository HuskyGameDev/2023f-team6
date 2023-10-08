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
        if (Input.GetKeyDown("f"))
        {
            startRound();
        }
    }

    private void startRound()
    {
        if (state == GameState.Idle)
        {
            if (gameObject.GetComponent<EnemyManager>().getRound() % 10 == 0)
                state = GameState.BossRound;
            else
                state = GameState.Defend;

            gameObject.SendMessage("SpawnEnemies");

            //Broadcast "StartRound" to all towers
            GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
            foreach (GameObject tower in towers)
            {
                tower.SendMessage("StartRound");
            }
        } else
            Debug.Log("Not idling");
    }

    private void endRound()
    {
        //Enable next-round button
        //Or wait a certain amount of time between rounds
        state = GameState.Idle;
    }

    public GameState getGameState()
    {
        return state;
    }

    public void addScrap(int newScrap) { scrap += newScrap; Debug.Log("Scrap is now " + scrap); }
    public void addXP(int newXP) { XP += newXP; Debug.Log("XP is now " + XP); }

    public bool cost(int cost)
    {
        if (cost < scrap)
        {
            scrap -= cost;
            return true;
        }
        return false;
    }
}
