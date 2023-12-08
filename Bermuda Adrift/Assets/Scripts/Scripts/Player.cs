using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player",menuName = "ScriptableObjects/Player")]
public class Player : ScriptableObject
{
    [SerializeField] private RuntimeAnimatorController anim;
    [SerializeField] private Ability defPrimary;
    [SerializeField] private Ability defSecondary;
    [SerializeField] private Ability defUtility;
    [SerializeField] private Ability defSpecial;
    [SerializeField] private Ability altPrimary;
    [SerializeField] private Ability altSecondary;
    [SerializeField] private Ability altUtility;
    [SerializeField] private Ability altSpecial;
    [SerializeField] private Buffs defPassive;
    [SerializeField] private Buffs altPassive;
    [SerializeField] private string description;
    [SerializeField] private string name;
    [SerializeField] private Sprite mainBodySprite;
    [SerializeField] private int speed;
    [SerializeField] private bool unlocked;

    private bool altP;
    private bool altS;
    private bool altU;
    private bool altSp;
    private bool altPa;
    public void alt(int slot, bool alt)
    {
        if (slot == 1)
            altP = alt;
        else if (slot == 2)
            altS = alt;
        else if (slot == 3)
            altU = alt;
        else if (slot == 4)
            altSp = alt;
        else if (slot == 5)
            altPa = alt;
        else
            altP = alt;
    }

    public RuntimeAnimatorController getAnim() { return anim; }
    public Ability getPrimary()
    {
        if (altP && altPrimary != null)
            return altPrimary;
        return defPrimary;
    }
    public Ability getSecondary()
    {
        if (altS && altSecondary != null)
            return altSecondary;
        return defSecondary;
    }
    public Ability getUtility()
    {
        if (altU && altUtility != null)
            return altUtility;
        return defUtility;
    }
    public Ability getSpecial()
    {
        if (altSp && altSpecial != null)
            return altSpecial;
        return defSpecial;
    }
    public Buffs getPassive()
    {
        if (altPa && altPassive != null)
            return altPassive;
        return defPassive;
    }
    public void unlock() { unlocked = true; }


    public Ability getDefPrimary() { return defPrimary; }
    public Ability getDefSecondary() { return defSecondary; }
    public Ability getDefUtility() { return defUtility; }
    public Ability getDefSpecial() { return defSpecial; }
    public Buffs getDefPassive() { return defPassive; }
    public Ability getAltPrimary() { return altPrimary; }
    public Ability getAltSecondary() { return altSecondary; }
    public Ability getAltUtility() { return altUtility; }
    public Ability getAltSpecial() { return altSpecial; }
    public Buffs getAltPassive() { return altPassive; }
    public bool altedP() { return altP; }
    public bool altedS() { return altS; }
    public bool altedU() { return altU; }
    public bool altedSp() { return altSp; }
    public bool altedPa() { return altPa; }
    public string getDescription() { return description; }
    public string getName() { return name; }
    public Sprite getMainBodySprite() { return mainBodySprite; }
    public int getSpeed() { return speed; }
    public bool getUnlocked() { return unlocked; }
}
