using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnlockableButtonTower : UnlockableButtonBase
{
    public static event Action<Tower> UnlockableTowerButtonClicked;

    public Tower tower;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(buttonClicked);
        iconImage = gameObject.transform.GetChild(0).GetComponent<Image>();
        borderImage = gameObject.transform.GetChild(1).GetComponent<Image>();
        backgroundImage = gameObject.GetComponent<Image>();
        myCol = new Color(247f / 255f, 179f / 255f, 137f / 255f);
        txt = gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
    }

    public void unlockTowerButton(Tower t)
    {
        if (t.getName().Equals(tower.getName()))
        {
            locked = false;
            iconImage.color = Color.white;
            borderImage.color = Color.white;
            backgroundImage.color = myCol;
            txt.text = tower.getName();
        }
    }

    public void buttonClicked()
    {
        if (!locked)
        {
            UnlockableTowerButtonClicked?.Invoke(tower);
        }
    }
}
