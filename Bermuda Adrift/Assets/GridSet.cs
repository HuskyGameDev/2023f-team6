using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridSet : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Tilemap sr;
    [SerializeField] private Color hoverColor;


    private GameObject towerBase;
    private Color startColor;

    // Start is called before the first frame update
    private void Start()
    {
        //startColor = sr.color;
    }

    private void OnMouseEnter()
    {
        //sr.color = hoverColor ;
    }

    private void OnMouseExit()
    {
        //sr.color = startColor;
    }

    /*private void OnMouseDown()
    {
        if (towerBase != null) return;

        GameObject towerToBuild = BuildManager.main.GetSelectedTower();
        towerBase = Instantiate(towerBase, transform.position, Quaternion.identity);
    
    }*/





    // Update is called once per frame
    void Update()
    {
        
    }
}
