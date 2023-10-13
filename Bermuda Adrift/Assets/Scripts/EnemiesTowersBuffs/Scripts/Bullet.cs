using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bullet", menuName = "ScriptableObjects/Bullet", order = 2)]
public class Bullet : ScriptableObject
{
    [SerializeField] private int projectileSpeed;
    [SerializeField] private int damage;
    [SerializeField] private float AOE; //radius of the aoe from the bullet landing. 0 to only damage enemies that were directly hit
    [SerializeField] private int effect;    //Various effects detailed in Hitscan. Might copy here if the list gets long enough
    [SerializeField] private float scale;   //Size of the bullet
    [SerializeField] private int timer;
    [SerializeField] private RuntimeAnimatorController animator;
    [SerializeField] private bool FriendlyFire; //Could be used to let bullets activate other bullets' AOE effects, like shooting the barrel
    [SerializeField] private Buffs debuff;

    //Get Functions
    public int getProjectileSpeed() { return projectileSpeed; }
    public int getDamage() { return damage; }
    public float getAOE() { return AOE; }
    public int getEffect() { return effect; }
    public float getScale() { return scale; }
    public int getTimer() { return timer; }
    public RuntimeAnimatorController getAnimator() { return animator; }
    public bool getFriendlyFire() { return FriendlyFire; }
    public Buffs getDebuff() { return debuff; }
}
