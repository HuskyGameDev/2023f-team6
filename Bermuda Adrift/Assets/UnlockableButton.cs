using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnlockableButton : MonoBehaviour
{
    public static event Action<Enemy> unlockableButtonClicked;

    public Enemy enemy;
    Color32 myCol;
    [SerializeField] bool locked = true;
    [SerializeField] GameObject viewport;
    [SerializeField] GameObject descriptionField;
    Image iconImage;
    Image borderImage;
    Image backgroundImage;
    TextMeshProUGUI txt;

    private void OnEnable()
    {
        LogbookManager.enemyUnlocked += unlockButton;
    }

    private void OnDisable()
    {
        LogbookManager.enemyUnlocked -= unlockButton;
    }

    // Start is called before the first frame update
    void Start()
    {   
        iconImage = gameObject.transform.GetChild(0).GetComponent<Image>();
        borderImage = gameObject.transform.GetChild(1).GetComponent<Image>();
        backgroundImage = gameObject.GetComponent<Image>();
        myCol = new Color(247f / 255f, 179f / 255f, 137f / 255f);
        txt = gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        GetComponent<Button>().onClick.AddListener(buttonClicked);
    }

    public void unlockButton(Enemy e)
    {
        if (e.getName().Equals(this.enemy.getName()))
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
            unlockableButtonClicked?.Invoke(enemy);
        }
    }
}
