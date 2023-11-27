using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Weapon : MonoBehaviour
{
    public static event Action onCrit;

    [SerializeField] private float damage;
    private float dmgMult;
    private float underwaterMult;
    private float airborneMult;
    private float critChance;
    private float levelScale = 1;
    public enum WeaponType { Melee, Bullet }
    public WeaponType weaponType;

    public void damageMult(float mult) { dmgMult = mult; }
    public void setUnderwaterMult(float mult) { underwaterMult = mult; }
    public void setAirborneMult(float mult) { airborneMult = mult; }
    public void setCrit(float chance) { critChance = chance; }
    public void setLevelScale(float scale) { levelScale = scale; }

    private int critCalc()
    {
        if (Random.Range(0, 1f) < (critChance * 0.1))
        {
            return 2;
        }

        return 1;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.CompareTag("Enemy"))
            return;

        int crit = critCalc();
        if (crit > 1)
        {
            if (collision.gameObject.GetComponent<AI>().getType() == Enemy.Types.Airborne || collision.gameObject.GetComponent<AI>().getType() == Enemy.Types.AirborneBoss)
                collision.SendMessage("CritDamage", damage * dmgMult * airborneMult * crit * levelScale);
            else if (collision.gameObject.GetComponent<AI>().getType() == Enemy.Types.Underwater || collision.gameObject.GetComponent<AI>().getType() == Enemy.Types.WaterBoss)
                collision.SendMessage("CritDamage", damage * dmgMult * underwaterMult * crit * levelScale);
        }
        else
        {
            if (collision.gameObject.GetComponent<AI>().getType() == Enemy.Types.Airborne || collision.gameObject.GetComponent<AI>().getType() == Enemy.Types.AirborneBoss)
                collision.SendMessage("TakeDamage", damage * dmgMult * airborneMult * levelScale);
            else if (collision.gameObject.GetComponent<AI>().getType() == Enemy.Types.Underwater || collision.gameObject.GetComponent<AI>().getType() == Enemy.Types.WaterBoss)
                collision.SendMessage("TakeDamage", damage * dmgMult * underwaterMult * levelScale);
        }
    }
}
