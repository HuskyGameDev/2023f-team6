using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TurretMiddleMan : MonoBehaviour, IPointerDownHandler
{
    public static event Action<GameObject> onClicked;


    #region Sound Functions
    public void beforeBullet()
    {
        SendMessageUpwards("beforeFire");
    }

    public void towerPlaced() { SendMessageUpwards("towerPlace"); }

    public void towerReloaded() { SendMessageUpwards("towerReload"); }

    public void lightningRodActive1()
    {
        SendMessageUpwards("lightningRodSparks1");
    }

    public void lightningRodActive2()
    {
        SendMessageUpwards("lightningRodSparks2");
    }
    #endregion

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
            Camera.main.GetComponent<Physics2DRaycaster>().eventMask = LayerMask.GetMask(new string[] { "Default", "Towers" });
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8
            && collision.GetComponent<TowerAI>() != null
            && collision.GetComponent<TowerAI>().getTower().getName().CompareTo("Totem Pole") == 0)
            SendMessageUpwards("addBuff", collision.GetComponent<TowerAI>().getBulletBuff());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8 
            && collision.GetComponent<TowerAI>() != null 
            && collision.GetComponent<TowerAI>().getTower().getName().CompareTo("Totem Pole") == 0)
            SendMessageUpwards("removeBuff", collision.GetComponent<TowerAI>().getBulletBuff());
    }
}
