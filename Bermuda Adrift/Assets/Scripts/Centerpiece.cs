using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Centerpiece : MonoBehaviour
{
    [SerializeField] private int Health;
    [SerializeField] private GameObject manager;

    void CenterDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
            manager.SendMessage("GameEnd");
    }
}
