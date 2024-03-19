using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bullet", menuName = "ScriptableObjects/Bullet")]
public class Bullet : ScriptableObject
{
    [SerializeField] private int projectileSpeed;
    [SerializeField] private int damage;
    [SerializeField] private float AOE; //radius of the aoe from the bullet landing. 0 to only damage enemies that were directly hit

    public enum Effects {None, Shrapnel, DebuffDamage, Bait, Explosion, Shotgun};

    [SerializeField] private Effects effect;    //Various effects detailed in Hitscan. Might copy here if the list gets long enough
    [SerializeField] private float scale;   //Size of the bullet
    [SerializeField] private float timer;
    [SerializeField] private RuntimeAnimatorController animator;
    [SerializeField] private bool FriendlyFire; //Used to let bullets activate other bullets' AOE effects, like shooting the barrel
    [SerializeField] private float Homing; //The higher the number, the stronger the homing
    [SerializeField] private Buffs debuff;
    [SerializeField] private int pierce;
    [SerializeField] private float AOETimer;

    [SerializeField] private float UnderwaterDamage;
    [SerializeField] private float AirborneDamage;

    [SerializeField] private Bullet[] types;    //Used for bullets that can be any random bullet in a range

    [SerializeField] private TrailRenderer bulletTrail;

    [SerializeField] private string audioString;

    public enum Restrictions { None, }

    //Get Functions
    public int getProjectileSpeed() { return projectileSpeed; }
    public int getDamage() { return damage; }
    public float getAOE() { return AOE; }
    public Effects getEffect() { return effect; }
    public float getScale() { return scale; }
    public float getTimer() { return timer; }
    public RuntimeAnimatorController getAnimator() { return animator; }
    public bool getFriendlyFire() { return FriendlyFire; }
    public float getHomingStrength() { return Homing; }
    public Buffs getDebuff() { return debuff; }
    public int getPierce() { return pierce; }
    public float getAOETimer() { return AOETimer; }
    public float getUnderwaterDamage() { return UnderwaterDamage; }
    public float getAirborneDamage() { return AirborneDamage; }
    public bool hasTypes() { return types.Length > 0; }
    public int getNumOfTypes() { return types.Length; }
    public Bullet getBulletAtIndex(int i) { if (i < types.Length) return types[i]; else return null; }
    public string getAudioString() { return audioString; }
}
