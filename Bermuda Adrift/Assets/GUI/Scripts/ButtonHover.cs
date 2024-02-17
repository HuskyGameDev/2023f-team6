using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Tower tower;
    public BarrierScriptable barrier;
    public static event Action<Tower> OnHoverEnter;
    public static event Action<BarrierScriptable> OnHoverEnterB;
    //public static event Action OnHoverExit;
    private ButtonDescription buttonDescription;
    Image image;

    private void Start()
    {
        image = GetComponent<Image>();
        buttonDescription = gameObject.GetComponent<ButtonDescription>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tower != null)
            OnHoverEnter?.Invoke(tower);
        else if (barrier != null)
            OnHoverEnterB?.Invoke(barrier);

        AudioManager.Instance.PlaySFX("Menu Select Click");

        if (buttonDescription != null)
            buttonDescription.SendMessage("mouseEnter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //OnHoverExit?.Invoke();
        if (buttonDescription != null)
            buttonDescription.SendMessage("mouseExit");
    }
}
