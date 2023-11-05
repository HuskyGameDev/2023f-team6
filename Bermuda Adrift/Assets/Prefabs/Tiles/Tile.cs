using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    SpriteRenderer sR;
    // Start is called before the first frame update
    void Start()
    {
        sR = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    private void OnMouseEnter()
    {
        sR.color = new Vector4(0, 1, 0, 1);
    }

    private void OnMouseExit()
    {
        sR.color = new Vector4(0, 0, 0, 0.5f);
    }
}
