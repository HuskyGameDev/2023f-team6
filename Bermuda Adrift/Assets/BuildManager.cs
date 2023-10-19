using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager main;

    [Header("References")]
    [SerializeField] private GameObject[] towerPrefabs;

    private int selectedTower = 0;

    [SerializeField] private Tower testTower;

    public GameObject GetSelectedTower() { 
        return towerPrefabs[selectedTower];
    }

    public void placeTower(int selecction, Tower scriptable)
    {
        var tower = Instantiate(towerPrefabs[selecction]);

        //while (!tower.GetComponent<TowerAI>().getPlaced()) { }

        tower.SendMessage("place", scriptable);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("l"))
        {
            placeTower(0, testTower);
        }
    }
}
