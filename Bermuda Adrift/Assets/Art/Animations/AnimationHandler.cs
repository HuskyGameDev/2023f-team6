using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    [SerializeField] Animator buyMenu;
    private void OnEnable()
    {
        GameManager.onRoundEnd += BuyMenu_SlideIn;
        GameManager.OnRoundStart += BuyMenu_SlideOut;
    }

    private void OnDisable()
    {
        GameManager.onRoundEnd -= BuyMenu_SlideIn;
        GameManager.OnRoundStart -= BuyMenu_SlideOut;
    }

    void BuyMenu_SlideIn()
    {
        buyMenu.Play("BuyMenu_SlideIn");
    }

    void BuyMenu_SlideOut()
    {
        buyMenu.Play("BuyMenu_SlideOut");
    }
}
