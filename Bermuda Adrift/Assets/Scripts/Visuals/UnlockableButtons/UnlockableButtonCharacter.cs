using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnlockableButtonCharacter : UnlockableButtonBase
{
    public static event Action<Player> UnlockableTowerButtonClicked;

    public Player player;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(buttonClicked);
        iconImage = gameObject.transform.GetChild(0).GetComponent<Image>();
        borderImage = gameObject.transform.GetChild(1).GetComponent<Image>();
        backgroundImage = gameObject.GetComponent<Image>();
        myCol = new Color(247f / 255f, 179f / 255f, 137f / 255f);
        txt = gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
    }

    public void unlockTowerButton(Player p)
    {
        if (p.getName().Equals(player.getName()))
        {
            locked = false;
            iconImage.color = Color.white;
            borderImage.color = Color.white;
            backgroundImage.color = myCol;
            txt.text = player.getName();
            FindObjectOfType<DataPersistenceManager>().getSavingData().getCharacterSave(p.name).logged = true;
        }
    }

    public void buttonClicked()
    {
        if (!locked)
        {
            UnlockableTowerButtonClicked?.Invoke(player);
        }
    }
}
