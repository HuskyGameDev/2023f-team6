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

    //public Transform Aim;
    private Vector3 AimVector;
    bool isWalking = false;

    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        AimVector = new Vector3();
    }

    // Update is called once per frame
    void Update()
    {
        //Animate();

        if (input.x < 0 && facingLeft || input.x > 0 && !facingLeft) {
            Flip();
        }
    }

    private void FixedUpdate() {
        ProccessInputs();
        rb.velocity = input * speed * Time.fixedDeltaTime;
        
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

        anim.SetFloat("MoveX", input.x);
        anim.SetFloat("MoveY", input.y);
    }

    void Animate() {
        
        anim.SetFloat("MoveMagnitude", input.magnitude);
        anim.SetFloat("LastMoveX", lastMoveDirection.x);
        anim.SetFloat("LastMoveY", lastMoveDirection.y);
    }

    void Flip() {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        facingLeft = !facingLeft;
    }

    public Vector3 getAim() { return AimVector; }
}
