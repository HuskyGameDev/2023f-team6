using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEBuff : MonoBehaviour
{
    private Buffs buff;
    private CircleCollider2D collider;

    private void setBuff(Buffs buff) 
    { 
        this.buff = buff;
        collider = gameObject.GetComponent<CircleCollider2D>();
    }
    private void setRadius(float radius) { collider.radius = radius; }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);
        if (collision.CompareTag("Enemy"))
            collision.SendMessage("InflictDebuff", buff);
    }

}
