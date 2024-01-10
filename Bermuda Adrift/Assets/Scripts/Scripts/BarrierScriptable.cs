using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "Barrier", menuName = "ScriptableObjects/Barrier", order = 0)]
public class BarrierScriptable : ScriptableObject
{
    public enum Effect { Blockade, Effect }
    [SerializeField] private string barrierName;
    [SerializeField] private string description;
    [SerializeField] private Effect effect;
    [SerializeField] private int Health;
    [SerializeField] private int cost;
    [SerializeField] private Buffs debuff;
    [SerializeField] private Sprite startingSprite;
    [SerializeField] private RuntimeAnimatorController animator;
    [SerializeField] private Sprite Thumbnail;

    public string getName() { return barrierName; }
    public string getDescription() { return description; }
    public Effect getEffect() { return effect; }
    public int getHealth() { return Health; }
    public int getCost() { return cost; }
    public Buffs getDebuff() { return debuff; }
    public Sprite getStartingSprite() { return startingSprite; }
    public RuntimeAnimatorController getAnimator() { return animator; }
    public Sprite getThumbnail() { return Thumbnail; }
}
