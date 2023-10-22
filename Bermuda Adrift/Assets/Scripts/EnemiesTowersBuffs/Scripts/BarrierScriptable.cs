using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "Barrier", menuName = "ScriptableObjects/Barrier", order = 0)]
public class BarrierScriptable : ScriptableObject
{
    public enum Effect { Blockade, Effect }
    [SerializeField] private Effect effect;
    [SerializeField] private int Health;
    [SerializeField] private int cost;
    [SerializeField] private Buffs debuff;
    [SerializeField] private Sprite startingSprite;
    [SerializeField] private RuntimeAnimatorController animator;

    public Effect getEffect() { return effect; }
    public int getHealth() { return Health; }
    public int getCost() { return cost; }
    public Buffs getDebuff() { return debuff; }
    public Sprite getStartingSprite() { return startingSprite; }
    public RuntimeAnimatorController getAnimator() { return animator; }
}
