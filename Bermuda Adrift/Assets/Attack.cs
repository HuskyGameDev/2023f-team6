using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{

    //melee attacks
    private GameObject[] Melees;
    bool isAttacking = false;
    float atkDuration = 0.3f;
    float atkTimer = 0f;


    //ranged attacks
    //public Transform Aim;
    public GameObject bulletPrefab;
    public Bullet bullet;
    public float fireForce = 10f;
    float shootCooldown = 0.25f;
    float shootTimer = 0.5f;

    private void Start()
    {
        Melees = new GameObject[8];
        for (int i = 0; i < 8; i++)
        {
            Melees[i] = gameObject.transform.GetChild(i).gameObject;
            Melees[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("Managers").GetComponent<GameManager>().getGameState() != GameManager.GameState.Defend && GameObject.Find("Managers").GetComponent<GameManager>().getGameState() != GameManager.GameState.BossRound)
            return;
        //CheckMeleeTimer();
        shootTimer += Time.deltaTime;

        if(Input.GetMouseButtonDown(0)) {
            StartCoroutine(OnAttack());
        }
        if(Input.GetMouseButtonDown(1)) {
            OnShoot();
        }
    }

    void OnShoot() {
        if(shootTimer > shootCooldown) {
            shootTimer = 0;

            Vector3 offset = gameObject.GetComponent<Movement>().getAim();
            Quaternion output = Quaternion.LookRotation(Vector3.forward, offset).normalized;
            output = Quaternion.Euler(output.eulerAngles / 2);

            GameObject intBullet = Instantiate(bulletPrefab, gameObject.transform.position, output);
            intBullet.SendMessage("setBullet", bullet);
            //intBullet.GetComponent<Rigidbody2D>().AddForce(-Aim.up * fireForce, ForceMode2D.Impulse);
            //Destroy(intBullet, 2f);
        }
    }

    IEnumerator OnAttack() {
        if(!isAttacking) {
            int i = directionalHitbox();
            Melees[i].SetActive(true);
            isAttacking = true;
            //Set animation trigger

            yield return new WaitForSeconds(atkDuration);

            isAttacking = false;
            Melees[i].SetActive(false);
        }
    }

    void CheckMeleeTimer() {
        if (isAttacking) {
            atkTimer += Time.deltaTime;
            if(atkTimer >= atkDuration) {
                atkTimer = 0;
                isAttacking = false;
                Melees[directionalHitbox()].SetActive(false);
            }
        }
    }

    private int directionalHitbox() //Will probably link the hitboxes to animations later
    {
        Vector3 aim = gameObject.GetComponent<Movement>().getAim();

        if (aim.x == 0 && aim.y < 0)        //Down Hitbox
            return 0;
        else if (aim.x < 0 && aim.y < 0)    //Down Left Hitbox
            return 1;
        else if (aim.x < 0 && aim.y == 0)   //Left Hitbox
            return 2;
        else if (aim.x < 0 && aim.y > 0)    //Up Left Hitbox
            return 3;
        else if (aim.x == 0 && aim.y > 0)   //Up Hitbox
            return 4;
        else if (aim.x > 0 && aim.y > 0)    //Up Right Hitbox
            return 5;
        else if (aim.x > 0 && aim.y == 0)   //Right Hitbox
            return 6;
        else if (aim.x > 0 && aim.y < 0)    //Down Right Hitbox
            return 7;
        else        //Start facing down
            return 0;
    }
}
