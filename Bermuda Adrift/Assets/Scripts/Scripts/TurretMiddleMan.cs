using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretMiddleMan : MonoBehaviour
{
    public static event Action<GameObject> onClicked;
    public void fireBullet()
    {
        SendMessageUpwards("fire");
    }
    private void OnMouseDown()
    {
        if (transform.parent.gameObject.GetComponent<TowerAI>().getPlaced() 
            && 
            GameObject.Find("Managers").GetComponent<GameManager>().getGameState() == GameManager.GameState.Idle)
        {
            onClicked?.Invoke(transform.parent.gameObject);
        }
    }

}
