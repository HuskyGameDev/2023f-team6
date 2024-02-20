using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IslandInteractions : MonoBehaviour, IPointerDownHandler
{
    private IslandManager islandManager;
    private Island island;

    private void Start()
    {
        islandManager = FindObjectOfType<IslandManager>();
        AddPhysics2DRaycaster();
    }

    private void setIsland(Island newIsland)
    {
        this.island = newIsland;
        //Set sprite (animator will be the same for all and will move the island around)
        gameObject.GetComponent<SpriteRenderer>().sprite = newIsland.getSprite();
    }

    public void deleteIsland()
    {
        islandManager.deleteIsland(this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        islandManager.SendMessage("interact", island);
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
