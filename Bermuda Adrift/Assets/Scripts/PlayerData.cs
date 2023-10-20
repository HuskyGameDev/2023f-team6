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

    public PlayerData(EnemyManager enemies, Centerpiece center, GameManager game){
        round = enemies.getRound();
        total = enemies.getTotal();
        centerHealth = center.getHealth();
        xp = game.getXP();
        Debug.Log("            Saved XP: " + xp);
        scrap = game.getScrap();
        Debug.Log("            Saved Scrap: " + scrap);
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
}
