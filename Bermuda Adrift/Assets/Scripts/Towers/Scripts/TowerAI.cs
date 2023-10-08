using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAI : MonoBehaviour
{
    [SerializeField] private Tower tower;
    [SerializeField] private Transform pivot;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform nozzle;
    private int health;
    private GameObject target = null;
    [SerializeField] private GameManager gameManager;

    private void Start()
    {
        health = tower.getHealth();

        if (gameObject.tag != "Barrier") // && gameManager.getGameState() == GameManager.GameState.Defend)
        {
            newTarget();
            StartRound();
        }
    }

    private void Update()
    {
        //if (Input.GetKeyDown("b")) { place(); }

        if (gameObject.tag != "Barrier" && target != null)
        {
            targetPoint();
        }
    }

    public void place()
    {

    }

    private void StartRound()
    {
        StartCoroutine(firing());
    }

    private void targetPoint()
    {
        pivot.rotation = Quaternion.Lerp(pivot.rotation, pointAtTarget(), tower.getTurnSpeed());
        //pivot.Rotate(Quaternion.Lerp(pivot.rotation, pointAtTarget(), tower.getTurnSpeed()).eulerAngles);
    }

    private Quaternion pointAtTarget()
    {
        Vector3 offset = target.transform.position - transform.position;
        Quaternion output = Quaternion.LookRotation(Vector3.forward, offset);
        return output;

    }

    private IEnumerator firing()
    {
        if (gameManager.getGameState() == GameManager.GameState.Defend)
        {
            newTarget();
        }
        while (target != null && target.active)
        {
            var boolet = Instantiate(bullet, nozzle.position, nozzle.rotation);
            yield return new WaitForEndOfFrame();
            boolet.SendMessage("Mult", tower.getDamageMult());

            yield return new WaitForSeconds(tower.getFireRate());
        }
        target = null;
    }

    private void enemyKilled()
    {
        if (gameManager.getGameState() == GameManager.GameState.Defend)
        {
            newTarget();
        }
        else
            target = null; 
    }

    private void newTarget()    //Set target to closest enemy
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        float distance = Mathf.Infinity;
        foreach (GameObject enemy in enemies)
        {
            Vector3 diff = enemy.transform.position - pivot.position;
            float curDistance = diff.sqrMagnitude;
            if (tower.getRange() > 0)
            {
                if (curDistance < distance && curDistance < tower.getRange())
                {
                    target = enemy;
                    distance = curDistance;
                }
            } else
            {
                if (curDistance < distance)
                {
                    target = enemy;
                    distance = curDistance;
                }
            }
        }
    }
    public void barrierDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
