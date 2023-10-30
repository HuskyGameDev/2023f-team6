using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 0.5f;
    public Rigidbody2D rb;
    private Vector2 input;

    Animator anim;
    private Vector2 lastMoveDirection;
    private bool facingLeft = true;
    private int stop = 1;

    //public Transform Aim;
    private Vector3 AimVector;
    bool isWalking = false;

    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        AimVector = new Vector3();
    }

    private void FixedUpdate() {
        ProccessInputs();
        rb.velocity = input * speed * Time.fixedDeltaTime * stop;
        
        if(isWalking) {
            AimVector = input;
        }
    }

    void ProccessInputs() {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        if(input.x == 0 && input.y == 0) {
            isWalking = false;
            anim.SetBool("Moving", false);
            //lastMoveDirection = input;
            //Vector3 vector3 = Vector3.left * lastMoveDirection.x + Vector3.down * lastMoveDirection.y;

            //AimVector = vector3;
            //Aim.rotation = Quaternion.LookRotation(Vector3.forward, vector3);
        }
        else {
            isWalking = true;
            anim.SetBool("Moving", true);
        }

        input.Normalize();

        if (isWalking)
        {
            anim.SetFloat("MoveX", input.x);
            anim.SetFloat("MoveY", input.y);
        }
    }

    public Vector3 getAim() { return AimVector; }
    public bool stopped()
    {
        if (stop == 0)
            return false;
        return true;
    }
    public void Stop() { stop = 0; }
    public void resume() { stop = 1; }
}
