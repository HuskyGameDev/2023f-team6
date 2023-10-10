using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOE : MonoBehaviour
{
    private CircleCollider2D area;
    private int effect;
    private int damage;

    private void Start()
    {
        area = gameObject.GetComponent<CircleCollider2D>();
    }

    public IEnumerator AOEHit(float[] inputs)
    {
        //Inputs[0] is radius
        //Inputs[1] is duration
        //Inputs[2] is effect
        //Inputs[3] is damage

        Debug.Log("Received AOE");
        area.radius = inputs[0];
        effect = (int) inputs[2];
        damage = (int) inputs[3];

        Debug.Log(inputs[1]);
        if (inputs[1] != 0)
            yield return new WaitForSeconds(inputs[1]);
        else
            yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        Destroy(gameObject);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (effect == 0)
        {
            collision.gameObject.SendMessage("TakeDamage", damage);
        }
    }
}
