using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barriers : MonoBehaviour
{
    [SerializeField] private BarrierScriptable barrier;
    private int health;
    
    // Start is called before the first frame update
    void Start()
    {
        health = barrier.getHealth();
        Locate();
    }

    // Update is called once per frame
    void Update()
    {
        //While not placed, call Locate()
    }

    private void setBarrier(BarrierScriptable newBarrier)
    {
        health = barrier.getHealth();
    }

    private void takeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            //Play destroyed animation
            Destroy(gameObject);
        }
    }

    private void Locate()   //Changes the animation based on which channel the barrier is in
    {
        if (Mathf.Abs(gameObject.transform.position.x) < 3)
        {
            if (gameObject.transform.position.y > 0)
            {
                //North Animation
            } 
            else
            {
                //South Animation
            }
        } else
        {
            if (gameObject.transform.position.x < 0)
            {
                //Left Animation
            }
            else
            {
                //Right Animation
            }
        }
    }
    public BarrierScriptable.Effect getEffect() { return barrier.getEffect(); }
}
