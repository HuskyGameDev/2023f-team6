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

    //0 for no spawn, 1 for guaranteed spawn
    [SerializeField] private float rarity;
    
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
        return rarity;
    }
    public float getSpeed()
    {
        return Speed;
    }
}
