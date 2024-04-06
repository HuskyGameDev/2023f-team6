using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SkillTree;

[CreateAssetMenu(fileName = "Skill", menuName = "ScriptableObjects/Skill")]
public class Skill : ScriptableObject
{
    [SerializeField] private string TitleText;
    [SerializeField] private string DescriptionText;
    [SerializeField] private int SkillCost;

    //Just check names to apply effect
    [SerializeField] private float effectStrength;

    [SerializeField] private Skill[] Pairs;
    [SerializeField] private bool unlocked;

    public string getTitleText() { return TitleText;}
    public string getDescriptionText() { return DescriptionText;}
    public int getCost() { return SkillCost; }
    public Skill[] getPairs() {  return Pairs;}

    public bool getUnlocked() { return unlocked; } 
    public void setUnlocked(bool unlocked) {  this.unlocked = unlocked; }

}
