using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData {
    
    private int round;
    private int centerHealth;
    private int xp;
    private int scrap;

    public PlayerData(EnemyManager enemies, Centerpiece center, GameManager game){
        round = enemies.getRound();
        centerHealth = center.getHealth();
        xp = game.getXP();
        scrap = game.getScrap();
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
}
