using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private float speed;
    private Vector2 input;

    Animator anim;
    private float stop = 1;

    //public Transform Aim;
    private Vector3 AimVector;

    private Buffs[] buffs;

    // Start is called before the first frame update
    void Start()
    {
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
            anim.SetBool("Moving", false);
            //lastMoveDirection = input;
            //Vector3 vector3 = Vector3.left * lastMoveDirection.x + Vector3.down * lastMoveDirection.y;

            //AimVector = vector3;
            //Aim.rotation = Quaternion.LookRotation(Vector3.forward, vector3);
        }

        input.Normalize();
    }
    private void move()
    {
        int layerMask = 1 << 4;
        int channelMask = 1 << 6;
        int bridgeMask = 1 << 7;

        float xDirection = 1;
        if (input.x < 0)
            xDirection = -1;
        float yDirection = 1;
        if (input.y < 0)
            yDirection = -1;

        //if going to hit water or channel and not on a bridge, don't move that direction
        if ((Physics2D.Raycast(transform.position + (Vector3.down * 0.58f) + (Vector3.right * 0.3357f * xDirection), Vector3.right * xDirection, (input * speed * getSpeed() * Time.fixedDeltaTime * stop).x, layerMask).collider != null
            ||
            Physics2D.Raycast(transform.position + (Vector3.down * 0.58f) + (Vector3.right * 0.3357f * xDirection), Vector3.right * xDirection, (input * speed * getSpeed() * Time.fixedDeltaTime * stop).x, channelMask).collider != null)
            &&
            Physics2D.Raycast(transform.position + (Vector3.down * 0.58f) + (Vector3.right * 0.3357f * xDirection), Vector3.right * xDirection, (input * speed * getSpeed() * Time.fixedDeltaTime * stop).x, bridgeMask).collider == null)
        {
            xDirection = 0;
        }
        else
            xDirection = input.x;

        if ((Physics2D.Raycast(transform.position + (Vector3.down * 0.58f) + (Vector3.up * 0.09779f * yDirection), Vector3.up * yDirection, (input * speed * getSpeed() * Time.fixedDeltaTime * stop).y, layerMask).collider != null
            ||
            Physics2D.Raycast(transform.position + (Vector3.down * 0.58f) + (Vector3.up * 0.09779f * yDirection), Vector3.up * yDirection, (input * speed * getSpeed() * Time.fixedDeltaTime * stop).y, channelMask).collider != null)
            &&
            Physics2D.Raycast(transform.position + (Vector3.down * 0.58f) + (Vector3.up * 0.09779f * yDirection), Vector3.up * yDirection, (input * speed * getSpeed() * Time.fixedDeltaTime * stop).y, bridgeMask).collider == null)
        {
            yDirection = 0;
        }
        else
            yDirection = input.y;

        if (xDirection == 0 && yDirection == 0) anim.SetBool("Moving", false);
        else anim.SetBool("Moving", true);
        //transform.position = transform.position + new Vector3(xDirection, yDirection) * speed * getSpeed() * Time.fixedDeltaTime * stop;
        transform.position += new Vector3(xDirection, yDirection) * speed * getSpeed() * Time.fixedDeltaTime * stop;
        //rb.velocity = new Vector2(xDirection, yDirection) * speed * getSpeed() * Time.fixedDeltaTime * stop;
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
    public void resume() 
    { 
        stop = 1;
    }


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

    public void setSpeed(float speed) { this.speed = speed; }
    private float getSpeed()
    {
        if (buffs == null) return 1;

        float speed = 1;
        for (int i = 0; i < buffs.Length; i++)
        {
            if (buffs[i] != null)
                speed *= buffs[i].getSpeed();
        }
        return speed;
    }

    public bool isIdle()
    {
        int layerMask = 1 << 4;
        int channelMask = 1 << 6;
        int bridgeMask = 1 << 7;

        float xDirection = 1;
        if (input.x < 0)
            xDirection = -1;
        float yDirection = 1;
        if (input.y < 0)
            yDirection = -1;

        //if going to hit water or channel and not on a bridge, don't move that direction
        if ((Physics2D.Raycast(transform.position + (Vector3.down * 0.58f) + (Vector3.right * 0.3357f * xDirection), Vector3.right * xDirection, (input * speed * getSpeed() * Time.fixedDeltaTime * stop).x, layerMask).collider != null
            ||
            Physics2D.Raycast(transform.position + (Vector3.down * 0.58f) + (Vector3.right * 0.3357f * xDirection), Vector3.right * xDirection, (input * speed * getSpeed() * Time.fixedDeltaTime * stop).x, channelMask).collider != null)
            &&
            Physics2D.Raycast(transform.position + (Vector3.down * 0.58f) + (Vector3.right * 0.3357f * xDirection), Vector3.right * xDirection, (input * speed * getSpeed() * Time.fixedDeltaTime * stop).x, bridgeMask).collider == null)
        {
            xDirection = 0;
        }
        else
            xDirection = input.x;

        if ((Physics2D.Raycast(transform.position + (Vector3.down * 0.58f) + (Vector3.up * 0.09779f * yDirection), Vector3.up * yDirection, (input * speed * getSpeed() * Time.fixedDeltaTime * stop).y, layerMask).collider != null
            ||
            Physics2D.Raycast(transform.position + (Vector3.down * 0.58f) + (Vector3.up * 0.09779f * yDirection), Vector3.up * yDirection, (input * speed * getSpeed() * Time.fixedDeltaTime * stop).y, channelMask).collider != null)
            &&
            Physics2D.Raycast(transform.position + (Vector3.down * 0.58f) + (Vector3.up * 0.09779f * yDirection), Vector3.up * yDirection, (input * speed * getSpeed() * Time.fixedDeltaTime * stop).y, bridgeMask).collider == null)
        {
            yDirection = 0;
        }
        else
            yDirection = input.y;

        if (xDirection != 0 || yDirection != 0)
            return false;

        if (!gameObject.GetComponent<Attack>().idleAttacks())
            return false;

        return true;
    }
}
