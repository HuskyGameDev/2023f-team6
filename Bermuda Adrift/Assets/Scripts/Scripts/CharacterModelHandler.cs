using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CharacterModelHandler : MonoBehaviour
{
    public static event Action<Player> onCharacterChange;

    [SerializeField] private Player character;
    [SerializeField] private Image Primary;
    [SerializeField] private Image Secondary;
    [SerializeField] private Image Utility;
    [SerializeField] private Image Special;
    [SerializeField] private Image mainBody;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private GameObject abilityDescriptions;
    private void Start()
    {
        setCharacter(character);    
    }
    public void setCharacter(Player character)
    {
        this.character = character;
        updateDescription();
        for (int i = 1; i < 6; i++)
            updateModel(i);

        setDefPrimary();
        setDefSecondary();
        setDefUtility();
        setDefSpecial();

        onCharacterChange?.Invoke(character);
    }

    #region Loadout functions
    public void setDefPrimary()
    {
        character.alt(1, false);
        updateModel(1);
        abilityDescriptions.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = character.getPrimary().getName() + " - " + character.getPrimary().getDescription();
        abilityDescriptions.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = character.getPrimary().getThumbnail();
    }
    public void setAltPrimary()
    {
        character.alt(1, true);
        updateModel(1);
        abilityDescriptions.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = character.getPrimary().getName() + " - " + character.getPrimary().getDescription();
        abilityDescriptions.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = character.getPrimary().getThumbnail();
    }
    public void setDefSecondary()
    {
        character.alt(2, false);
        updateModel(2);
        abilityDescriptions.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = character.getSecondary().getName() + " - " + character.getSecondary().getDescription();
        abilityDescriptions.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = character.getSecondary().getThumbnail();
    }
    public void setAltSecondary()
    {
        character.alt(2, true);
        updateModel(2);
        abilityDescriptions.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = character.getSecondary().getName() + " - " + character.getSecondary().getDescription();
        abilityDescriptions.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = character.getSecondary().getThumbnail();
    }
    public void setDefUtility()
    {
        character.alt(3, false);
        updateModel(3);
        abilityDescriptions.transform.GetChild(2).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = character.getUtility().getName() + " - " + character.getUtility().getDescription();
        abilityDescriptions.transform.GetChild(2).gameObject.GetComponent<Image>().sprite = character.getUtility().getThumbnail();
    }
    public void setAltUtility()
    {
        character.alt(3, true);
        updateModel(3);
        abilityDescriptions.transform.GetChild(2).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = character.getUtility().getName() + " - " + character.getUtility().getDescription();
        abilityDescriptions.transform.GetChild(2).gameObject.GetComponent<Image>().sprite = character.getUtility().getThumbnail();
    }
    public void setDefSpecial()
    {
        character.alt(4, false);
        updateModel(4);
        abilityDescriptions.transform.GetChild(3).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = character.getSpecial().getName() + " - " + character.getSpecial().getDescription();
        abilityDescriptions.transform.GetChild(3).gameObject.GetComponent<Image>().sprite = character.getSpecial().getThumbnail();
    }
    public void setAltSpecial()
    {
        character.alt(4, true);
        updateModel(4);
        abilityDescriptions.transform.GetChild(3).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = character.getSpecial().getName() + " - " + character.getSpecial().getDescription();
        abilityDescriptions.transform.GetChild(3).gameObject.GetComponent<Image>().sprite = character.getSpecial().getThumbnail();
    }
    public void setDefPassive()
    {
        character.alt(5, false);
        updateModel(5);
        abilityDescriptions.transform.GetChild(4).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = character.getPassive().getDescription();
        abilityDescriptions.transform.GetChild(4).gameObject.GetComponent<Image>().sprite = character.getPassive().getThumbnail();
    }
    public void setAltPassive()
    {
        character.alt(5, true);
        updateModel(5);
        abilityDescriptions.transform.GetChild(4).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = character.getPassive().getDescription();
        abilityDescriptions.transform.GetChild(4).gameObject.GetComponent<Image>().sprite = character.getPassive().getThumbnail();
    }
    #endregion

    public void updateModel(int slot)
    {
        if (slot == 1 && Primary != null) 
        { 
            Primary.sprite = character.getPrimary().getModelSprite();
            if (character.getPrimary().getModelSprite() == null)
                Primary.gameObject.SetActive(false);
            else
                Primary.gameObject.SetActive(true);
        }
        else if (slot == 2 && Secondary != null)
        {
            Secondary.sprite = character.getSecondary().getModelSprite();
            if (character.getSecondary().getModelSprite() == null)
                Secondary.gameObject.SetActive(false);
            else
                Secondary.gameObject.SetActive(true);
        }
        else if (slot == 3 && Utility != null) 
        {
            Utility.sprite = character.getUtility().getModelSprite();
            if (character.getUtility().getModelSprite() == null)
                Utility.gameObject.SetActive(false);
            else
                Utility.gameObject.SetActive(true);
        }
        else if (slot == 4 && Special != null) 
        {
            Special.sprite = character.getSpecial().getModelSprite();
            if (character.getSecondary().getModelSprite() == null)
                Special.gameObject.SetActive(false);
            else
                Special.gameObject.SetActive(true);
        }
        else if (slot == 5 && mainBody != null) { mainBody.sprite = character.getMainBodySprite(); }
    }
    public void updateDescription()
    {
        description.text = character.getDescription();
    }
}
