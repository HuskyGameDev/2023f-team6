using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bullet", menuName = "ScriptableObjects/Bullet", order = 2)]
public class Bullet : ScriptableObject
{
    [SerializeField] private int projectileSpeed;
    [SerializeField] private int damage;
    [SerializeField] private float AOE; //radius of the aoe from the bullet landing. 0 to only damage enemies that were directly hit

    public int getProjectileSpeed() { return projectileSpeed; }

    public int getDamage() { return damage; }
}
