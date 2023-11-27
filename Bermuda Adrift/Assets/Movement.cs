using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float speed;
    private Rigidbody2D rb;
    private Vector2 input;

    Animator anim;
    private float stop = 1;

    //public Transform Aim;
    private Vector3 AimVector;
    bool isWalking = false;

    private Buffs[] buffs;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        AimVector = new Vector3();
    }

    private void FixedUpdate() {
        ProccessInputs();
        move();

        //Update Aim to reflect mouse position
        var mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0f;
        AimVector = mouseWorldPosition - gameObject.transform.position;

        //Update movex and movey based on AIM
        anim.SetFloat("MoveX", AimVector.x);
        anim.SetFloat("MoveY", AimVector.y);
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
    }
    private void move()
    {
        int layerMask = 1 << 4;
        float xDirection = 1;
        if (input.x < 0)
            xDirection = -1;
        float yDirection = 1;
        if (input.y < 0)
            yDirection = -1;


        if (Physics.Raycast(transform.position + (Vector3.down * 0.58f), Vector3.forward * xDirection, (input * speed * getSpeed() * Time.fixedDeltaTime * stop).x, layerMask))
            xDirection = 0;
        else
            xDirection = input.x;

        if (Physics.Raycast(transform.position + (Vector3.down * 0.58f), Vector3.up * yDirection, (input * speed * getSpeed() * Time.fixedDeltaTime * stop).y, layerMask))
            yDirection = 0;
        else
            yDirection = input.y;

        //transform.position = transform.position + new Vector3(xDirection, yDirection) * speed * getSpeed() * Time.fixedDeltaTime * stop;
        rb.velocity = new Vector2(xDirection, yDirection) * speed * getSpeed() * Time.fixedDeltaTime * stop;
    }

    public Vector3 getAim() { return AimVector; }
    public bool stopped()
    {
        if (stop < 1)
            return false;
        return true;
    }
    public void Stop() { stop = 0.25f; }
    public void fullStop() { stop = 0f; }
    public void resume() { stop = 1; }


    public void buff(Buffs buff) 
    {
        if (buffs == null)
            buffs = buffs = new Buffs[10];
        StartCoroutine(selfBuff(buff)); 
    }
    private IEnumerator selfBuff(Buffs buff)
    {
        addBuff(buff);

        yield return new WaitForSeconds(buff.getDuration());

        removeBuff(buff);
    }
    private void addBuff(Buffs buff)
    {
        for (int i = 0; i < buffs.Length; i++)
        {
            if (buffs[i] == null)
            {
                buffs[i] = buff;
                return;
            }
        }
    }
    private void removeBuff(Buffs buff)
    {
        for (int i = 0; i < buffs.Length; i++)
        {
            if (buffs[i] == buff)
            {
                buffs[i] = null;
            }
        }
    }

    private float getSpeed()
    {
        float speed = 1;
        for (int i = 0; i < buffs.Length; i++)
        {
            if (buffs[i] != null)
                speed *= buffs[i].getSpeed();
        }
        return speed;
    }
}
