using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Centerpiece : MonoBehaviour
{
    [SerializeField] private int Health;
    [SerializeField] private GameObject manager;

    void CenterDamage(int damage)
    {
        if (Health > 0)
        {
            Health -= damage;
        }

        if (Health <= 0)
        {
            manager.SendMessage("GameEnd");
        }
    }

    public int getHealth()
    {
        return Health;
    }

    public void setHealth(int health)
    {
        Health = health;
    }
}
