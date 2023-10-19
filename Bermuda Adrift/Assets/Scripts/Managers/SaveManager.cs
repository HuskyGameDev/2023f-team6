using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [SerializeField] EnemyManager enemies;
    [SerializeField] Centerpiece center;
    [SerializeField] GameManager game;
    private GameManager.GameState state;

    private void Start(){
        state = game.getGameState();
    }

    private void Update(){
        state = game.getGameState();
        if(state == GameManager.GameState.Idle){
            SavePlayer();
        }

        if(Input.GetKeyDown("m")){
            LoadPlayer();
        }
    }

    public void SavePlayer(){
        PlayerData data = SaveSystem.loadPlayer();
        if(data.getRound() > 1){
            SaveSystem.savePlayer(enemies, center, game);
        }
    }

    public void LoadPlayer(){
        PlayerData data = SaveSystem.loadPlayer();

        enemies.setRound(data.getRound());

        center.setHealth(data.getHealth());

        game.setScrap(data.getScrap());
        game.setXP(data.getXP());
    }
}
