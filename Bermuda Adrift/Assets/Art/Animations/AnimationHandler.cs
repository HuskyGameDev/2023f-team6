using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    public static event Action<TowerAI> onUpgradeOpened;
    public static event Action onUpgradeClosed;

    [SerializeField] Animator buyMenu;
    [SerializeField] Animator upgradeMenu;
    bool upgradeMenuOut;
    private void OnEnable()
    {
        GameManager.onRoundEnd += BuyMenu_SlideIn;
        GameManager.OnRoundStart += BuyMenu_SlideOut;
        TurretMiddleMan.onClicked += upgradeMenu_SlideIn;
        GameManager.OnRoundStart += upgradeMenu_SlideOut;
        UpgradeMenuHandler.OnDestroy += upgradeMenu_SlideOut;
    }

    private void OnDisable()
    {
        GameManager.onRoundEnd -= BuyMenu_SlideIn;
        GameManager.OnRoundStart -= BuyMenu_SlideOut;
        TurretMiddleMan.onClicked -= upgradeMenu_SlideIn;
        GameManager.OnRoundStart -= upgradeMenu_SlideOut;
        UpgradeMenuHandler.OnDestroy -= upgradeMenu_SlideOut;
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
    void upgradeMenu_SlideOut()
    {
        if (upgradeMenuOut) //Won't play the animation if it wasn't opened
        {
            upgradeMenuOut = false;
            onUpgradeClosed?.Invoke();
            upgradeMenu.Play("SlideOut");
        }
    }
}
