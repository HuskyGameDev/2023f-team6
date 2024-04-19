using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllTiles : MonoBehaviour
{
    private void OnEnable()
    {
        BuildManager.OnTowerPicked += TowerActivateTiles;
        BuildManager.OnBarrierPicked += BarrierActivateTiles;
        TowerAI.OnCancel += DeactivateTiles;
        TowerAI.OnTowerPlaced += DeactivateTiles;
        Barriers.OnCancel += DeactivateTiles;
        Barriers.OnTowerPlaced += DeactivateTiles;
    }

    private void OnDisable()
    {
        BuildManager.OnTowerPicked -= TowerActivateTiles;
        BuildManager.OnBarrierPicked -= BarrierActivateTiles;
        TowerAI.OnCancel -= DeactivateTiles;
        TowerAI.OnTowerPlaced -= DeactivateTiles;
        Barriers.OnCancel -= DeactivateTiles;
        Barriers.OnTowerPlaced -= DeactivateTiles;
    }
    private void Start()
    {
        DeactivateTiles();
    }

    void TowerActivateTiles(Tower tower)
    {
        gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
    }
    void BarrierActivateTiles(BarrierScriptable scriptable)
    {
        gameObject.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
    }

    void DeactivateTiles()
    {
        gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        gameObject.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
    }
}
