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

    private CenterpieceScriptable center;
    private int maxHealth;
    [SerializeField] private GameObject shield;
    private GameObject shieldGO;
    private int Health;
    private GameManager manager;
    private Transform Player;
    private SpriteRenderer spriteRenderer;
    private int maxBarrier;
    private int barrierDamage = 0;
    private int barrierReflect;
    private int regen;

    private void OnEnable()
    {
        GameManager.onRoundEnd += resetBarrier;
        GameManager.onRoundEnd += resetBarrierReflect;
        GameManager.onRoundEnd += regenerate;

        GameManager.OnRoundStart += barrierSetup;
    }
    private void OnDisable()
    {
        GameManager.onRoundEnd -= resetBarrier;
        GameManager.onRoundEnd -= resetBarrierReflect;
        GameManager.onRoundEnd -= regenerate;

        GameManager.OnRoundStart -= barrierSetup;
    }
    private void Start() 
    { 
        manager = FindObjectOfType<GameManager>();
        Player = FindObjectOfType<Movement>().gameObject.transform;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        Health = maxHealth;
        shieldGO = Instantiate(shield, transform);
        shieldGO.GetComponent<Animator>().SetFloat("HealthPercent", 1);
        shieldGO.SetActive(false);

        AddPhysics2DRaycaster();
    }
    private void Update()
    {
        if (Player.position.y >= transform.position.y)
            spriteRenderer.sortingOrder = 5;
        else
            spriteRenderer.sortingOrder = 3;
    }

    public void setCenterpiece(CenterpieceScriptable centerpiece)
    {
        center = centerpiece;

        maxHealth = center.getHealth();
        Health = maxHealth;

        maxBarrier = center.getBarrier();

        regen = center.getRegen();
        gameObject.GetComponent<SpriteRenderer>().sprite = center.getSprite();

        onCenterpieceDamaged?.Invoke();
        Debug.Log(maxHealth);
    }

    void barrierSetup()
    {
        if (maxBarrier > 0)
            shieldGO.SetActive(true);
    }

    void resetBarrier() 
    { 
        barrierDamage = 0; 
        maxBarrier = center.getBarrier();
        shieldGO.SetActive(false); 
        onCenterpieceDamaged?.Invoke();
    }
    void addBarrierStrength(int strength) 
    { 
        maxBarrier += strength; 
        shieldGO.SetActive(true); 
        onCenterpieceDamaged?.Invoke();
    }
    void resetBarrierReflect() 
    { 
        barrierReflect = 0; 
        shieldGO.SetActive(false); 
    }
    void addBarrierReflect(int reflect) 
    {
        barrierReflect += reflect; 
        shieldGO.SetActive(true);
    }

    void barrierDestroyed() { shieldGO.SetActive(false); }

    void regenerate()
    {
        if (Health + regen > maxHealth)
            maxHealth = Health + regen;

        Health += regen;
        onCenterpieceDamaged?.Invoke();
    }

    void TakeDamage((int, AI) enemyDamage)
    {
        if (maxBarrier > barrierDamage) 
        {
            if (barrierReflect > 0)
                enemyDamage.Item2.SendMessage("TakeDamage", barrierReflect);

            barrierDamage += enemyDamage.Item1; 
            shieldGO.GetComponent<Animator>().SetFloat("HealthPercent", (float) (maxBarrier - barrierDamage) / maxBarrier); 
        }
        else { Health -= enemyDamage.Item1; }
        
        onCenterpieceDamage?.Invoke(enemyDamage.Item1);

        onCenterpieceDamaged?.Invoke();

        if (Health <= 0)
            manager.SendMessage("GameEnd");
    }
    void bulletTakeDamage(int enemyDamage)
    {
        if (maxBarrier > barrierDamage)
        {
            barrierDamage += enemyDamage;
            shieldGO.GetComponent<Animator>().SetFloat("HealthPercent", (float)(maxBarrier - barrierDamage) / maxBarrier);
        }
        else { Health -= enemyDamage; }

        onCenterpieceDamage?.Invoke(enemyDamage);

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

    public int getHealth() { return Health + maxBarrier - barrierDamage; }
    public int getMaxHealth() { return maxHealth; }
    public int getMaxBarrier() { return maxBarrier; }
    public int getCurrentHealth() { return Health; }

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
