using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Attack : MonoBehaviour
{
    public static event Action<float, CooldownIndicator.position> updateCooldowns;
    public static event Action<Sprite, CooldownIndicator.position> setThumbnails;

    [SerializeField] private Player character;

    //melee attacks
    private GameObject[] Melees1 = new GameObject[1];
    private GameObject[] Melees2 = new GameObject[1];
    private GameObject[] Melees3 = new GameObject[1];
    private GameObject[] Melees4 = new GameObject[1];

    private Animator anim;
    private Movement movement;
    private bool primaryOnCooldown;
    private float primaryCooldown;

    private bool secondaryOnCooldown;
    private float secondaryCooldown = 1f;

    private bool utilityOnCooldown;
    private float utilityCooldown = 6f;

    private bool specialOnCooldown;
    private float specialCooldown = 10f;

    private int direction;

    private Buffs[] buffs = new Buffs[10];

    private void OnEnable()
    {
        CooldownIndicator.cooldownComplete += cooldownFinished;
        CooldownIndicator.awoken += cooldownCreated;
    }
    private void OnDisable()
    {
        CooldownIndicator.cooldownComplete -= cooldownFinished;
    }
    private void Start()
    {
        movement = gameObject.GetComponent<Movement>();
        anim = gameObject.GetComponent<Animator>();

        setCharacter(character);
    }
    private void setCharacter(Player playerData)
    {
        character = playerData;
        anim.runtimeAnimatorController = playerData.getAnim();

        setAbility(playerData.getPrimary());
        setAbility(playerData.getSecondary());
        setAbility(playerData.getUtility());
        setAbility(playerData.getSpecial());
        if (playerData.getPassive() != null)
            StartCoroutine(Buff(playerData.getPassive()));
        movement.buff(playerData.getPassive());
    }

    private void setAbility(Ability ability)
    {
        if (ability.getAttackType() == Ability.attackType.melee)
        {
            GameObject hitboxes = null;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i) == ability.getHitboxes())
                {
                    hitboxes = transform.GetChild(i).gameObject;
                }
            }
            if (hitboxes == null)
            {
                hitboxes = Instantiate(ability.getHitboxes());
                hitboxes.transform.parent = gameObject.transform;
                hitboxes.transform.position = gameObject.transform.position;
            }
            

            if (ability.getSlot() == 1)
            {
                Melees1 = new GameObject[8];
                for (int i = 0; i < 8; i++)
                {
                    Melees1[i] = hitboxes.transform.GetChild(i).gameObject;
                    Melees1[i].SetActive(false);
                }
            }
            else if (ability.getSlot() == 2)
            {
                Melees2 = new GameObject[8];
                for (int i = 0; i < 8; i++)
                {
                    Melees2[i] = hitboxes.transform.GetChild(i).gameObject;
                    Melees2[i].SetActive(false);
                }
            }
            else if (ability.getSlot() == 3)
            {
                Melees3 = new GameObject[8];
                for (int i = 0; i < 8; i++)
                {
                    Melees3[i] = hitboxes.transform.GetChild(i).gameObject;
                    Melees3[i].SetActive(false);
                }
            }
            else if (ability.getSlot() == 4)
            {
                Melees4 = new GameObject[8];
                for (int i = 0; i < 8; i++)
                {
                    Melees4[i] = hitboxes.transform.GetChild(i).gameObject;
                    Melees4[i].SetActive(false);
                }
            }
        }

        //Set cooldown
        if (ability.getSlot() == 1)
        {
            primaryCooldown = ability.getCooldown();
            anim.SetBool("AltPrimary", character.altedP());
        }
        else if (ability.getSlot() == 2)
        {
            secondaryCooldown = ability.getCooldown();
            anim.SetBool("AltSecondary", character.altedS());
        }
        else if (ability.getSlot() == 3)
        {
            utilityCooldown = ability.getCooldown();
            anim.SetBool("AltUtility", character.altedU());
        }
        else if (ability.getSlot() == 4)
        {
            specialCooldown = ability.getCooldown();
            anim.SetBool("AltSpecial", character.altedSp());
        }
        else
        {
            primaryCooldown = ability.getCooldown();
            anim.SetBool("AltPrimary", character.altedP());
        }
    }

    public void cooldownCreated()
    {
        setThumbnails?.Invoke(character.getPrimary().getThumbnail(), CooldownIndicator.position.primary);
        setThumbnails?.Invoke(character.getSecondary().getThumbnail(), CooldownIndicator.position.secondary);
        setThumbnails?.Invoke(character.getUtility().getThumbnail(), CooldownIndicator.position.utility);
        setThumbnails?.Invoke(character.getSpecial().getThumbnail(), CooldownIndicator.position.special);
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
        {
            anim.ResetTrigger("Primary");
            anim.ResetTrigger("Secondary");
            anim.ResetTrigger("Utility");
            anim.ResetTrigger("Special");
        }

        if (primaryOnCooldown)
            updateCooldowns?.Invoke(primaryCooldown * getCooldowns(), CooldownIndicator.position.primary);
        if (secondaryOnCooldown)
            updateCooldowns?.Invoke(secondaryCooldown * getCooldowns(), CooldownIndicator.position.secondary);
        if (utilityOnCooldown)
            updateCooldowns?.Invoke(utilityCooldown * getCooldowns(), CooldownIndicator.position.utility);
        if (specialOnCooldown)
            updateCooldowns?.Invoke(specialCooldown * getCooldowns(), CooldownIndicator.position.special);
    }

    #region Attacks
    public void primary()
    {
        if (!primaryOnCooldown)
        {
            anim.SetFloat("AttackDirectionX", movement.getAim().x);
            anim.SetFloat("AttackDirectionY", movement.getAim().y);

            anim.SetTrigger("Primary");
        }
    }
    public void secondary()
    {
        if (!secondaryOnCooldown)
        {
            anim.SetFloat("AttackDirectionX", movement.getAim().x);
            anim.SetFloat("AttackDirectionY", movement.getAim().y);

            anim.SetTrigger("Secondary");
        }
    }
    public void utility()
    {
        if (!utilityOnCooldown)
        {
            anim.SetFloat("AttackDirectionX", movement.getAim().x);
            anim.SetFloat("AttackDirectionY", movement.getAim().y);

            anim.SetTrigger("Utility");
        }
    }
    public void special()
    {
        if (!specialOnCooldown)
        {
            anim.SetFloat("AttackDirectionX", movement.getAim().x);
            anim.SetFloat("AttackDirectionY", movement.getAim().y);

            anim.SetTrigger("Special");
        }
    }
    public void attack(Ability ability)    //Handles projectiles and buffs
    {
        if (ability.getSlot() == 1 && primaryCooldown > 0)
            primaryOnCooldown = true;
        else if (ability.getSlot() == 2 && secondaryCooldown > 0)
            secondaryOnCooldown = true;
        else if (ability.getSlot() == 3 && utilityCooldown > 0)
            utilityOnCooldown = true;
        else if (ability.getSlot() == 4 && specialCooldown > 0)
            specialOnCooldown = true;

        if (ability.getAttackType() == Ability.attackType.melee)
            attackMelee(ability);
        else if (ability.getAttackType() == Ability.attackType.projectile)
        {
            Vector3 offset = new Vector3(anim.GetFloat("AttackDirectionX"), anim.GetFloat("AttackDirectionY"));
            Quaternion output = Quaternion.LookRotation(Vector3.forward, offset).normalized;
            output = Quaternion.Euler(output.eulerAngles / 2);

            GameObject intBullet = Instantiate(ability.getBulletPrefab(), gameObject.transform.position, output);
            intBullet.SendMessage("setBullet", ability.getBullet());
            intBullet.SendMessage("Mult", getDamageMult());
            intBullet.SendMessage("setCritChance", getCrit());
            intBullet.SendMessage("UnderwaterMult", getUnderwaterMult());
            intBullet.SendMessage("AirborneMult", getAirborneMult());
        } 
        else if (ability.getAttackType() == Ability.attackType.buff)
        {
            StartCoroutine(Buff(ability.getBuff()));
            movement.buff(ability.getBuff());
        }

    }

    #region Melee Attacks
    public void setDirection(int i) { direction = i; }
    private void attackMelee(Ability ability) //Handles melee attacks
    {
        if (ability.getAttackType() != Ability.attackType.melee)
            return;

        if (ability.getSlot() == 1)
        {
            Weapon weapon = Melees1[direction].GetComponent<Weapon>();
            weapon.damageMult(getDamageMult());
            weapon.setUnderwaterMult(getUnderwaterMult());
            weapon.setAirborneMult(getAirborneMult());
            weapon.setCrit(getCrit());
            Melees1[direction].SetActive(true);
        } 
        else if (ability.getSlot() == 2)
        {
            Weapon weapon = Melees2[direction].GetComponent<Weapon>();
            weapon.damageMult(getDamageMult());
            weapon.setUnderwaterMult(getUnderwaterMult());
            weapon.setAirborneMult(getAirborneMult());
            weapon.setCrit(getCrit());
            Melees2[direction].SetActive(true);
        }
        else if (ability.getSlot() == 3)
        {
            Weapon weapon = Melees3[direction].GetComponent<Weapon>();
            weapon.damageMult(getDamageMult());
            weapon.setUnderwaterMult(getUnderwaterMult());
            weapon.setAirborneMult(getAirborneMult());
            weapon.setCrit(getCrit());
            Melees3[direction].SetActive(true);
        }
        else if (ability.getSlot() == 4)
        {
            Weapon weapon = Melees4[direction].GetComponent<Weapon>();
            weapon.damageMult(getDamageMult());
            weapon.setUnderwaterMult(getUnderwaterMult());
            weapon.setAirborneMult(getAirborneMult());
            weapon.setCrit(getCrit());
            Melees4[direction].SetActive(true);
        }

    }
    public void attackFinish()  //Turns off all the hitboxes of a given melee ability
    {
        for (int i = 0; i < Melees1.Length; i++)
        {
            if (Melees1[i] == null)
                break;

            Melees1[i].SetActive(false);
        }
        for (int i = 0; i < Melees2.Length; i++)
        {
            if (Melees2[i] == null)
                break;

            Melees2[i].SetActive(false);
        }
        for (int i = 0; i < Melees3.Length; i++)
        {
            if (Melees3[i] == null)
                break;

            Melees3[i].SetActive(false);
        }
        for (int i = 0; i < Melees4.Length; i++)
        {
            if (Melees4[i] == null)
                break;

            Melees4[i].SetActive(false);
        }
    }
    public void cooldownFinished(int type)  //Called by the cooldown indicators when they're refreshed
    {
        if (type == 0)
            primaryOnCooldown = false;
        else if (type == 1)
            secondaryOnCooldown = false;
        else if (type == 2)
            utilityOnCooldown = false;
        else if (type == 3)
            specialOnCooldown = false;
    }
    #endregion

    #endregion

    #region Buff managing
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
    private IEnumerator Buff(Buffs buff)
    {
        addBuff(buff);

        if (buff.getDuration() == -1)   //Infinite buffs
            yield break;

        yield return new WaitForSeconds(buff.getDuration());

        removeBuff(buff);
    }
    private void clearBuffs()
    {
        for (int i = 0; i < buffs.Length; i++)
            removeBuff(buffs[i]);
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
    private float getUnderwaterMult()
    {
        float mult = 1;
        for (int i = 0; i < buffs.Length; i++)
        {
            if (buffs[i] != null)
                mult *= buffs[i].getUnderwaterMult();
        }
        return mult;
    }
    private float getAirborneMult()
    {
        float mult = 1;
        for (int i = 0; i < buffs.Length; i++)
        {
            if (buffs[i] != null)
                mult *= buffs[i].getAirborneMult();
        }
        return mult;
    }
    private float getCrit()
    {
        float crit = 1;
        for (int i = 0; i < buffs.Length; i++)
        {
            if (buffs[i] != null)
                crit *= buffs[i].getCritChance();
        }
        return crit;
    }
    #endregion

    private bool IsPointerOverUIObject()    //Mainly for checking if you're clicking on the cooldown indicator buttons
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
