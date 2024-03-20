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

    public int getHealth() { return health; }
    public int getBarrier() { return barrier; }
    public int getRegen() { return regen; }
    public Sprite getSprite() { return sprite; }
}
