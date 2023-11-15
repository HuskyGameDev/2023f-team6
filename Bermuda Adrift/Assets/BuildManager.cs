using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static event Action OnTowerPicked;
    public static event Action OnBarrierPicked;

    public static BuildManager main;

    [Header("References")]
    [SerializeField] private GameObject[] towerPrefabs;

    private int selectedTower = 0;  //Not sure what this is for - JD

    private GameManager gameManager;
    private GameObject mostRecent;
    private Vector3[] positions;
    private int activeIndex;

    [SerializeField] private Tower[] towers;
    [SerializeField] private BarrierScriptable[] barriers;

    public GameObject GetSelectedTower() { 
        return towerPrefabs[selectedTower];
    }

    public void placeTower(Tower scriptable)
    {
        if (gameManager.cost(scriptable.getCost()) && recentWasPlaced())
        {
            OnTowerPicked?.Invoke();
            mostRecent = Instantiate(towerPrefabs[0]);

            mostRecent.SendMessage("place", scriptable);

            StartCoroutine(positionTracker());
        }
    }

    public void placeBarrier(BarrierScriptable scriptable)
    {
        if (gameManager.cost(scriptable.getCost()) && recentWasPlaced())
        {
            OnBarrierPicked?.Invoke();
            mostRecent = Instantiate(towerPrefabs[1]);
            mostRecent.SendMessage("setBarrier", scriptable);

            StartCoroutine(positionTracker());
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        gameManager = gameObject.GetComponent<GameManager>();
        positions = new Vector3[48];
    }

    #region Tower/Barrier Placements
    public void PlaceMachineGun()
    {
        placeTower(towers[1]);
    }

    public void PlaceTricannon()
    {
        placeTower(towers[0]);
    }

    public void PlaceBarrier()
    {
        placeBarrier(barriers[0]);
    }

    public void PlaceFishNet()
    {
        placeBarrier(barriers[1]);
    }
    #endregion
    private bool recentWasPlaced()  //Checks if previous tower selected was placed. If cancelled, both will be null and it will return true
    {
        if (mostRecent == null)
            return true;

        if (mostRecent.GetComponent<TowerAI>() != null)
            return mostRecent.GetComponent<TowerAI>().getPlaced();
        else if (mostRecent.GetComponent<Barriers>())
            return mostRecent.GetComponent<Barriers>().getPlaced();
        return true;
    }

    private IEnumerator positionTracker()
    {
        int i = 0;
        for (; i < positions.Length; i++)   //Find next spot to insert a position
        {
            if (positions[i] == Vector3.zero)
                break;
        }

        activeIndex = i;

        while (mostRecent != null && (mostRecent.GetComponent<TowerAI>() != null && !mostRecent.GetComponent<TowerAI>().getPlaced() || mostRecent.GetComponent<Barriers>() != null && !mostRecent.GetComponent<Barriers>().getPlaced()))
        {
            positions[i] = mostRecent.transform.position;
            yield return new WaitForEndOfFrame();
        }

        if (mostRecent == null)
            positions[i] = Vector3.zero;
    }

    public bool approvePosition(Vector3 position)
    {
        Vector3 temp = Vector3.zero;
        for (int i = 0; i < activeIndex; i++)
        {
            if (positions[i] == Vector3.zero) { return true; }

            temp.x = position.x - positions[i].x;
            temp.y = position.y - positions[i].y;
            if (temp.magnitude < 2 ) {
                //Debug.Log("Denied - too close to tower " + i);
                return false; 
            }
        }
        return true;
    }

    public Vector3[] getPositions() { return positions; }

    public void removePosition(Vector3 position)
    {
        for (int i = 0; i < positions.Length; i++)
        {
            if (position == positions[i])
            {
                positions[i] = Vector3.zero;
                return;
            }
        }
    }
}
