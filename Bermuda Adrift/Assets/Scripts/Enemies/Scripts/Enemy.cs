using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/Enemy", order = 1)]
public class Enemy : ScriptableObject
{
    public enum Types { Underwater, Airborne, Boss };  //When we add more bosses, maybe we should have an underwater boss vs airborne boss types in the enum

    [SerializeField] private Types Type;

    [SerializeField] private int Health;
    [SerializeField] private int Damage;
    [SerializeField] private float Speed;
    [SerializeField] private int xp;
    [SerializeField] private int scrap;

    //0 for no spawn, 1 for guaranteed spawn
    [SerializeField] private float rarity;

    [SerializeField] private RuntimeAnimatorController anim;
    [SerializeField] private float xSize;
    [SerializeField] private float ySize;


    public Types getType() { return Type; }
    public int getHealth() { return Health; }
    public int getDamage() { return Damage; }
    public float getSpeed() { return Speed; }
    public int getXP() { return xp; }
    public int getScrap() { return scrap; }
    public float getRarity() { return rarity; }
    public RuntimeAnimatorController getAnim() { return anim; }
    public float getXSize() { return xSize; }
    public float getYSize() { return ySize; }
}
