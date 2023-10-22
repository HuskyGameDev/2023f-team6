using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager main;

    [Header("References")]
    [SerializeField] private GameObject[] towerPrefabs;

    private int selectedTower = 0;  //Not sure what this is for - JD

    private GameManager gameManager;
    private GameObject mostRecent;

    [SerializeField] private Tower[] towers;
    [SerializeField] private BarrierScriptable[] barriers;

    public GameObject GetSelectedTower() { 
        return towerPrefabs[selectedTower];
    }

    public void placeTower(Tower scriptable)
    {
        mostRecent = Instantiate(towerPrefabs[0]);

        mostRecent.SendMessage("place", scriptable);
    }

    public void placeBarrier(BarrierScriptable scriptable)
    {
        mostRecent = Instantiate(towerPrefabs[1]);
        mostRecent.SendMessage("setBarrier", scriptable);
    }


    // Start is called before the first frame update
    void Start()
    {
        gameManager = gameObject.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            if (gameManager.cost(towers[0].getCost()) && recentWasPlaced())
                placeTower(towers[0]);
        }
        else if (Input.GetKeyDown("2"))
        {
            if (gameManager.cost(towers[1].getCost()) && recentWasPlaced())
                placeTower(towers[1]);
        }
        else if (Input.GetKeyDown("3"))
        {
            if (gameManager.cost(barriers[0].getCost()) && recentWasPlaced())
                placeBarrier(barriers[0]);
        }
        else if (Input.GetKeyDown("4"))
        {
            if (gameManager.cost(barriers[1].getCost()) && recentWasPlaced())
                placeBarrier(barriers[1]);
        }
    }

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
}
