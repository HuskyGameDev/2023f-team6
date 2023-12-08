using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetedAttacks : MonoBehaviour
{
    private AI target;
    private float initialOffset = 1;
    private float damage = 10;

    private void Start()
    {
        AI[] enemies = FindObjectsOfType<AI>();
        if (enemies.Length == 0) return;

        target = enemies[UnityEngine.Random.Range(0, enemies.Length)];

        transform.position = target.gameObject.transform.position;
        if (transform.position.x < 0)
        {
            transform.position -= Vector3.right * initialOffset;
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
            transform.position += Vector3.right * initialOffset;
    }

    public void Attack()
    {
        StartCoroutine(moveToAttack());
    }
    private IEnumerator moveToAttack()
    {
        while ((transform.position - target.transform.position).magnitude > 0.5)
        {
            transform.position = Vector3.Lerp(transform.position, target.transform.position, 0.75f);
            yield return new WaitForEndOfFrame();
        }

        target.SendMessage("TakeDamage", damage);   //Can be used as a damaging move against bosses, but can't crit
        target.SendMessage("Forget");   //Could be changed later if we decide to have something else that works like this

        Destroy(gameObject);
    }
    public void missionComplete()
    {
        Destroy(gameObject);
    }
}
