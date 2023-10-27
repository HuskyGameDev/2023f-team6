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
    public Bullet primaryBullet;
    [SerializeField] private Bullet secondaryBullet;
    [SerializeField] private Buffs buff;
    [SerializeField] private Bullet specialBullet;


    private bool primaryOnCooldown;

    private bool secondaryOnCooldown;
    float secondaryCooldown = 0.25f;

    private bool buffOnCooldown;
    private float buffCooldown = 5f;

    private bool specialOnCooldown;
    private float specialCooldown = 10f;

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

        if(Input.GetMouseButtonDown(0)) {
            StartCoroutine(OnAttack());
        }
        else if(Input.GetMouseButtonDown(1)) {
            StartCoroutine(OnShoot(1));
        }
        else if (Input.GetKeyDown("q"))
        {
            StartCoroutine(Buff());
        }
        else if (Input.GetKeyDown("r"))
        {
            StartCoroutine(OnShoot(3));
        }
    }

    IEnumerator OnShoot(int slot) {

        if(slot == 1 && !secondaryOnCooldown) {
            secondaryOnCooldown = true;

            Vector3 offset = gameObject.GetComponent<Movement>().getAim();
            Quaternion output = Quaternion.LookRotation(Vector3.forward, offset).normalized;
            output = Quaternion.Euler(output.eulerAngles / 2);

            GameObject intBullet = Instantiate(bulletPrefab, gameObject.transform.position, output);
            intBullet.SendMessage("setBullet", secondaryBullet);

            yield return new WaitForSeconds(secondaryCooldown);
            secondaryOnCooldown = false;
        } 
        else if (slot == 3 && !specialOnCooldown)
        {
            specialOnCooldown = true;

            Vector3 offset = gameObject.GetComponent<Movement>().getAim();
            Quaternion output = Quaternion.LookRotation(Vector3.forward, offset).normalized;
            output = Quaternion.Euler(output.eulerAngles / 2);

            GameObject intBullet = Instantiate(bulletPrefab, gameObject.transform.position, output);
            intBullet.SendMessage("setBullet", specialBullet);

            yield return new WaitForSeconds(secondaryCooldown);
            specialOnCooldown = false;
        }
    }

    IEnumerator Buff()
    {
        if (!buffOnCooldown)
        {
            buffOnCooldown = true;
            gameObject.SendMessage("buff", buff);

            yield return new WaitForSeconds(buffCooldown);
            buffOnCooldown = false;
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
