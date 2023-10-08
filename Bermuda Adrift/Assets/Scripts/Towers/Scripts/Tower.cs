using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tower", menuName = "ScriptableObjects/Tower", order = 2)]
public class Tower : ScriptableObject
{
    [SerializeField] private int damageMult;
    [SerializeField] private float fireRate;    //0 is no delay, 1 is 1 second between shots, and so on
    [SerializeField] private float turnSpeed;
    [SerializeField] private float range;       //Use tiles as a measure of range? -1 for infinite range
    [SerializeField] private int barrierHealth;

    [SerializeField] private int cost;
    [SerializeField] private float rarity;      //For use in the level system. Rarity of it showing up in the level up options

    public int getHealth() { return barrierHealth; }

    public float getFireRate() { return fireRate; }

    public float getTurnSpeed() { return turnSpeed; }

    public int getDamageMult() { return damageMult; }

    public float getRange() { return range; }
}
