using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "Barrier", menuName = "ScriptableObjects/Barrier")]
public class BarrierScriptable : ScriptableObject
{
    [SerializeField] private string saveString;

    [SerializeField] 
    public enum Effect { Blockade, Effect }
    [SerializeField] private string barrierName;
    [SerializeField] private string description;
    [SerializeField] private bool unlocked;
    [SerializeField] private Effect effect;
    [SerializeField] private int Health;
    [SerializeField] private int cost;
    [SerializeField] private Buffs debuff;
    [SerializeField] private Sprite startingSprite;
    [SerializeField] private RuntimeAnimatorController animator;
    [SerializeField] private Sprite Thumbnail;

    public void setUnlock(bool unlocked) { this.unlocked = unlocked; }

    public string getSaveString() { return saveString; }
    public string getName() { return barrierName; }
    public string getDescription() { return description; }
    public bool getUnlocked() { return unlocked; }
    public Effect getEffect() { return effect; }
    public int getHealth() { return Health; }
    public int getCost() { return cost; }
    public Buffs getDebuff() { return debuff; }
    public Sprite getStartingSprite() { return startingSprite; }
    public RuntimeAnimatorController getAnimator() { return animator; }
    public Sprite getThumbnail() { return Thumbnail; }
}
