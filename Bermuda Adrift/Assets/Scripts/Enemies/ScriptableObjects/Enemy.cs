using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/Enemy", order = 1)]
public class Enemy : ScriptableObject
{
    private enum Types { Underwater, Surface, Airborne, Center };

    [SerializeField] private Types Type;

    [SerializeField] private int Health;
    [SerializeField] private int Damage;
    [SerializeField] private float Speed;
    [SerializeField] private int xp;
    [SerializeField] private int scrap;
    [SerializeField] private GameObject enemyManager;
    [SerializeField] private Animator animator;
    [SerializeField] private Sprite sprite;

    //0 doesn't spawn, 1 always spawns
    [SerializeField] private float Rarity = 1;

    public int getDamage()
    {
        return Damage;
    }
    public int getHealth()
    {
        return Health;
    }
    public float getRarity()
    {
        return Rarity;
    }
    public float getSpeed()
    {
        return Speed;
    }


    public void takeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
            enemyManager.SendMessage("EnemyDown");
    }

    public void heal(int health)   //For repairing the raft, or maybe an enemy that heals other enemies?
    {
        Health += health;
    }


}
