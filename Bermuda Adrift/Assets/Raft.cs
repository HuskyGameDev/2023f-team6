using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Raft : MonoBehaviour
{
    [SerializeField] GameObject tile;
    [SerializeField] GameObject tileParent;
    Tilemap tilemap;
    Grid grid;
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        //Debug.Log(tilemap.origin);

        if (tilemap == null)
            return;

        // tilemap position
        var tp = tilemap.transform.position;

        // bounds + offset
        var tBounds = tilemap.cellBounds;

        // corner points
        var c0 = new Vector3(tBounds.min.x + 1f, tBounds.min.y + 2f) + tp;
        var c1 = new Vector3(tBounds.min.x, tBounds.max.y) + tp;
        var c2 = new Vector3(tBounds.max.x, tBounds.max.y) + tp;
        var c3 = new Vector3(tBounds.max.x, tBounds.min.y) + tp;

        //grid = new Grid(tile, tileParent, tilemap.size.x / 2, tilemap.size.y / 2, 2f, c0);
        Instantiate(tile, tileParent.transform);
        var barrierTiles = Instantiate(tile, tileParent.transform);
        barrierTiles.GetComponent<Tile>().setType(1);
    }

    private void Update()
    {
        
    }

    void OnValidate()
    {
        if (tilemap == null)
            tilemap = GetComponent<Tilemap>();
    }

    void OnDrawGizmos()
    {
        Draw();
    }

    void Draw()
    {
        if (tilemap == null)
            return;

        // tilemap position
        var tp = tilemap.transform.position;

        // bounds + offset
        var tBounds = tilemap.cellBounds;

        // corner points
        var c0 = new Vector3(tBounds.min.x, tBounds.min.y) + tp;
        var c1 = new Vector3(tBounds.min.x, tBounds.max.y) + tp;
        var c2 = new Vector3(tBounds.max.x, tBounds.max.y) + tp;
        var c3 = new Vector3(tBounds.max.x, tBounds.min.y) + tp;

        // draw borders
        Debug.DrawLine(c0, c1, Color.red);
        Debug.DrawLine(c1, c2, Color.red);
        Debug.DrawLine(c2, c3, Color.red);
        Debug.DrawLine(c3, c0, Color.red);

        // draw origin cross
        Debug.DrawLine(new Vector3(tp.x, tBounds.min.y + tp.y), new Vector3(tp.x, tBounds.max.y + tp.y), Color.green);
        Debug.DrawLine(new Vector3(tBounds.min.x + tp.x, tp.y), new Vector3(tBounds.max.x + tp.x, tp.y), Color.green);
    }

    private void OnMouseOver()
    {

    }

    private void OnMouseDown()
    {

    }
}
