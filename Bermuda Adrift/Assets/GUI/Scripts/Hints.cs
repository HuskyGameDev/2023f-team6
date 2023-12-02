using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Hints : MonoBehaviour
{
    public static event Action OnPopup;
    public static event Action OnPopdown;
    public static event Action OnPopupAppeared;

    Stack<string> stack;
    TextMeshProUGUI textMesh;
    [SerializeField] bool disabled;
    bool tooltipEndEarly;

    private void OnEnable()
    {
        GameManager.onRoundEnd += displayPopup;
        GameManager.OnRoundStart += disappear;

        Attack.loopUsed += loopAttacks;
        Attack.barrelUsed += shootBarrels;
        BuildManager.OnTwoTowersPlaced += twoTowers;
        AI.OnDecoyDeath += decoyDeath;
        OnPopupAppeared += endTooltipEarly;
        GameManager.OnBossWarning += bossWarning;
        GameManager.OnBossApproaching += bossApproaching;
        Centerpiece.onCenterpieceDamaged += repairHint;
    }
    private void OnDisable()
    {
        GameManager.onRoundEnd -= displayPopup;
        GameManager.OnRoundStart -= disappear;

        Attack.loopUsed -= loopAttacks;
        Attack.barrelUsed -= shootBarrels;
        BuildManager.OnTwoTowersPlaced -= twoTowers;
        AI.OnDecoyDeath -= decoyDeath;
        OnPopupAppeared -= endTooltipEarly;
        GameManager.OnBossWarning -= bossWarning;
        GameManager.OnBossApproaching -= bossApproaching;
        Centerpiece.onCenterpieceDamaged -= repairHint;
    }
    private void Start()
    {
        stack = new Stack<string>();
        textMesh = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
    }
    public void disappear() { OnPopdown?.Invoke(); }

    public void displayPopup()
    {
        if (disabled) return;

        string tip;
        if (!stack.TryPop(out tip))
            return;

        textMesh.text = tip;
        OnPopup?.Invoke();
        
        OnPopupAppeared?.Invoke();

        StartCoroutine(delay(5f));
    }
    private IEnumerator delay(float timeOnScreen)
    {
        yield return new WaitForSeconds(timeOnScreen);
        disappear();
    }

    #region Tips
    private void endTooltipEarly()
    {
        stack.Push("You can click on these tips to make them disappear faster!");

        OnPopupAppeared -= endTooltipEarly;
    }
    void loopAttacks()
    {
        stack.Push("For some attacks, you can hold down the button to keep attacking");
        stack.Push("If you hold down the button for any attack, it will activate once it's cooldown finishes");

        Attack.loopUsed -= loopAttacks;
    }
    void shootBarrels()
    {
        stack.Push("Fun Fact: you can shoot your barrels to detonate them early");

        Attack.barrelUsed -= shootBarrels;
    }
    void twoTowers() 
    {
        stack.Push("If you click on a tower, you can upgrade it to make it much more powerful");

        BuildManager.OnTwoTowersPlaced -= twoTowers;
    }
    void decoyDeath()
    {
        stack.Push("Some enemies will attract all the attention of your towers. Apart from that, they're mostly harmless");

        AI.OnDecoyDeath -= decoyDeath;
    }
    void bossWarning()
    {
        stack.Push("A powerful enemy approaches...");
    }
    void bossApproaching()
    {
        stack.Push("PREPARE YOURSELF");
    }
    void repairHint()
    {
        stack.Push("You can click on the centerpiece to have a chance to repair it. Be warned, the more damage there is, the more costly it will be to fix");
        
        Centerpiece.onCenterpieceDamaged -= repairHint;
    }
    #endregion
}