using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridges : MonoBehaviour
{

    Color tmp;
    // Start is called before the first frame update
    void Start()
    {
        tmp = GetComponent<SpriteRenderer>().color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("This worked");
        if(collision.CompareTag("Enemy"))
        {
            tmp.a = 0.5f;
            GetComponent<SpriteRenderer>().color = tmp;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        tmp.a = 1;
        GetComponent<SpriteRenderer>().color = tmp;
    }
}
