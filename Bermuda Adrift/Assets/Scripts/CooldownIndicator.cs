using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CooldownIndicator : MonoBehaviour
{
    public enum position { primary, secondary, utility, special};
    private position attackType;

    public static event Action<int> cooldownComplete;

    private Slider slider;
    private float soFar;
    private TextMeshProUGUI text;

    private void OnEnable()
    {
        Attack.updateCooldowns += updateCooldowns;
    }
    private void OnDisable()
    {
        Attack.updateCooldowns -= updateCooldowns;
    }
    private void Awake()
    {
        slider = gameObject.GetComponent<Slider>();
        slider.value = 0;
        slider.interactable = false;

        text = transform.parent.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        text.gameObject.SetActive(false);

        //Determine which of the attacks this slider
        var tempParent = transform.parent.parent.parent; //CooldownIndicators canvas
        var generalType = transform.parent.parent;  //The general types, like Primary or Secondary

        for (int i = 0; i < 4; i++)
        {
            if (tempParent.GetChild(i) == generalType)
                attackType = (position)i;
        }
    }

    public void updateCooldowns(float total, position type)
    {
        if (type != attackType)
            return;

        soFar += Time.deltaTime;
        if (soFar >= total) //Cooldown complete
        {
            cooldownComplete?.Invoke((int)type);
            soFar = 0;
            text.gameObject.SetActive(false);
        }
        else
            text.gameObject.SetActive(true);

        text.text = ((int)(total - soFar)).ToString();
        slider.value = soFar / total;
    }
}
