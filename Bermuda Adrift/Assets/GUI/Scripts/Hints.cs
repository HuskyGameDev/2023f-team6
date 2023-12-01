using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Hints : MonoBehaviour
{
    public static event Action OnPopup;
    public static event Action OnPopdown;

    Queue<string> queue;
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
    }
    private void OnDisable()
    {
        GameManager.onRoundEnd -= displayPopup;
        GameManager.OnRoundStart -= disappear;
        Attack.loopUsed -= loopAttacks;
        Attack.barrelUsed -= shootBarrels;
        BuildManager.OnTwoTowersPlaced -= twoTowers;
    }
    private void Start()
    {
        queue = new Queue<string>();
        textMesh = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
    }
    public void disappear() { OnPopdown?.Invoke(); }

    public void displayPopup()
    {
        if (disabled) return;

        string tip;
        if (!queue.TryDequeue(out tip))
            return;

        textMesh.text = tip;
        OnPopup?.Invoke();

        if (!tooltipEndEarly)
        {
            endTooltipEarly();
            tooltipEndEarly = true;
        }
        StartCoroutine(delay(5f));
    }
    private IEnumerator delay(float timeOnScreen)
    {
        yield return new WaitForSeconds(timeOnScreen);
        disappear();
    }

    private void endTooltipEarly() { Debug.Log("Queueing early tooltips"); queue.Enqueue("You can click on these tips to make them disappear!"); }
    public void loopAttacks()
    {
        Debug.Log("Queueing loop attacks");
        queue.Enqueue("For some attacks, you can hold down the button to keep attacking");
        queue.Enqueue("If you hold down the button for any attack, it will activate once it's cooldown finishes");
    }
    public void shootBarrels() { Debug.Log("Queueing barrel attacks"); queue.Enqueue("Fun Fact: you can shoot your barrels to detonate them early"); }
    public void twoTowers() { Debug.Log("Queueing towers"); queue.Enqueue("If you click on a tower, you can upgrade it to make it much more powerful"); }
}
