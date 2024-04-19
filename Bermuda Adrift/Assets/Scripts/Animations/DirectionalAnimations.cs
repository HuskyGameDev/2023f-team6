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
    bool hasMoveX()
    {
        foreach (AnimatorControllerParameter param in anim.parameters)
        {
            if (param.name == "MoveX")
                return true;
        }
        return false;
    }

    private void direction()
    {
        try
        {
            movingX = ai.getGoal().x - location.position.x;
            movingY = ai.getGoal().y - location.position.y;

        } catch (Exception)
        {
            //Don't update if there's an error
        }

        Vector2 movementVec = new Vector2(movingX, movingY);
        movementVec = movementVec.normalized;

        if (hasMoveX())
        {
            anim.SetFloat("MoveX", movementVec.x);
            anim.SetFloat("MoveY", movementVec.y);
        } 
        else
        {
            if (movementVec.x < 0)
                gameObject.GetComponent<SpriteRenderer>().flipX = true;
            else
                gameObject.GetComponent<SpriteRenderer>().flipX = false;

            string[] opposites = { "Hamster" };
            foreach (string name in opposites)
            {
                if (gameObject.GetComponent<AI>().getName().CompareTo(name) == 0)
                    gameObject.GetComponent<SpriteRenderer>().flipX = !gameObject.GetComponent<SpriteRenderer>().flipX;
            }
        }
    }
}
