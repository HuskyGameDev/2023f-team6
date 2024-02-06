using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnlockableButtonEnemy : UnlockableButtonBase
{
    public static event Action<Enemy> UnlockableEnemyButtonClicked;

    public Enemy enemy;

    private void OnEnable()
    {
        LogbookManager.EnemyUnlocked += unlockEnemyButton;
    }

    private void OnDisable()
    {
        LogbookManager.EnemyUnlocked -= unlockEnemyButton;
    }

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(buttonClicked);
        iconImage = gameObject.transform.GetChild(0).GetComponent<Image>();
        borderImage = gameObject.transform.GetChild(1).GetComponent<Image>();
        backgroundImage = gameObject.GetComponent<Image>();
        myCol = new Color(247f / 255f, 179f / 255f, 137f / 255f);
        txt = gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
    }

    public void unlockEnemyButton(Enemy e)
    {
        if (e.getName().Equals(enemy.getName()))
        {
            locked = false;
            iconImage.color = Color.white;
            borderImage.color = Color.white;
            backgroundImage.color = myCol;
            txt.text = e.getName();
        }
    }

    public void buttonClicked()
    {
        if (!locked)
        {
            UnlockableEnemyButtonClicked?.Invoke(enemy);
        }
    }
}
