using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAI : MonoBehaviour
{
    [SerializeField] private Tower tower;
    [SerializeField] private GameObject nozzle;

    [SerializeField] private GameObject bullet;
    [SerializeField] private Bullet bulletScript;

    private GameObject target = null;
    private GameManager gameManager;
    private Animator anim;

    private void Start()
    {
        //Play setting-up animation
        nozzle = Instantiate(nozzle, gameObject.transform.position, Quaternion.identity, gameObject.transform);
        anim = nozzle.GetComponent<Animator>();

        gameManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<GameManager>();
        
        if (gameManager.getGameState() == GameManager.GameState.Defend || gameManager.getGameState() == GameManager.GameState.BossRound)
        {
            newTarget();
            StartRound();
        }
    }

    private void Update()
    {
        //if (Input.GetKeyDown("x")) { place(); }

        if (target != null)
        {
            targetPoint();
        }

        if (gameManager.getGameState() == GameManager.GameState.BossRound)
        {
            newTarget();    //Constantly target the closest enemy in a boss round
        }
    }

    public void place(Tower towerType)
    {
        tower = towerType;
        bulletScript = tower.getDefaultBullet();
        

        nozzle.GetComponent<Animator>().runtimeAnimatorController = tower.getAnim();
    }

    public void StartRound()
    {
        if (target == null)
            newTarget();
        StartCoroutine(firing());
    }

    private void targetPoint()
    {
        nozzle.transform.rotation = Quaternion.Lerp(nozzle.transform.rotation, pointAtTarget(), tower.getTurnSpeed());
    }

    private Quaternion pointAtTarget()
    {
        Vector3 offset = target.transform.position - transform.position;
        Quaternion output = Quaternion.LookRotation(Vector3.forward, offset).normalized;
        return output;

    }

    private IEnumerator firing()
    {
        if (gameManager.getGameState() == GameManager.GameState.Defend || gameManager.getGameState() == GameManager.GameState.BossRound)
        {
            newTarget();
            new WaitForSeconds(0.0833f);
            anim.SetTrigger("TargetFound");
        }
        while (target != null && target.active)
        {
            var boolet = Instantiate(bullet, nozzle.transform.position, nozzle.transform.rotation);
            yield return new WaitForEndOfFrame();
            boolet.SendMessage("setBullet", bulletScript);
            boolet.SendMessage("Mult", tower.getDamageMult());

            yield return new WaitForSeconds(tower.getFireRate());
        }
        target = null;
        if (gameManager.getGameState() == GameManager.GameState.Defend || gameManager.getGameState() == GameManager.GameState.BossRound)
            StartCoroutine(firing());
        anim.SetTrigger("TargetLost");
    }

    private void enemyKilled()
    {
        if (gameManager.getGameState() == GameManager.GameState.Defend || gameManager.getGameState() == GameManager.GameState.BossRound)
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
            Vector3 diff = enemy.transform.position - nozzle.transform.position;
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
}
