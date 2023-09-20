using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    private enum Types {Underwater, Surface, Airborne, Center};

    [SerializeField]
    private Types Type;

    [SerializeField]
    private int Health;
    [SerializeField]
    private int Damage;
    [SerializeField]
    private float Speed;

    //For when we add more enemies
    //At start of round, decide what type of enemy and then specific enemies are randomized
    //0 doesn't spawn, 1 always spawns
    private float Rarity;

    private void TakeDamage (int damage)
    {
        Health -= damage;
        //Mini Healthbar?
        if (Health <= 0)
            death();
    }

    void CenterDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
            SendMessage("Game End");
    }

    private void death()
    {
        //Play death animation
        Destroy(gameObject);
    }

    internal float getSpeed()
    {
        return Speed;
    }
}
