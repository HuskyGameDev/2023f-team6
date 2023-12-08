using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutHandler : MonoBehaviour
{
    public GameObject defPrimaryButton;
    public GameObject defSecondaryButton;
    public GameObject defUtilityButton;
    public GameObject defSpecialButton;
    public GameObject defPassiveButton;
    public GameObject altPrimaryButton;
    public GameObject altSecondaryButton;
    public GameObject altUtilityButton;
    public GameObject altSpecialButton;
    public GameObject altPassiveButton;

    private void OnEnable()
    {
        CharacterModelHandler.onCharacterChange += updateLoadoutOptions;
    }
    private void OnDisable()
    {
        CharacterModelHandler.onCharacterChange -= updateLoadoutOptions;
    }
    private void updateLoadoutOptions(Player character)
    {
        defPrimaryButton.GetComponent<Image>().sprite = character.getDefPrimary().getThumbnail();
        if (character.getAltPrimary() != null) altPrimaryButton.GetComponent<Image>().sprite = character.getAltPrimary().getThumbnail();

        defSecondaryButton.GetComponent<Image>().sprite = character.getDefSecondary().getThumbnail();
        if (character.getAltSecondary() != null) altSecondaryButton.GetComponent<Image>().sprite = character.getAltSecondary().getThumbnail();

        defUtilityButton.GetComponent<Image>().sprite = character.getDefUtility().getThumbnail();
        if (character.getAltUtility() != null) altUtilityButton.GetComponent<Image>().sprite = character.getAltUtility().getThumbnail();

        defSpecialButton.GetComponent<Image>().sprite = character.getDefSpecial().getThumbnail();
        if (character.getAltSpecial() != null) altSpecialButton.GetComponent<Image>().sprite = character.getAltSpecial().getThumbnail();

        defPassiveButton.GetComponent<Image>().sprite = character.getDefPassive().getThumbnail();
        if (character.getAltPassive() != null) altPassiveButton.GetComponent<Image>().sprite = character.getAltPassive().getThumbnail();

        if (character.getAltPrimary() != null) altPrimaryButton.SetActive(true); else altPrimaryButton.SetActive(false);
        if (character.getAltSecondary() != null) altSecondaryButton.SetActive(true); else altSecondaryButton.SetActive(false);
        if (character.getAltUtility() != null) altUtilityButton.SetActive(true); else altUtilityButton.SetActive(false);
        if (character.getAltSpecial() != null) altSpecialButton.SetActive(true); else altSpecialButton.SetActive(false);
        if (character.getAltPassive() != null) altPassiveButton.SetActive(true); else altPassiveButton.SetActive(false);
    }
}
