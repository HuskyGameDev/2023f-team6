using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Centerpiece : MonoBehaviour
{
    [SerializeField] private int Health;
    private GameObject manager;

    private void Start() { manager = GameObject.FindGameObjectWithTag("Managers"); }
    void TakeDamage(int damage)
    {
        Health -= damage;

        if (Health <= 0)
            manager.SendMessage("GameEnd");
    }

    private void InflictDebuff() { }    //probably won't do debuffs on the centerpiece, this is just to stop errors

    public int getHealth() { return Health; }

    public void setHealth(int health) { Health = health; }
}
