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
        direction();
    }

    private void direction()
    {
        try
        {
            movingX = ai.getGoal().x - location.position.x;
            movingY = ai.getGoal().y - location.position.y;

        } catch (Exception e)
        {
            //Don't update if there's an error
        }

        Vector2 movementVec = new Vector2(movingX, movingY);
        movementVec = movementVec.normalized;

        //Debug.Log(movementVec);

        anim.SetFloat("moveX", movementVec.x);
        anim.SetFloat("moveY", movementVec.y);
    }
}
