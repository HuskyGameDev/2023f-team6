using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeIndicatorScript : MonoBehaviour
{
    private GameObject towerParent;
    private void Awake()
    {
        deactivate();
    }
    private void OnEnable()
    {
        AnimationHandler.onUpgradeOpened += updateRange;
        AnimationHandler.onUpgradeClosed += deactivate;
        BuildManager.OnTowerPicked += updateRangeDirect;
        TowerAI.OnTowerPlaced += deactivate;
        TowerAI.OnUpgraded += updateRange;
    }
    private void OnDisable()
    {
        AnimationHandler.onUpgradeOpened -= updateRange;
        AnimationHandler.onUpgradeClosed -= deactivate;
        BuildManager.OnTowerPicked -= updateRangeDirect;
        TowerAI.OnTowerPlaced -= deactivate;
        TowerAI.OnUpgraded -= updateRange;
    }
    private void Update()
    {
        if (towerParent == null)
        {
            var mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0f;
            gameObject.transform.position = new Vector3(Mathf.Round(mouseWorldPosition.x), Mathf.Round(mouseWorldPosition.y));
        }
        else
            gameObject.transform.position = towerParent.transform.position;
    }

    public void updateRange(TowerAI towerAI)
    {
        towerParent = towerAI.gameObject;
        transform.localScale = new Vector3(towerAI.getTowerRange() * 2, towerAI.getTowerRange() * 2);
        activate();
    }
    public void updateRangeDirect(Tower tower)
    {
        towerParent = null;
        transform.localScale = new Vector3(tower.getRange() * 2, tower.getRange() * 2);
        activate();
    }
    private void activate() { gameObject.GetComponent<SpriteRenderer>().enabled = true; }
    public void deactivate() { gameObject.GetComponent<SpriteRenderer>().enabled = false; }
}
