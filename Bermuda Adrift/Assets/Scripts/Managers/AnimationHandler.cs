using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    public static event Action<TowerAI> onUpgradeOpened;
    public static event Action<Barriers> onUpgradeOpenedB;
    public static event Action<Centerpiece> onUpgradeOpenedC;
    public static event Action onUpgradeClosed;

    [SerializeField] Animator buyMenu;
    [SerializeField] Animator upgradeMenu;
    [SerializeField] Animator tooltips;
    [SerializeField] Animator levelUpCard1;
    [SerializeField] Animator levelUpCard2;
    [SerializeField] Animator levelUpCard3;

    bool upgradeMenuOut;
    bool tooltipOnScreen;
    private void OnEnable()
    {
        GameManager.onRoundEnd += BuyMenu_SlideIn;
        GameManager.OnRoundStart += BuyMenu_SlideOut;
        TurretMiddleMan.onClicked += upgradeMenu_SlideIn;
        GameManager.OnRoundStart += upgradeMenu_SlideOut;
        UpgradeMenuHandler.OnDestroy += upgradeMenu_SlideOut;
        Hints.OnPopup += tooltipsPopup;
        Hints.OnPopdown += tooltipsPopdown;
        Centerpiece.onClicked += upgradeMenu_SlideIn;
        Barriers.onClicked += upgradeMenu_SlideIn;
        LevelGUIManager.OnLevelUpOpen += levelUpScreen_SlideIn;
        LevelGUIManager.OnLevelUpClose += levelUpScreen_SlideOut;
    }

    private void OnDisable()
    {
        GameManager.onRoundEnd -= BuyMenu_SlideIn;
        GameManager.OnRoundStart -= BuyMenu_SlideOut;
        TurretMiddleMan.onClicked -= upgradeMenu_SlideIn;
        GameManager.OnRoundStart -= upgradeMenu_SlideOut;
        UpgradeMenuHandler.OnDestroy -= upgradeMenu_SlideOut;
        Hints.OnPopup -= tooltipsPopup;
        Hints.OnPopdown -= tooltipsPopdown;
        Centerpiece.onClicked -= upgradeMenu_SlideIn;
        Barriers.onClicked -= upgradeMenu_SlideIn;
        LevelGUIManager.OnLevelUpOpen -= levelUpScreen_SlideIn;
        LevelGUIManager.OnLevelUpClose -= levelUpScreen_SlideOut;
    }
    void BuyMenu_SlideIn()
    {
        buyMenu.Play("BuyMenu_SlideIn");
    }

    void BuyMenu_SlideOut()
    {
        buyMenu.Play("BuyMenu_SlideOut");
    }
    void upgradeMenu_SlideIn(GameObject go) 
    {
        onUpgradeOpened?.Invoke(go.GetComponent<TowerAI>());
        if (!upgradeMenuOut)
        {
            upgradeMenu.Play("SlideIn");
            upgradeMenuOut = true;
        }
    }
    void upgradeMenu_SlideIn(Centerpiece c)
    {
        onUpgradeOpenedC?.Invoke(c);
        if (!upgradeMenuOut)
        {
            upgradeMenu.Play("SlideIn");
            upgradeMenuOut = true;
        }
    }
    void upgradeMenu_SlideIn(Barriers b)
    {
        onUpgradeOpenedB?.Invoke(b);
        if (!upgradeMenuOut)
        {
            upgradeMenu.Play("SlideIn");
            upgradeMenuOut = true;
        }
    }
    void upgradeMenu_SlideOut()
    {
        if (upgradeMenuOut) //Won't play the animation if it wasn't opened
        {
            upgradeMenuOut = false;
            onUpgradeClosed?.Invoke();
            upgradeMenu.Play("SlideOut");
        }
    }
    void tooltipsPopup()
    {
        if (!tooltipOnScreen)
        {
            tooltips.Play("TooltipSlideIn");
            tooltipOnScreen = true;
        }
    }
    void tooltipsPopdown()
    {
        if (tooltipOnScreen)
        {
            tooltips.Play("TooltipSlideOut");
            tooltipOnScreen = false;
        }
    }

    void levelUpScreen_SlideIn()
    {
        StartCoroutine(levelUp_SlideIn());
    }
    void levelUpScreen_SlideOut()
    {
        StartCoroutine(levelUp_SlideOut());
    }
    private IEnumerator levelUp_SlideIn()
    {
        levelUpCard1.Play("SlideIn");

        yield return new WaitForSeconds(0.25f);

        levelUpCard2.Play("SlideIn");

        yield return new WaitForSeconds(0.25f);

        levelUpCard3.Play("SlideIn");
    }
    private IEnumerator levelUp_SlideOut()
    {
        levelUpCard1.Play("SlideOut");

        yield return new WaitForSeconds(0.25f);

        levelUpCard2.Play("SlideOut");

        yield return new WaitForSeconds(0.25f);

        levelUpCard3.Play("SlideOut");
    }
}
