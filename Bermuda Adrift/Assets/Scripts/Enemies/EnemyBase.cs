using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBase : MonoBehaviour
{
    public static event Action<int> onEnemyTakeDmg;

    private enum Types {Underwater, Surface, Airborne, Center};

    [SerializeField] private Types Type;

    [SerializeField] private int Health;
    [SerializeField] private int Damage;
    [SerializeField] private float Speed;
    [SerializeField] private int xp;
    [SerializeField] private int scrap;
    [SerializeField] private GameObject enemyManager;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Animator animator;

    //For when we add more enemies
    //At start of round, decide what type of enemy and then specific enemies are randomized
    //0 doesn't spawn, 1 always spawns
    [SerializeField] private float Rarity = 1;
    private void Start()
    {
        if (gameObject.GetComponent<Animator>() != null) animator = gameObject.GetComponent<Animator>();

        if (healthSlider != null) healthSlider.maxValue = Health;

        enemyManager = GameObject.FindGameObjectWithTag("Managers");
        
    }
    private void Update()
    {
        if (Input.GetKeyDown("a"))
        {
            if (animator != null)
            {
                animator.ResetTrigger("Hurt");
                animator.SetTrigger("Hurt");
            }
            TakeDamage(1);
        }
    }

    private void TakeDamage (int damage)
    {
        Health -= damage;
        if (healthSlider != null) healthSlider.value = Health;
        onEnemyTakeDmg?.Invoke(damage);
        //Mini Healthbar?
        if (Health <= 0)
            death();
    }

    void CenterDamage(int damage)
    {
        if (Type == Types.Center)
        {
            Health -= damage;
            //Update UI?

            if (Health <= 0)
                SendMessage("Game End");
        }
    }

    private void death()
    {
        //Play death animation
        Destroy(gameObject);
        //Add XP
        //Add Scrap
        enemyManager.SendMessage("EnemyDown");
    }

    internal float getSpeed()
    {
        return Speed;
    }

    private void heal(int health)   //For repairing the raft, or maybe an enemy that heals other enemies?
    {
        Health += health;
    }

    public float getRarity()
    {
        return Rarity;
    }
}
