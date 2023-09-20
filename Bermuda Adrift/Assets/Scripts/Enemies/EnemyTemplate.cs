using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTemplate : MonoBehaviour
{
    public enum Types {Underwater, Surface, Airborne};
    public Types Type;

    public int Health;
    public int Damage;
    public int Speed;

    //For when we add more enemies
    //At start of round, decide what type of enemy and then specific enemies are randomized
    //0 doesn't spawn, 1 always spawns
    private float Rarity;

    private void Start()
    {
        if (Type == Types.Airborne)
        {
            //Set AI to airborne type
        } else
        {
            //Set AI to default
        }
    }

    private void TakeDamage (int damage)
    {
        Health -= damage;
        if (Health <= 0)
            death();
    }

    private void death()
    {
        Destroy(this);
    }
}
