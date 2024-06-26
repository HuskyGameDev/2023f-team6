using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridges : MonoBehaviour
{

    Color tmp;


    private void OnEnable()
    {
        GameManager.onRoundEnd += opaque;
    }
    private void OnDisable()
    {
        GameManager.onRoundEnd -= opaque;
    }

    // Start is called before the first frame update
    void Start()
    {
        tmp = GetComponent<SpriteRenderer>().color;
    }

    public void transparent()
    {
        tmp.a = 0.5f;
        GetComponent<SpriteRenderer>().color = tmp;
    }
    public void opaque()
    {
        tmp.a = 1;
        GetComponent<SpriteRenderer>().color = tmp;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            tmp.a = 0.5f;
            GetComponent<SpriteRenderer>().color = tmp;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            tmp.a = 1;
            GetComponent<SpriteRenderer>().color = tmp;
        }
    }
}
