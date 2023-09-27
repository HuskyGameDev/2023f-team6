using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/Enemy", order = 1)]
public class Enemy : ScriptableObject
{
    private enum Types { Underwater, Surface, Airborne, Center };

    [SerializeField] private Types Type;

    [SerializeField] private int Health;
    [SerializeField] private int Damage;
    [SerializeField] private float Speed;
    [SerializeField] private int xp;
    [SerializeField] private int scrap;
    [SerializeField] private GameObject enemyManager;
    [SerializeField] private Animator animator;
    [SerializeField] private Sprite sprite;

    //For when we add more enemies
    //At start of round, decide what type of enemy and then specific enemies are randomized
    //0 doesn't spawn, 1 always spawns
    [SerializeField] private float Rarity = 1;
}
