using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player",menuName = "ScriptableObjects/Player")]
public class Player : ScriptableObject
{
    [SerializeField] private Ability defPrimary;
    [SerializeField] private Ability defSecondary;
    [SerializeField] private Ability defUtility;
    [SerializeField] private Ability defSpecial;
    [SerializeField] private Ability altPrimary;
    [SerializeField] private Ability altSecondary;
    [SerializeField] private Ability altUtility;
    [SerializeField] private Ability altSpecial;
    [SerializeField] private string description;

    private bool altP;
    private bool altS;
    private bool altU;
    private bool altSp;
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
        else
            altP = alt;
    }

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
    public bool altedP() { return altP; }
    public bool altedS() { return altS; }
    public bool altedU() { return altU; }
    public bool altedSp() { return altSp; }
    public string getDescription() { return description; }
}
