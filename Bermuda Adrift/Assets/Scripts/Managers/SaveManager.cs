using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [SerializeField] EnemyManager enemies;
    [SerializeField] Centerpiece center;
    [SerializeField] GameManager game;
    [SerializeField] PlaceableDatabase pdb;
    private BuildManager build;
    private GameManager.GameState state;
    bool saved1 = false;
    bool saved2 = false;

    private void Start(){
        state = game.getGameState();
        build = FindObjectOfType<BuildManager>();
    }

    private void Update(){
        state = game.getGameState();
        if(state == GameManager.GameState.Defend){
            beginRoundSave();
        }

        /*
        if(state == GameManager.GameState.Idle){
            endRoundSave();
        }
        */

        if(Input.GetKeyDown("m")){
            LoadPlayer();
        }

        if(state == GameManager.GameState.GameOver){
            DeletePlayer();
        }

        if(Input.GetKeyDown("9")){
            DeletePlayer();
        }
    }

    private void OnEnable(){
            EnemyManager.onRoundEnd += endRoundSave;
    }

    public void beginRoundSave(){
        if(!saved1){
            saved1 = true;
            SaveSystem.savePlayer(enemies, center, game, build);
        }
        saved2 = false;
    }

    public void LoadPlayer(){
        PlayerData data = SaveSystem.loadPlayer();

        enemies.setRound(data.getRound());

        center.setHealth(data.getHealth());

        game.setScrap(data.getScrap());
        game.setXP(data.getXP());
        game.setLevel(data.getLevel());

        List<string> saveStrings = data.getSaveStrings();
        List<float> locations = data.getLocations();
        List<int> upgrades = data.getUpgrades();
        List<PlaceableData> towers = new List<PlaceableData>();
        int i = 0;
        foreach(string s in saveStrings){
            int locIndex = i*3;
            Vector3 loc = new Vector3(locations[locIndex], locations[locIndex + 1], locations[locIndex + 2]);
            PlaceableData tower = new PlaceableData(pdb.getMatchingObject(s), loc, upgrades[i]);
            towers.Add(tower);
            i++;
        }
        Debug.Log("Got this far");
        build.loadPlaceables(towers);
    }

    public void endRoundSave(int i){
        if(!saved2){
            saved2 = true;
            SaveSystem.savePlayer(enemies, center, game, build);
            Debug.Log("            Saved at end of round");
            saved1 = false;
        }
    }

    public void DeletePlayer(){
        SaveSystem.deletePlayer();
    }
}
