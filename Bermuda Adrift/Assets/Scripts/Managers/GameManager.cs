using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    #region Singleton Setup
    public static GameManager Instance { get; private set; }

    private int XP;
    private float XPNeeded;
    private int scrap;
    private int level;

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
    public static event Action<int, float> onXPCollect;
    public static event Action onRoundEnd;
    public static event Action OnRoundStart;
    public static event Action OnGameEnd;
    public static event Action<int> OnLevelUp;

    public static event Action<string> OnBossWarning;

    public enum GameState
    {
        Idle,
        Strategize,
        Defend,
        BossRound,
        GameOver
    }

    private GameState state;
    private Player character;
   
    private void Start()
    {
        Time.timeScale = 1;
        state = GameState.Idle;

        level = 1;
        scrap = 0;
        XPNeeded = (Mathf.Pow((float)level, 1.5f) * 100.0f);
        GameObject.Find("Audio Source").GetComponent<AudioManager>().PlaySound();

        if (Screen.width >= Screen.height * 16f / 9f)
            Screen.SetResolution(Screen.width, (int)(Screen.width * 9f / 16f), true);   //Forces a 16:9 ratio. Several things don't work with different resolutions (mainly UI)
        else
            Screen.SetResolution((int)(Screen.height * 16f / 9f), Screen.height, true);
    }
    
    private void Update()
    {
        //f to start a round
        if (Input.GetKeyDown("f"))  //Replaced in the main scene, but still useful for testing in other scenes
        {
            startRound();
        }
        else if (Input.GetKeyDown("n"))
            addScrap(100);
        else if (Input.GetKeyDown("z"))
            addScrap(1000);
        else if (Input.GetKeyDown("b"))
            addXP(100);
        


        levelUp();
    }

    public void startRound()   //Sets up everything when a round starts
    {
        if (state == GameState.Idle)    //Only does anything when we're not in a round
        {
            OnRoundStart?.Invoke();
            if (gameObject.GetComponent<EnemyManager>().getRound() % 10 == 0)   //Checks if it's a boss round
                state = GameState.BossRound;
            else
                state = GameState.Defend;

            gameObject.SendMessage("SpawnEnemies");     //Sends a message to the EnemyManager

            GameObject.Find("Audio Source").GetComponent<AudioManager>().fullVolume();

        } else
            Debug.Log("Not idling");    //Maybe disable the button when we get it until the round is over?
    }

    private void endRound() //Does everything that needs doing at the end of a round (received from enemyManager)
    {
        GameObject.Find("Audio Source").GetComponent<AudioManager>().quiet();

        if (gameObject.GetComponent<EnemyManager>().getRound() % 10 == 8)   //Adds the tip right before the round ends to guarantee it'll be the one to show up
            OnBossWarning?.Invoke(gameObject.GetComponent<EnemyManager>().getUpcomingBoss().getWarning1());
        else if (gameObject.GetComponent<EnemyManager>().getRound() % 10 == 9)
        {
            OnBossWarning?.Invoke(gameObject.GetComponent<EnemyManager>().getUpcomingBoss().getWarning2());
            gameObject.GetComponent<ShaderManager>().startBossRound();
        }

        onRoundEnd?.Invoke();

        if (gameObject.GetComponent<EnemyManager>().getRound() % 10 == 0)   //Award the player with a random tower after every boss round
        {
            FindObjectOfType<LevelGUIManager>().addRandom();
            gameObject.GetComponent<ShaderManager>().endBossRound();
        }

        state = GameState.Idle;
    }

    private void GameEnd()
    {
        OnGameEnd?.Invoke();
        state = GameState.GameOver;
    }

    public GameState getGameState() { return state; }   //Returns gameState; referenced a lot

    public void addScrap(int newScrap) { scrap += newScrap; onScrapCollect?.Invoke(scrap); }   //Received from enemy AI in death()
    
    public void addXP(int newXP) { XP += newXP; onXPCollect?.Invoke(XP, XPNeeded); }     //Both don't do anything yet

    public void levelUp()
    {
        if (getXP() > XPNeeded)
        {
            setLevel(level += 1);
            OnLevelUp?.Invoke(getLevel());
            setXP(0);
            XPNeeded = (Mathf.Pow((float)level, 1.5f) * 100.0f);
        }
    }

    public bool cost(int cost)  //If you can afford it, place it, but if you can't, return false
    {
        if (cost <= scrap) { return true; }
        //Debug.Log("Not Enough Scrap!");
        return false;
    }

    public void spendScrap(int cost)
    {
        Debug.Log("Spending " + cost + " scrap");
        scrap -= cost;
        onScrapCollect?.Invoke(scrap);
    }

    public void setScrap(int newScrap) {
        scrap = newScrap;
        onScrapCollect?.Invoke(scrap);
    }

    public void setXP(int newXP) {
        XP = newXP;
        onXPCollect?.Invoke(XP, XPNeeded);
    }

    public void setLevel(int newLevel)
    {
        level = newLevel;
    }

    public int getScrap(){
        return scrap;
    }

    public int getXP(){
        return XP;
    }

    public int getLevel()
    {
        return level;
    }

    public float getLevelScale()
    {
        return (float) Math.Pow(1.3, (double) getLevel() - 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void freezeTime()
    {
        Time.timeScale = 0;
    }

    public void unfreezeTime()
    {
        Time.timeScale = 1;
    }
}
