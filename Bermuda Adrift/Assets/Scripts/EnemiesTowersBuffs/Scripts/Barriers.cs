using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barriers : MonoBehaviour
{
    [SerializeField] private BarrierScriptable barrier; //Won't need to be serialized after the placing is set up
    private int health;
    private float armor = 1;
    private Buffs[] debuffs;
    private Buffs debuffToInflict;
    
    // Start is called before the first frame update
    void Start()
    {
        health = barrier.getHealth();
        debuffs = new Buffs[5];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("b")) { setBarrier(barrier); }
        //While not placed (following mouse to be places), call Locate()
    }

    private void setBarrier(BarrierScriptable newBarrier)   //Sets up barrier. For now only sets the health and calls Locate()
    {
        barrier = newBarrier;
        health = barrier.getHealth();
        debuffToInflict = barrier.getDebuff();

        Locate();
    }

    private void takeDamage(int damage)
    {
        health -= (int) (damage * armor);
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

    private void addDebuff(Buffs debuff)    //Add a debuff to the list of debuffs
    {
        for (int i = 0; i < debuffs.Length; i++)
        {
            if (debuffs[i] == null)
            {
                debuffs[i] = debuff;
                return;
            }
        }
    }
    private void removeDebuff(Buffs debuff) //Removes a debuff from the list of debuffs currently applied
    {
        int i = 0;
        for (; i < debuffs.Length; i++)
        {
            if (debuffs[i] == debuff)
            {
                for (; i < debuffs.Length - 1; i++)
                {
                    debuffs[i] = debuffs[i + 1];
                }
                debuffs[i] = null;
            }
        }
    }
    private IEnumerator InflictDebuff(Buffs newDebuff)  //adds a debuff to the list
    {
        addDebuff(newDebuff);

        StartCoroutine(DOT(newDebuff.getDOT(), newDebuff.getDOTSpeed(), newDebuff.getDuration()));

        yield return new WaitForSeconds(newDebuff.getDuration());

        removeDebuff(newDebuff);
    }
    private IEnumerator DOT(int damage, float speed, float duration)    //Does [damage] damage every [speed] seconds for [duration] seconds
    {
        float startTime = Time.time;
        while (Time.time < startTime + duration) //Even if no DOT, still waits until the end of the duration
        {
            if (damage > 0)
                takeDamage(damage);

            yield return new WaitForSeconds(speed);
        }
    }


    public BarrierScriptable.Effect getEffect() { return barrier.getEffect(); }
    public Buffs getDebuff() { return debuffToInflict; }
}
