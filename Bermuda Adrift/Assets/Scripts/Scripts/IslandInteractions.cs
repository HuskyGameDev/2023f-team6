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
        AddPhysics2DRaycaster();
    }

    private void setIsland(Island newIsland)
    {
        islandManager = FindObjectOfType<IslandManager>();

        island = newIsland;
        //Set sprite (animator will be the same for all and will move the island around)
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = newIsland.getSprite();
        transform.GetChild(0).GetComponent<Animator>().runtimeAnimatorController = newIsland.getAnimator();

        if (island.getIslandType() == Island.islandType.Obelisk)
        {
            if (islandManager.getObeliskCount() == 0)
                transform.GetChild(0).GetComponent<Animator>().Play("Obelisk_Y_idle");
            if (islandManager.getObeliskCount() == 1)
                transform.GetChild(0).GetComponent<Animator>().Play("Obelisk_B_idle");
            if (islandManager.getObeliskCount() == 2)
                transform.GetChild(0).GetComponent<Animator>().Play("Obelisk_R_idle");
            if (islandManager.getObeliskCount() == 3)
                transform.GetChild(0).GetComponent<Animator>().Play("Obelisk_P_idle");
            if (islandManager.getObeliskCount() >= 4)
                transform.GetChild(0).GetComponent<Animator>().Play("True_Obelisk_idle");
        }

    }

    public void deleteIsland()
    {
        islandManager.deleteIsland(this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        islandManager.SendMessage("interact", this);
    }

    private void AddPhysics2DRaycaster()
    {
        Physics2DRaycaster physicsRaycaster = FindObjectOfType<Physics2DRaycaster>();
        if (physicsRaycaster == null)
        {
            Camera.main.gameObject.AddComponent<Physics2DRaycaster>();
        }
    }

    public Island getIsland() { return island; }
}
