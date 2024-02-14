using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  This program is in charge of the SaveSystem program.
 *  It uses the other managers in the game scene to feed
 *  the necessary information into the SaveSystem, as 
 *  well as deciding when to save or load based on in
 *  game factors. It also translates the data from a
 *  load into the desired in game function (e.g. an int
 *  into the round number).
 */

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

        if(Input.GetKeyDown("m")){
            LoadPlayer();
        }

        if(Input.GetKeyDown("9")){
            DeletePlayer();
        }
    }

    private void OnEnable(){
        EnemyManager.onRoundEnd += endRoundSave;
        GameManager.OnRoundStart += beginRoundSave;
        GameManager.OnGameEnd += DeletePlayer;
    }
    private void OnDisable()
    {
        EnemyManager.onRoundEnd -= endRoundSave;
        GameManager.OnRoundStart -= beginRoundSave;
        GameManager.OnGameEnd -= DeletePlayer;
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
        List<Tower> blueprints = new List<Tower>();
        List<string> blueStrings = data.getBlueprints();

        int i = 0;
        foreach(string s in saveStrings){
            int locIndex = i*3;
            Vector3 loc = new Vector3(locations[locIndex], locations[locIndex + 1], locations[locIndex + 2]);
            PlaceableData tower = new PlaceableData(pdb.getMatchingObject(s), loc, upgrades[i]);
            towers.Add(tower);
            i++;
        }
        build.loadPlaceables(towers);
        
        foreach(string s in blueStrings)
        {
            //PlaceableData tower = new PlaceableData(pdb.getMatchingObject(s));
            blueprints.Add((Tower) pdb.getMatchingObject(s));
        }
        build.setPlaceables(blueprints);
    }

    public void endRoundSave(int i){
        if(!saved2){
            saved2 = true;
            SaveSystem.savePlayer(enemies, center, game, build);
            Debug.Log("Saved at end of round");
            saved1 = false;
        }
    }

    public void DeletePlayer(){
        SaveSystem.deletePlayer();
    }
}
