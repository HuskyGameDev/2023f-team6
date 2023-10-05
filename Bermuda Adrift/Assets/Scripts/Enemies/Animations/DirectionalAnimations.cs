using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalAnimations : MonoBehaviour
{
    private Animator anim;
    private AI ai;
    private Transform location;
    private float movingX = 0;
    private float movingY = 0;
    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        location = gameObject.transform;
        ai = gameObject.GetComponent<AI>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetInteger("Direction", direction());
    }

    private int direction()
    {
        movingX = location.position.x - ai.getGoal().position.x;
        movingY = location.position.y - ai.getGoal().position.y;

        if (Mathf.Abs(movingX) > Mathf.Abs(movingY))    //Pick between left and right
        {
            if (movingX < 0)
                return 1;   //Right
            return 0;       //Left
        }
        else                                            //Pick between up and down
        {
            if (movingY < 0)
                return 2;   //Up
            return 3;   //Down
        }
    }
}
