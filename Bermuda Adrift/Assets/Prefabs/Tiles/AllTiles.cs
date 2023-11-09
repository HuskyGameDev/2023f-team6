using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllTiles : MonoBehaviour
{
    private void OnEnable()
    {
        BuildManager.OnTowerPicked += ActivateTiles;
        TowerAI.OnCancel += DeactivateTiles;
        TowerAI.OnTowerPlaced += DeactivateTiles;
        Barriers.OnCancel += DeactivateTiles;
        Barriers.OnTowerPlaced += DeactivateTiles;
    }

    private void OnDisable()
    {
        BuildManager.OnTowerPicked -= ActivateTiles;
        TowerAI.OnCancel -= DeactivateTiles;
        TowerAI.OnTowerPlaced -= DeactivateTiles;
        Barriers.OnCancel -= DeactivateTiles;
        Barriers.OnTowerPlaced -= DeactivateTiles;
    }

    void ActivateTiles()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }

    void DeactivateTiles()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }
}
