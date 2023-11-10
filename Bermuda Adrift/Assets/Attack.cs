using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Attack : MonoBehaviour
{
    public static event Action<float, CooldownIndicator.position> updateCooldowns;

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
    private float primaryCooldown;

    private bool secondaryOnCooldown;
    private float secondaryCooldown = 1f;

    private bool buffOnCooldown;
    private float buffCooldown = 6f;

    private bool specialOnCooldown;
    private float specialCooldown = 10f;

    private int buffer;

    private Buffs[] buffs;

    private void OnEnable()
    {
        CooldownIndicator.cooldownComplete += cooldownFinished;
    }
    private void OnDisable()
    {
        CooldownIndicator.cooldownComplete -= cooldownFinished;
    }
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

        if (Input.GetMouseButton(0) && !IsPointerOverUIObject())
        {
            primary();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            secondary();
        }
        else if (Input.GetKeyDown("q"))
        {
            utility();
        }
        else if (Input.GetKeyDown("r"))
        {
            special();
        }
        else
            anim.ResetTrigger("Primary");

        if (primaryOnCooldown)
            updateCooldowns?.Invoke(primaryCooldown * getCooldowns(), CooldownIndicator.position.primary);
        if (secondaryOnCooldown)
            updateCooldowns?.Invoke(secondaryCooldown * getCooldowns(), CooldownIndicator.position.secondary);
        if (buffOnCooldown)
            updateCooldowns?.Invoke(buffCooldown * getCooldowns(), CooldownIndicator.position.utility);
        if (specialOnCooldown)
            updateCooldowns?.Invoke(specialCooldown * getCooldowns(), CooldownIndicator.position.special);
    }
    private void primary() 
    {
        anim.SetFloat("AttackDirectionX", movement.getAim().x);
        anim.SetFloat("AttackDirectionY", movement.getAim().y);

        anim.SetTrigger("Primary");
    }
    private void secondary()
    {
        if (!secondaryOnCooldown)
        {
            anim.SetFloat("AttackDirectionX", movement.getAim().x);
            anim.SetFloat("AttackDirectionY", movement.getAim().y);

            secondaryOnCooldown = true;
            anim.SetTrigger("Secondary");
        }
    }
    private void utility()
    {
        if (!buffOnCooldown)
        {
            anim.SetFloat("AttackDirectionX", movement.getAim().x);
            anim.SetFloat("AttackDirectionY", movement.getAim().y);

            buffOnCooldown = true;
            anim.SetTrigger("Buff");
        }
    }
    private void special()
    {
        if (!specialOnCooldown)
        {
            anim.SetFloat("AttackDirectionX", movement.getAim().x);
            anim.SetFloat("AttackDirectionY", movement.getAim().y);

            specialOnCooldown = true;
            anim.SetTrigger("Special");
        }
    }

    public void shoot(int slot) { OnShoot(slot); }
    private void OnShoot(int slot) {
        if (slot == 1) {
            Vector3 offset = new Vector3(anim.GetFloat("AttackDirectionX"), anim.GetFloat("AttackDirectionY"));
            Quaternion output = Quaternion.LookRotation(Vector3.forward, offset).normalized;
            output = Quaternion.Euler(output.eulerAngles / 2);

            GameObject intBullet = Instantiate(bulletPrefab, gameObject.transform.position, output);
            intBullet.SendMessage("setBullet", secondaryBullet);
            intBullet.SendMessage("Mult", getDamageMult());
        } 
        else if (slot == 3)
        {
            Vector3 offset = new Vector3(anim.GetFloat("AttackDirectionX"), anim.GetFloat("AttackDirectionY"));
            Quaternion output = Quaternion.LookRotation(Vector3.forward, offset).normalized;
            output = Quaternion.Euler(output.eulerAngles / 2);

            GameObject intBullet = Instantiate(bulletPrefab, gameObject.transform.position, output);
            intBullet.SendMessage("setBullet", specialBullet);
            intBullet.SendMessage("Mult", getDamageMult());
        }
    }

    public void startBuff() 
    {
        StartCoroutine(Buff(selfBuff));
        movement.buff(selfBuff);
    }

    private IEnumerator Buff(Buffs buff)
    {
        addBuff(buff);

        yield return new WaitForSeconds(buff.getDuration());

        removeBuff(buff);
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

    public void cooldownFinished(int type)
    {
        if (type == 0)
            primaryOnCooldown = false;
        else if (type == 1)
            secondaryOnCooldown = false;
        else if (type == 2)
            buffOnCooldown = false;
        else if (type == 3)
            specialOnCooldown = false;
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
                return;
            }
        }
    }

    private float getCooldowns()
    {
        float cooldowns = 1;
        for (int i = 0; i < buffs.Length; i++)
        {
            if (buffs[i] != null)
                cooldowns *= buffs[i].getCooldowns();
        }
        return cooldowns;
    }
    private float getAttackSpeed()
    {
        float attackSpeed = 1;
        for (int i = 0; i < buffs.Length; i++)
        {
            if (buffs[i] != null)
                attackSpeed *= buffs[i].getAttackSpeed();
        }
        return attackSpeed;
    }
    private float getDamageMult()
    {
        float mult = 1;
        for (int i = 0; i < buffs.Length; i++)
        {
            if (buffs[i] != null)
                mult *= buffs[i].getDamage();
        }
        return mult;
    }
    private bool IsPointerOverUIObject()
    {
        var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
