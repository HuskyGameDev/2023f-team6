using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{

    //melee attacks
    private GameObject[] Melees;
    float atkDuration = 0.3f;
    float atkTimer = 0f;


    //ranged attacks
    //public Transform Aim;
    public GameObject bulletPrefab;
    public Bullet primaryBullet;
    [SerializeField] private Bullet secondaryBullet;
    [SerializeField] private Buffs selfBuff;
    [SerializeField] private Bullet specialBullet;

    private Animator anim;
    private Movement movement;
    private bool primaryOnCooldown;

    private bool secondaryOnCooldown;
    float secondaryCooldown = 0.25f;

    private bool buffOnCooldown;
    private float buffCooldown = 5f;

    private bool specialOnCooldown;
    private float specialCooldown = 10f;

    private Buffs[] buffs;

    private void Start()
    {
        Melees = new GameObject[8];
        buffs = new Buffs[10];

        for (int i = 0; i < 8; i++)
        {
            Melees[i] = gameObject.transform.GetChild(i).gameObject;
            Melees[i].SetActive(false);
        }

        movement = gameObject.GetComponent<Movement>();
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("Managers").GetComponent<GameManager>().getGameState() != GameManager.GameState.Defend && GameObject.Find("Managers").GetComponent<GameManager>().getGameState() != GameManager.GameState.BossRound)
            return;

        anim.speed = getAttackSpeed();

        if (Input.GetMouseButton(0))
        {
            anim.SetTrigger("Primary");
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (!secondaryOnCooldown)
                anim.SetTrigger("Secondary");
        }
        else if (Input.GetKeyDown("q"))
        {
            if (!buffOnCooldown)
                anim.SetTrigger("Buff");
        }
        else if (Input.GetKeyDown("r"))
        {
            if (!specialOnCooldown)
                anim.SetTrigger("Special");
        }
        else
            anim.ResetTrigger("Primary");

        if (!movement.stopped())
        {
            anim.SetFloat("AttackDirectionX", movement.getAim().x);
            anim.SetFloat("AttackDirectionY", movement.getAim().y);
        }
    }

    public void shoot(int slot) { StartCoroutine(OnShoot(slot)); }
    private IEnumerator OnShoot(int slot) {
        if (slot == 1 && !secondaryOnCooldown) {

            secondaryOnCooldown = true;

            Vector3 offset = gameObject.GetComponent<Movement>().getAim();
            Quaternion output = Quaternion.LookRotation(Vector3.forward, offset).normalized;
            output = Quaternion.Euler(output.eulerAngles / 2);

            GameObject intBullet = Instantiate(bulletPrefab, gameObject.transform.position, output);
            intBullet.SendMessage("setBullet", secondaryBullet);
            intBullet.SendMessage("Mult", getDamageMult());

            yield return new WaitForSeconds(secondaryCooldown * getCooldowns());
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
            intBullet.SendMessage("Mult", getDamageMult());

            yield return new WaitForSeconds(specialCooldown * getCooldowns());
            specialOnCooldown = false;
        }
    }

    public void startBuff() 
    {
        StartCoroutine(Buff(selfBuff));
        movement.buff(selfBuff);
    }

    private IEnumerator Buff(Buffs buff)
    {
        buffOnCooldown = true;
        addBuff(buff);

        yield return new WaitForSeconds(buff.getDuration());

        removeBuff(buff);

        if (buffCooldown * getCooldowns() > buff.getDuration())
            yield return new WaitForSeconds(buffCooldown * getCooldowns() - buff.getDuration());

        buffOnCooldown = false;
    }

    public void OnAttack(int i) {
        Melees[i].GetComponent<Weapon>().damageMult(getDamageMult());
        Melees[i].SetActive(true);
        //Set animation trigger
    }

    public void attackFinish() 
    {
        for (int i = 0; i < Melees.Length; i++)
        {
            Melees[i].SetActive(false);
        }
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
                for (int j = i; j < buffs.Length - 1; j++)
                {
                    buffs[i] = buffs[i + 1];
                }
                buffs[buffs.Length - 1] = null;
                return;
            }
        }
    }

    private float getCooldowns()
    {
        float cooldowns = 1;
        for (int i = 0; buffs[i] != null && i < buffs.Length; i++)
        {
            cooldowns *= buffs[i].getCooldowns();
        }
        return cooldowns;
    }
    private float getAttackSpeed()
    {
        float attackSpeed = 1;
        for (int i = 0; buffs[i] != null && i < buffs.Length; i++)
        {
            attackSpeed *= buffs[i].getAttackSpeed();
        }
        return attackSpeed;
    }
    private float getDamageMult()
    {
        float mult = 1;
        for (int i = 0; buffs[i] != null && i < buffs.Length; i++)
        {
            mult *= buffs[i].getDamage();
        }
        return mult;
    }
}
