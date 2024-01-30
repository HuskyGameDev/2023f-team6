using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TurretMiddleMan : MonoBehaviour, IPointerDownHandler
{
    public static event Action<GameObject> onClicked;
    public void fireBullet()
    {
        SendMessageUpwards("fire");
    }
    public void destroy() { SendMessageUpwards("destroyTower"); }
    /*
    private void OnMouseDown()
    {
        if (transform.parent.gameObject.GetComponent<TowerAI>().getPlaced() 
            && 
            GameObject.Find("Managers").GetComponent<GameManager>().getGameState() == GameManager.GameState.Idle)
        {
            onClicked?.Invoke(transform.parent.gameObject);
        }
    }
    */
    private void Start()
    {
        AddPhysics2DRaycaster();
    }

    public void openUpgradeMenu() { onClicked?.Invoke(transform.parent.gameObject); }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (transform.parent.gameObject.GetComponent<TowerAI>().getPlaced()
            &&
            GameObject.Find("Managers").GetComponent<GameManager>().getGameState() == GameManager.GameState.Idle)
        {
            onClicked?.Invoke(transform.parent.gameObject);
        }
    }

    private void AddPhysics2DRaycaster()
    {
        Physics2DRaycaster physicsRaycaster = FindObjectOfType<Physics2DRaycaster>();
        if (physicsRaycaster == null)
        {
            Camera.main.gameObject.AddComponent<Physics2DRaycaster>();
        }
    }
}
