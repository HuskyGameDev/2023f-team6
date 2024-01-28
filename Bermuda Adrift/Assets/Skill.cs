using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SkillTree;

[CreateAssetMenu(fileName = "Skill", menuName = "ScriptableObjects/Skill")]
public class Skill : ScriptableObject
{
    // Start is called before the first frame update
    public int id;

    public string TitleText;
    public string DescriptionText;


    public int[] ConnectedSkills;
    bool unlocked;

    public int getID() { return id; }
    public string getTitleText() { return TitleText;}
    public string getDescriptionText() { return DescriptionText;}
    public int[] getConnectedSkills() {  return ConnectedSkills;}
    public bool isLocked() { return unlocked;} public void setLocked(bool locked) {  unlocked = locked;}








}
