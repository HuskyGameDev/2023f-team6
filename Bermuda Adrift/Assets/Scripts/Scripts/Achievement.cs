using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Achievement", menuName = "ScriptableObjects/Achievement")]
public class Achievement : ScriptableObject
{
    [SerializeField] private string Name;
    [SerializeField] private string Description;

    [SerializeField] private Tower associatedTower;
    [SerializeField] private BarrierScriptable associatedBarrier;
    [SerializeField] private Player associatedCharacter;

    [SerializeField] private int skillPoints;
    [SerializeField] private bool unlocked;


    public string getName() { return Name; }
    public string getDescription() { return Description; }
    public Tower getAssociatedTower() { return associatedTower; }
    public BarrierScriptable getAssociatedBarrier() { return associatedBarrier; }
    public Player getAssociatedCharacter() { return associatedCharacter; }
    public int getSkillPoints() { return skillPoints; }
    public bool getUnlocked() { return unlocked; }
    public void setUnlocked(bool unlocked) { this.unlocked = unlocked; }
}
