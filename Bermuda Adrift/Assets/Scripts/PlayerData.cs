using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class holds all data needed for a singular round
 * of Bermuda: Adrift. An object of this class has the data
 * for round number, center piece health, xp, scrap, level,
 * blueprints, and towers placed.
 */

[System.Serializable]
public class PlayerData {
    
    private int round;
    private int centerHealth;
    private int xp;
    private int scrap;
    private int total;
    private int level;
    private int character;
    private int layout;
    private int barrier1;
    private int barrier2;
    private int centerType;
    private string charName;
    List<SettingsTracker> rafts;
    List<Player> player;
    List<PlaceableData> towers;
    List<string> saveStrings;
    List<float> locations;
    List<int> upgrades;
    List<Tower> placeables;
    List<string> blueprintStrings;

    public PlayerData(EnemyManager enemies, Centerpiece center, GameManager game, BuildManager build, Attack attack){
        towers = build.GetPlaceableDatas();
        placeables = build.getPlaceables();
        saveStrings = new List<string>();
        locations = new List<float>();
        upgrades = new List<int>();
        player = new List<Player>();
        rafts = new List<SettingsTracker>();
        blueprintStrings = new List<string>();
        player.Add(attack.getCharacter());
        rafts.Add(GameObject.FindObjectOfType<SettingsTracker>());

        foreach(SettingsTracker track in rafts)
        {
            if(track == null)
            {
                break;
            }
            layout = track.getRaft();
            barrier1 = track.getBarrier1Num();
            barrier2 = track.getBarrier2Num();
            centerType = track.getCenterNum();
        }
        rafts = null;

        foreach(Player p in player)
        {
            if(p != null)
            {
                charName = p.getName();
            }
        }
        
        if(charName == "The Pirate")
        {
            character = 0;
        }
        else if(charName == "The Pilot")
        {
            character = 1;
        }
        else { character = 2;}
        player = null;

        foreach (PlaceableData pd in towers)
        {
            saveStrings.Add(pd.getSaveString());
            locations.Add(pd.getLocation().x);
            locations.Add(pd.getLocation().y);
            locations.Add(pd.getLocation().z);
            upgrades.Add(pd.getUpgradeLevel());

        }
        towers = null;

        foreach (Tower t in placeables)
        {
            blueprintStrings.Add(t.getSaveString());
        }
        placeables = null;

        round = enemies.getRound();
        total = enemies.getTotal();
        centerHealth = center.getHealth();
        xp = game.getXP();
        scrap = game.getScrap();
        level = game.getLevel();
        //towers.AddRange(build.GetPlaceableDatas());
    }

    public PlayerData(){
        round = 1;
        centerHealth = 10000; //Change to Centerpieces full health later
        xp = 0;
        scrap = 0;
    }

    public int getHealth(){
        return centerHealth;
    }

    public int getRound(){
        return round;
    }

    public int getXP(){
        return xp;
    }

    public int getScrap(){
        return scrap;
    }

    public int getTotal(){
        return total;
    }

    public int getLevel(){
        return level;
    }

    public List<PlaceableData> getTowers(){
        return towers;
    }
    public List<string> getSaveStrings() { return saveStrings; }

    public List<float> getLocations(){
        return locations;
    }

    public List<int> getUpgrades(){
        return upgrades;
    }

    public List<string> getBlueprints()
    {
        return blueprintStrings;
    }

    public int getChar()
    {
        return character;
    }

    public int getLayout()
    {
        return layout;
    }

    public int getBarrier1()
    {
        return barrier1;
    }

    public int getBarrier2()
    {
        return barrier2;
    }

    public int getCenterpiece()
    {
        return centerType;
    }
}
