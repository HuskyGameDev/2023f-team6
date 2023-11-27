using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff", menuName = "ScriptableObjects/(De)buff")]
public class Buffs : ScriptableObject
{
    //Time in seconds that the buff/debuff is active
    [SerializeField] private float duration;
    [SerializeField] private string description;
    [SerializeField] private Sprite thumbnail;


    //All these variables are multipliers
    //More than 1 to increase stats
    //1 to do nothing (except DOT)
    //Between 0 and 1 to decrease stats


    //Applies only to Enemies
    [SerializeField] private bool Distracted; //Wander away from the raft or in circles
    [SerializeField] private bool Stunned;

    //Applies to the player
    [SerializeField] private float Cooldowns;
    [SerializeField] private float CritChance;

    //Can apply to Enemies and Player
    [SerializeField] private float Speed;
    [SerializeField] private float AttackSpeed;
    [SerializeField] private float underwaterMult;
    [SerializeField] private float airborneMult;

    //Applies only to towers and barriers
    [SerializeField] private float FireRate;    // < 1 for faster
    [SerializeField] private float TurnSpeed;   // < 1 for slower

    //Can apply to Enemies and barriers
    [SerializeField] private int DamageOverTime;  //If there's multiple effects on an enemy, they can stack. Probably shouldn't stack on barriers
    [SerializeField] private float DOTSpeed;    //Low for faster damage
    [SerializeField] private float percentDOT;
    [SerializeField] private Color DOTColor;

    [SerializeField] private float Health;  //Probably best used as a health buff
    [SerializeField] private float armor;   //Take more damage if > 1, less if < 1

    //Can apply to Player and Towers/barriers


    //Can apply to all
    [SerializeField] private float Damage;  //Weakness or a damage buff



    public float getDuration() { return duration; }
    public string getDescription() { return description; }
    public Sprite getThumbnail() { return thumbnail; }
    public bool getDistracted() { return Distracted; }
    public bool getStunned() { return Stunned; }
    public float getCooldowns() { return Cooldowns; }
    public float getCritChance() { return CritChance; }
    public float getSpeed() { return Speed; }
    public float getAttackSpeed() { return AttackSpeed; }
    public float getUnderwaterMult() { return underwaterMult; }
    public float getAirborneMult() { return airborneMult; }
    public float getFireRate() { return FireRate; }
    public float getTurnSpeed() { return TurnSpeed; }
    public int getDOT() { return DamageOverTime; }
    public float getDOTSpeed() { return DOTSpeed; }
    public float getPercentDOT() { return percentDOT; }
    public Color getDOTColor() { return DOTColor; }
    public float getHealth() { return Health; }
    public float getDamage() { return Damage; }
    public float getArmor() { return armor; }

}
