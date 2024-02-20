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
    [SerializeField] private GameObject shield;
    private int Health;
    private GameManager manager;
    private Transform Player;
    private SpriteRenderer spriteRenderer;
    private int barrier;
    private int barrierDamage;

    private void OnEnable()
    {
        GameManager.onRoundEnd += resetBarrier;
    }
    private void OnDisable()
    {
        GameManager.onRoundEnd -= resetBarrier;
    }
    private void Start() 
    { 
        manager = FindObjectOfType<GameManager>();
        Player = FindObjectOfType<Movement>().gameObject.transform;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        Health = maxHealth;

        AddPhysics2DRaycaster();
    }
    private void Update()
    {
        if (Player.position.y >= transform.position.y)
            spriteRenderer.sortingOrder = 5;
        else
            spriteRenderer.sortingOrder = 3;
    }

    void resetBarrier() { barrierDamage = 0; }
    void setBarrierStrength(int strength) { barrier = strength; }
    void addBarrierStrength(int strength) { barrier += strength; }
    void removeBarrierStrength(int strength) { barrier -= strength; }

    void TakeDamage(int damage)
    {
        if (barrier > barrierDamage) { barrierDamage += damage; }
        else { Health -= damage; }

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
        if (manager.getGameState() == GameManager.GameState.Idle)
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
