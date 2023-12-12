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

    private void OnEnable()
    {
        GameManager.onRoundEnd += displayPopup;
        GameManager.OnRoundStart += disappear;

        Attack.loopUsed += loopAttacks;
        Attack.barrelUsed += shootBarrels;
        AI.OnDecoyDeath += decoyDeath;
        OnPopupAppeared += endTooltipEarly;
        GameManager.OnBossWarning += bossWarning;
        Centerpiece.onCenterpieceDamaged += repairHint;
        BuildManager.TooManyBlueprints += blueprintLimit;
        GameManager.OnLevelUp += levelExplanations;
    }
    private void OnDisable()
    {
        GameManager.onRoundEnd -= displayPopup;
        GameManager.OnRoundStart -= disappear;

        Attack.loopUsed -= loopAttacks;
        Attack.barrelUsed -= shootBarrels;
        AI.OnDecoyDeath -= decoyDeath;
        OnPopupAppeared -= endTooltipEarly;
        GameManager.OnBossWarning -= bossWarning;
        Centerpiece.onCenterpieceDamaged -= repairHint;
        BuildManager.TooManyBlueprints -= blueprintLimit;
        GameManager.OnLevelUp -= levelExplanations;
    }
    private void Start()
    {
        stack = new Stack<string>();
        textMesh = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
    }
    public void disappear() 
    {
        OnPopdown?.Invoke(); 
    }

    public void displayPopup()
    {
        if (disabled) return;

        string tip;
        if (!stack.TryPop(out tip))
            return;

        textMesh.text = tip;
        OnPopup?.Invoke();
        
        OnPopupAppeared?.Invoke();

        StartCoroutine(delay(10f));
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
    /*
    void twoTowers() 
    {
        stack.Push("If you click on a tower, you can upgrade it to make it much more powerful");

        BuildManager.OnTwoTowersPlaced -= twoTowers;
    }
    */
    void decoyDeath()
    {
        stack.Push("Some enemies will attract all the attention of your towers. Apart from that, they're mostly harmless");

        AI.OnDecoyDeath -= decoyDeath;
    }
    void bossWarning(string warning)
    {
        stack.Push(warning);
    }
    void repairHint()
    {
        stack.Push("You can click on the centerpiece to have a chance to repair it. Be warned, the more you repair things, the more those repairs will cost");
        
        Centerpiece.onCenterpieceDamaged -= repairHint;
    }
    void blueprintLimit()
    {
        stack.Push("If you have more than 10 blueprints, you won't be able to receive any more");

        BuildManager.TooManyBlueprints -= blueprintLimit;
    }
    void levelExplanations(int i)
    {
        stack.Push("The scrap these creatures give you have some strange properties. Maybe the papers they drop can give insight into their uses");

        GameManager.OnLevelUp -= levelExplanations;
    }
    #endregion
}
