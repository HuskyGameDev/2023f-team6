using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Island", menuName = "ScriptableObjects/Island")]
public class Island : ScriptableObject
{
    public enum islandType { Materials, Shop, Buff, SpecialInteraction};


    [SerializeField] private islandType type;

    [SerializeField] private float rarity;  //Chance for this island to be picked to show up given that an island is showing up at all

    //Materials
    [SerializeField] private int scrapBonus;

    //Shop
    //No shop functionality yet

    //Buff
    [SerializeField] private Buffs buff;

    //Special interaction
    //Nothing here yet either


    public islandType getIslandType() { return type; }
    public float getRarity() { return rarity; }
    public int getScrapBonus() { return scrapBonus; }
    public Buffs getBuff() { return buff; }
}
