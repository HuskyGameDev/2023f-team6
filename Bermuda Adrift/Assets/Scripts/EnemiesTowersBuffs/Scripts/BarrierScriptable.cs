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

    public Effect getEffect() { return effect; }
    public int getHealth() { return Health; }
    public Buffs getDebuff() { return debuff; }
}
