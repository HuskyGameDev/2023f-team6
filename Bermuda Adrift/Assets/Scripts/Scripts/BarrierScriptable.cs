using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "Barrier", menuName = "ScriptableObjects/Barrier")]
public class BarrierScriptable : ScriptableObject
{
    [SerializeField] private string saveString;

    [SerializeField] 
    public enum Effect { Blockade, Effect, Explosion, Platform }
    [SerializeField] private string barrierName;
    [SerializeField] private string description;
    [SerializeField] private bool unlocked;
    [SerializeField] private Effect effect;
    [SerializeField] private int Health;
    [SerializeField] private int cost;
    [SerializeField] private int damage;
    [SerializeField] private Buffs debuff;
    [SerializeField] private Sprite startingSprite;
    [SerializeField] private RuntimeAnimatorController animator;
    [SerializeField] private Sprite Thumbnail;

    [Header("Skill Effect")]
    [SerializeField] private Skill relatedSkill;
    public enum skillStatEffect { None, Damage, Range, FireSpeed, Effectiveness }
    [SerializeField] private skillStatEffect statEffect;

    public void setUnlock(bool unlocked) { this.unlocked = unlocked; }

    public string getSaveString() { return saveString; }
    public string getName() { return barrierName; }
    public string getDescription() { return description; }
    public bool getUnlocked() { return unlocked; }
    public Effect getEffect() { return effect; }
    public int getHealth() { return Health; }
    public int getCost() { return cost; }
    public int getDamage() { return damage; }
    public Buffs getDebuff() { return debuff; }
    public Sprite getStartingSprite() { return startingSprite; }
    public RuntimeAnimatorController getAnimator() { return animator; }
    public Sprite getThumbnail() { return Thumbnail; }
    public Skill getSkill() { return relatedSkill; }
    public skillStatEffect getSkillEffect() { return statEffect; }
}
