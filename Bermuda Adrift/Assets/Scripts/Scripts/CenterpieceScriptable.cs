using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Centerpiece", menuName = "ScriptableObjects/Centerpiece")]
public class CenterpieceScriptable : ScriptableObject
{
    [SerializeField] private int health;
    [SerializeField] private int barrier;
    [SerializeField] private int regen;
    [SerializeField] private Sprite sprite;
    [SerializeField] private bool unlocked;

    public void setUnlock(bool unlocked) { this.unlocked = unlocked; }

    public int getHealth() { return health; }
    public int getBarrier() { return barrier; }
    public int getRegen() { return regen; }
    public Sprite getSprite() { return sprite; }
    public bool getUnlocked() { return unlocked; }
}
