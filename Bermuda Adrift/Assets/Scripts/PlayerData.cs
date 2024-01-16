using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData {
    
    private int round;
    private int centerHealth;
    private int xp;
    private int scrap;
    private int total;
    private int level;
    List<PlaceableData> towers;
    List<string> saveStrings;
    List<float> locations;
    List<int> upgrades;

    public PlayerData(EnemyManager enemies, Centerpiece center, GameManager game, BuildManager build){
        towers = build.GetPlaceableDatas();
        saveStrings = new List<string>();
        locations = new List<float>();
        upgrades = new List<int>();

        foreach (PlaceableData pd in towers)
        {
            Debug.Log(pd.getSaveString());
            saveStrings.Add(pd.getSaveString());
            locations.Add(pd.getLocation().x);
            locations.Add(pd.getLocation().y);
            locations.Add(pd.getLocation().z);
            upgrades.Add(pd.getUpgradeLevel());

        }
        towers = null;

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
}
