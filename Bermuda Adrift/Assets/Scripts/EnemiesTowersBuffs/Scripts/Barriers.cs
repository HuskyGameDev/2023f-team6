using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barriers : MonoBehaviour
{
    [SerializeField] private BarrierScriptable barrier; //Won't need to be serialized after the placing is set up
    private int health;
    private float armor = 1;
    private Buffs debuff;
    
    // Start is called before the first frame update
    void Start()
    {
        health = barrier.getHealth();
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
        debuff = barrier.getDebuff();
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

    private IEnumerator InflictDebuff(Buffs debuff)
    {
        float startTime = Time.time;
        armor = debuff.getArmor();

        while (Time.time < startTime + debuff.getDuration()) //Even if no DOT, still waits until the end of the duration
        {
            takeDamage(debuff.getDOT());

            yield return new WaitForSeconds(debuff.getDOTSpeed());
        }

        armor = 1;
    }
    public BarrierScriptable.Effect getEffect() { return barrier.getEffect(); }
    public Buffs getDebuff() { return debuff; }
}
