using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/Enemy", order = 1)]
public class Enemy : ScriptableObject
{
    public enum Types { Underwater, Airborne, WaterBoss, AirborneBoss };
    public enum Attack { Minions, Resurface, Heal, Projectile, Buff, AOEBuff, Lightning };

    [SerializeField] private Types Type;

    public enum SpecialTypes { None, Decoy, Immune }

    [SerializeField] private SpecialTypes specialType;

    [SerializeField] private string Name;
    [SerializeField] private string description;
    [SerializeField] private int pageNumber;
    [SerializeField] private int Health;
    [SerializeField] private int Damage;
    [SerializeField] private float Speed;
    [SerializeField] private float attackSpeed; //Less for faster
    [SerializeField] private Sprite sprite;

    [SerializeField] private int xp;
    [SerializeField] private int scrap;

    //0 for no spawn, 1 for guaranteed spawn
    [SerializeField] private float rarity;
    [SerializeField] private int roundLimit;    //Won't appear before the given round. Good for high-scaled enemies so you don't get flattened in the early game

    [SerializeField] private RuntimeAnimatorController anim;
    [SerializeField] private float xSize;
    [SerializeField] private float ySize;

    [SerializeField] private Buffs debuff;  //Probably won't be used much other than something like making barriers take more damage or making them take damage over time

    [SerializeField] private GameObject extra;
    [SerializeField] private Bullet bullet;
    [SerializeField] private Enemy minion;

    [SerializeField] private string bossWarning;
    [SerializeField] private string bossApproaching;

    [SerializeField] private Attack[] availableAttacks;

    [SerializeField] private float phase2switch;
    [SerializeField] private Attack[] Phase2ExtraAttacks;

    [SerializeField] private bool logged;


    public Types getType() { return Type; }
    public SpecialTypes getSpecialType() { return specialType; }
    public string getName() { return Name; }
    public int getHealth() { return Health; }
    public int getDamage() { return Damage; }
    public float getSpeed() { return Speed; }
    public float getAttackSpeed() { return attackSpeed; }

    public string getDescription() { return description; }
    public Sprite getSprite() { return sprite; }
    public int getXP() { return xp; }
    public int getScrap() { return scrap; }
    public float getRarity() { return rarity; }
    public int getRoundLimit() { return roundLimit; }
    public RuntimeAnimatorController getAnim() { return anim; }
    public float getXSize() { return xSize; }
    public float getYSize() { return ySize; }
    public Buffs getDefuff() { return debuff; }
    public GameObject getExtra() { return extra; }
    public Bullet getBullet() { return bullet; }
    public Enemy getMinion() { return minion; }
    public string getWarning1() { return bossWarning; }
    public string getWarning2() { return bossApproaching; }
    public Attack[] getAvailableAttacks() { return availableAttacks; }
    public Attack[] getPhase2Attacks() { return Phase2ExtraAttacks; }
    public float phase2TriggerHealth() { return phase2switch; }

    public bool getLogged() { return logged; }
    public void setLogged(bool logged) { this.logged = logged; }
}
