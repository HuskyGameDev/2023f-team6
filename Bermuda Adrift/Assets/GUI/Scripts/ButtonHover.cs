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
    [SerializeField] Button menuButton;
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
        else if (menuButton != null)
        {
            AudioManager.Instance.PlaySFX("Menu Select Click");
            gameObject.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(1.25f, 1.25f, 1.25f);
        }

        if (buttonDescription != null)
            buttonDescription.SendMessage("mouseEnter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //OnHoverExit?.Invoke();

        if (menuButton != null)
        {
            gameObject.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        }

        if (buttonDescription != null)
            buttonDescription.SendMessage("mouseExit");
    }
}
