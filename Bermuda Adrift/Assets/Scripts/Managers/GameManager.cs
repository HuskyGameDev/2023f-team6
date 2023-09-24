using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton Setup
    public static GameManager Instance { get; private set; }

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
            state = GameState.Defend;
            gameObject.SendMessage("EnemySpawns");
        } else
            Debug.Log("Not idling");
    }

    private void endRound()
    {
        //Enable next-round button
        //Or wait a certain amount of time between trounds
        Debug.Log("End round");
        state = GameState.Idle;
    }
}
