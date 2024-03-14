using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Attack : MonoBehaviour
{
    public static event Action<float, CooldownIndicator.position> updateCooldowns;
    //public static event Action<Sprite, Ability> setThumbnails;
    public static event Action loopUsed;
    public static event Action barrelUsed;

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
    private KeyCode primaryButton = KeyCode.Mouse0;

    private bool secondaryOnCooldown;
    private float secondaryCooldown = 1f;
    private KeyCode secondaryButton = KeyCode.Mouse1;

    private bool utilityOnCooldown;
    private float utilityCooldown = 6f;
    private KeyCode utilityButton = KeyCode.Q;

    private bool specialOnCooldown;
    private float specialCooldown = 10f;
    private KeyCode specialButton = KeyCode.R;

    private KeyCode currentButton;

    private int direction;
    private int attackCooldownFrames = 5;
    private bool attackCooldownBool;

    private Buffs[] buffs = new Buffs[10];

    private void OnEnable()
    {
        CooldownIndicator.cooldownComplete += cooldownFinished;
        CooldownIndicator.awoken += cooldownCreated;
    }
    private void OnDisable()
    {
        CooldownIndicator.cooldownComplete -= cooldownFinished;
        CooldownIndicator.awoken -= cooldownCreated;
    }
    private void Start()
    {
        movement = gameObject.GetComponent<Movement>();
        anim = gameObject.GetComponent<Animator>();

        if (GameObject.FindAnyObjectByType<SettingsTracker>() == null)
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
        {
            StartCoroutine(Buff(playerData.getPassive()));
            movement.buff(playerData.getPassive());
        }
        movement.setSpeed(character.getSpeed());
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
        GameObject.Find("Primary").transform.GetChild(0).GetChild(0).GetComponent<CooldownIndicator>().setThumbnail(character.getPrimary());
        GameObject.Find("Secondary").transform.GetChild(0).GetChild(0).GetComponent<CooldownIndicator>().setThumbnail(character.getSecondary());
        GameObject.Find("Utility").transform.GetChild(0).GetChild(0).GetComponent<CooldownIndicator>().setThumbnail(character.getUtility());
        GameObject.Find("Special").transform.GetChild(0).GetChild(0).GetComponent<CooldownIndicator>().setThumbnail(character.getSpecial());

        CooldownIndicator.awoken -= cooldownCreated;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("Managers").GetComponent<GameManager>().getGameState() != GameManager.GameState.Defend && GameObject.Find("Managers").GetComponent<GameManager>().getGameState() != GameManager.GameState.BossRound)
        {
            anim.SetBool("Primary", false);
            anim.SetBool("Secondary", false);
            anim.SetBool("Utility", false);
            anim.SetBool("Special", false);

            return;
        }

        anim.speed = getAttackSpeed();

        setCurrentButton();

        if (currentButton == primaryButton) { primary(); }

        else if (currentButton == secondaryButton) { secondary(); }

        else if (currentButton == utilityButton) { utility(); }

        else if (currentButton == specialButton) { special(); }

        if (!anim.GetBool("Locked"))
        {
            anim.SetFloat("AttackDirectionX", movement.getAim().x);
            anim.SetFloat("AttackDirectionY", movement.getAim().y);
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
    private void primaryCancel()
    {
        anim.ResetTrigger("Primary");
        anim.Play("Idle");
    }
    private void setCurrentButton()
    {
        //Input.GetKeyDown(specialButton) || (character.getSpecial().canBeLooped() && Input.GetKey(specialButton)) || (!specialOnCooldown && !anim.GetBool("Special") && Input.GetKey("r"))
        //Priority: Special, then Utility, then Secondary, then Primary

        if (Input.GetKeyDown(specialButton) || (character.getSpecial().canBeLooped() && Input.GetKey(specialButton)) || (!specialOnCooldown && !anim.GetBool("Special") && Input.GetKey(specialButton)))
            currentButton = specialButton;
        else if (Input.GetKeyDown(utilityButton) || (character.getUtility().canBeLooped() && Input.GetKey(utilityButton)) || (!utilityOnCooldown && !anim.GetBool("Utility") && Input.GetKey(utilityButton)))
            currentButton = utilityButton;
        else if (Input.GetKeyDown(secondaryButton) || (character.getSecondary().canBeLooped() && Input.GetKey(secondaryButton)) || (!secondaryOnCooldown && !anim.GetBool("Secondary") && Input.GetKey(secondaryButton)))
            currentButton = secondaryButton;
        else if (Input.GetKeyDown(primaryButton) || (character.getPrimary().canBeLooped() && Input.GetKey(primaryButton)) ||(!primaryOnCooldown && !anim.GetBool("Primary") && Input.GetKey(primaryButton)))
            currentButton = primaryButton;
        else
            currentButton = KeyCode.None;
    }
    public void primary()
    {
        //Debug.Log("Primary");
        if (!primaryOnCooldown && !otherAttackActive(1))
        {
            if (primaryCooldown > 0)
                primaryOnCooldown = true;

            if (character.getPrimary().directionLocked())
                anim.SetBool("Locked", true);

            anim.SetTrigger("Primary");
        }
    }
    public void secondary()
    {
        //Debug.Log("Secondary");
        if (!secondaryOnCooldown)
        {
            primaryCancel();

            if (secondaryCooldown > 0)
                secondaryOnCooldown = true;

            if (character.getSecondary().directionLocked())
                anim.SetBool("Locked", true);

            anim.SetTrigger("Secondary");
        }
    }
    public void utility()
    {
        //Debug.Log("Utility");
        if (!utilityOnCooldown)
        {
            primaryCancel();

            if (utilityCooldown > 0)
                utilityOnCooldown = true;

            if (character.getUtility().directionLocked())
                anim.SetBool("Locked", true);

            anim.SetTrigger("Utility");
        }
    }
    public void special()
    {
        //Debug.Log("Special");
        if (!specialOnCooldown)
        {
            primaryCancel();

            if (specialCooldown > 0)
                specialOnCooldown = true;

            if (character.getSpecial().directionLocked())
                anim.SetBool("Locked", true);

            if (character.name == "P_Pirate")
                barrelUsed?.Invoke();

            anim.SetTrigger("Special");
        }
    }
    public void attack(Ability ability)    //Handles projectiles and buffs
    {
        if (attackCooldownBool) { return; }
        attackCooldownBool = true;            

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
            intBullet.SendMessage("setLevelScale", GameObject.Find("Managers").GetComponent<GameManager>().getLevelScale());
        } 
        else if (ability.getAttackType() == Ability.attackType.buff)
        {
            StartCoroutine(Buff(ability.getBuff()));
            movement.buff(ability.getBuff());
        }
        else if (ability.getAttackType() == Ability.attackType.destinedProjectile)
        {
            GameObject intBullet = Instantiate(ability.getBulletPrefab(), new Vector3(100f,100f), Quaternion.identity);
            intBullet.SendMessage("setBullet", ability.getDestinedBullet());
            intBullet.SendMessage("Mult", getDamageMult());
            intBullet.SendMessage("setCritChance", getCrit());
            intBullet.SendMessage("UnderwaterMult", getUnderwaterMult());
            intBullet.SendMessage("AirborneMult", getAirborneMult());
            intBullet.SendMessage("setLevelScale", GameObject.Find("Managers").GetComponent<GameManager>().getLevelScale());
        }
        else if (ability.getAttackType() == Ability.attackType.targetedAttack)
        {
            Instantiate(ability.getTargetedAttacker()); //The scripts on these game objects will deal with everything from here
        }
        else if (ability.getAttackType() == Ability.attackType.tempTower)
        {
            FindObjectOfType<BuildManager>().placeRandom(ability.getTempTower(), ability.getTowerDuration());
        }

        if (ability.canBeLooped())
            loopUsed?.Invoke();

        StartCoroutine(attackDelay());
    }

    private IEnumerator attackDelay()   //Blend trees make 2 animations with the same animation events happen simultaneously. This makes it so the attack even only happens once
    {
        for (int i = 0; i < attackCooldownFrames; i++)
            yield return new WaitForEndOfFrame();
        attackCooldownBool = false;
    }
    private IEnumerator antiHold(bool canLoop, int attack)  //Resets the animation bool after a few frames so you can't just hold the button down
    {
        if (!canLoop)
        {
            for (int i = 0; i < attackCooldownFrames; i++)
                yield return new WaitForEndOfFrame();

            if (attack == 1)
                anim.SetBool("Primary", false);
            if (attack == 2)
                anim.SetBool("Secondary", false);
            if (attack == 3)
                anim.SetBool("Utility", false);
            if (attack == 4)
                anim.SetBool("Special", false);
        }
    }
    public void resetMove(String boolName) { anim.SetBool(boolName, false); }

    #region Melee Attacks
    int getDirection() 
    {
        var mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0f;
        Vector3 AimVector = mouseWorldPosition - gameObject.transform.position;
        AimVector.Normalize();

        if (AimVector.x < Mathf.Cos(22.5f) && AimVector.x > Mathf.Cos(67.5f))   //Up right or Down Right
        {
            if (AimVector.y > 0)
                return 5;                   //Up Right
            else
                return 7;                   //Down Right
        }
        else if (AimVector.x <= Mathf.Cos(67.5f) && AimVector.x >= -1 * Mathf.Cos(67.5f))    //Up or Down
        {
            if (AimVector.y > 0) 
                return 4;                   //Up
            else 
                return 0;                  //Down
        }
        else if(AimVector.x > -1 * Mathf.Cos(22.5f) && AimVector.x < -1 * Mathf.Cos(67.5f))  //Up Left or Down Left
        {
            if (AimVector.y > 0)
                return 3;                   //Up Left
            else
                return 1;                   //Down Left
        }
        else //Left or Right
        {
            if (AimVector.x < 0)
                return 2;                   //Left
            else
                return 6;                   //Right
        }

    }
    private void attackMelee(Ability ability) //Handles melee attacks
    {
        if (ability.getAttackType() != Ability.attackType.melee)
            return;

        direction = getDirection();

        if (ability.getSlot() == 1)
        {
            Weapon weapon = Melees1[direction].GetComponent<Weapon>();
            weapon.damageMult(getDamageMult());
            weapon.setUnderwaterMult(getUnderwaterMult());
            weapon.setAirborneMult(getAirborneMult());
            weapon.setCrit(getCrit());
            weapon.setLevelScale(GameObject.Find("Managers").GetComponent<GameManager>().getLevelScale());
            Melees1[direction].SetActive(true);
        } 
        else if (ability.getSlot() == 2)
        {
            Weapon weapon = Melees2[direction].GetComponent<Weapon>();
            weapon.damageMult(getDamageMult());
            weapon.setUnderwaterMult(getUnderwaterMult());
            weapon.setAirborneMult(getAirborneMult());
            weapon.setCrit(getCrit());
            weapon.setLevelScale(GameObject.Find("Managers").GetComponent<GameManager>().getLevelScale());
            Melees2[direction].SetActive(true);
        }
        else if (ability.getSlot() == 3)
        {
            Weapon weapon = Melees3[direction].GetComponent<Weapon>();
            weapon.damageMult(getDamageMult());
            weapon.setUnderwaterMult(getUnderwaterMult());
            weapon.setAirborneMult(getAirborneMult());
            weapon.setCrit(getCrit());
            weapon.setLevelScale(GameObject.Find("Managers").GetComponent<GameManager>().getLevelScale());
            Melees3[direction].SetActive(true);
        }
        else if (ability.getSlot() == 4)
        {
            Weapon weapon = Melees4[direction].GetComponent<Weapon>();
            weapon.damageMult(getDamageMult());
            weapon.setUnderwaterMult(getUnderwaterMult());
            weapon.setAirborneMult(getAirborneMult());
            weapon.setCrit(getCrit());
            weapon.setLevelScale(GameObject.Find("Managers").GetComponent<GameManager>().getLevelScale());
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
        {
            primaryOnCooldown = false;
            anim.SetBool("Primary", false);

            if (character.getPrimary().directionLocked())
                anim.SetBool("Locked", false);
        }
        else if (type == 1)
        {
            secondaryOnCooldown = false;
            anim.SetBool("Secondary", false);

            if (character.getSecondary().directionLocked())
                anim.SetBool("Locked", false);
        }
        else if (type == 2)
        {
            utilityOnCooldown = false;
            anim.SetBool("Utility", false);

            if (character.getUtility().directionLocked())
                anim.SetBool("Locked", false);
        }
        else if (type == 3)
        {
            specialOnCooldown = false;
            anim.SetBool("Special", false);

            if (character.getSpecial().directionLocked())
                anim.SetBool("Locked", false);
        }
    }
    #endregion

    private bool otherAttackActive(int currentAttack)   //Returns true if another attack anim bool is active, but false if the bool is the primary
    {
        if (currentAttack == 1 && (anim.GetBool("Secondary") || anim.GetBool("Utility") || anim.GetBool("Special")))
            return true;
        else if (currentAttack == 2 && (
            //anim.GetBool("Primary") || 
            anim.GetBool("Utility") || anim.GetBool("Special")))
            return true;
        else if (currentAttack == 3 && (
            //anim.GetBool("Primary") || 
            anim.GetBool("Secondary") || anim.GetBool("Special")))
            return true;
        else if (currentAttack == 4 && (
            //anim.GetBool("Primary") || 
            anim.GetBool("Secondary") || anim.GetBool("Utility")))
            return true;

        return false;
    }
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
    private float getCrit() //Returns crit chance
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
