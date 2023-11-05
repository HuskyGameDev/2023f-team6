using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private float damage;
    private float dmgMult;
    public enum WeaponType { Melee, Bullet }
    public WeaponType weaponType;

    public void damageMult(float mult)
    {
        dmgMult = mult;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.CompareTag("Enemy"))
            return;

        collision.SendMessage("TakeDamage", damage * dmgMult);
    }
}
