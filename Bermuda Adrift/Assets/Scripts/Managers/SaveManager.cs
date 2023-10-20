using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [SerializeField] EnemyManager enemies;
    [SerializeField] Centerpiece center;
    [SerializeField] GameManager game;
    private GameManager.GameState state;
    bool saved = false;

    private void Start(){
        state = game.getGameState();
    }

    private void Update(){
        state = game.getGameState();
        if(state == GameManager.GameState.Defend){
            beginRoundSave();
        }

        if(Input.GetKeyDown("m")){
            LoadPlayer();
        }

        if(Input.GetKeyDown(";")){
            DeletePlayer();
        }
    }

    private void OnEnable(){
        EnemyManager.onRoundEnd += endRoundSave;
    }

    public void beginRoundSave(){
        if(!saved){
            SaveSystem.savePlayer(enemies, center, game);
            saved = true;
        }
    }

    public void LoadPlayer(){
        PlayerData data = SaveSystem.loadPlayer();

        enemies.setRound(data.getRound());

        center.setHealth(data.getHealth());

        game.setScrap(data.getScrap());
        game.setXP(data.getXP());
    }

    public void endRoundSave(int bugFix){
        SaveSystem.savePlayer(enemies, center, game);
        Debug.Log("            Saved at end of round");
    }

    public void DeletePlayer(){
        SaveSystem.deletePlayer();
    }
}
