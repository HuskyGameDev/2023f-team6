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
        movingX = location.position.x - ai.getGoal().position.x;
        movingY = location.position.y - ai.getGoal().position.y;

        Vector2 movementVec = new Vector2(movingX, movingY);
        movementVec = movementVec.normalized;

        anim.SetFloat("moveX", movementVec.x);
        anim.SetFloat("moveY", movementVec.y);
    }
}
