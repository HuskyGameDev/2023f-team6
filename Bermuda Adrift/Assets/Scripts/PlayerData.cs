using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData {
    
    private int totalEnemies;
    private int round;
    private int centerHealth;

    public PlayerData(EnemyManager enemies, Centerpiece center){
        //totalEnemies = enemies.getTotal();
        //round = enemies.getRound();
        //centerHealth = center.getHealth();
    }

    public int getHealth(){
        return centerHealth;
    }

    public int getTotal(){
        return totalEnemies;
    }

    public int getRound(){
        return round;
    }
}
