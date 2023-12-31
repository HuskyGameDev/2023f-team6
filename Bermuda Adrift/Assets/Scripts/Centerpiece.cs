using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Centerpiece : MonoBehaviour, IPointerDownHandler
{
    public event Action<int> onCenterpieceDamage;
    public static event Action onCenterpieceDamaged;
    public static event Action<Centerpiece> onClicked;


    [SerializeField] private int maxHealth;
    private int Health;
    private GameObject manager;

    private void Start() 
    { 
        manager = GameObject.FindGameObjectWithTag("Managers");
        Health = maxHealth;

        AddPhysics2DRaycaster();
    }
    void TakeDamage(int damage)
    {
        Health -= damage;

        onCenterpieceDamage?.Invoke(damage);
        onCenterpieceDamaged?.Invoke();

        if (Health <= 0)
            manager.SendMessage("GameEnd");
    }

    private void InflictDebuff() { }    //probably won't do debuffs on the centerpiece, this is just to stop errors

    public void repair(int health) 
    {
        Health += health;
        onCenterpieceDamaged?.Invoke(); //Updates the healthbar
    }

    public int getHealth() { return Health; }
    public int getMaxHealth() { return maxHealth; }

    public void setHealth(int health) { Health = health; }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameObject.FindObjectOfType<GameManager>().getGameState() == GameManager.GameState.Idle)
            onClicked?.Invoke(this);
    }

    private void AddPhysics2DRaycaster()
    {
        Physics2DRaycaster physicsRaycaster = FindObjectOfType<Physics2DRaycaster>();
        if (physicsRaycaster == null)
        {
            Camera.main.gameObject.AddComponent<Physics2DRaycaster>();
        }
    }
}
