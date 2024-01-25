using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTree : MonoBehaviour
{
    public static SkillTree skillTree;
    private void Awake() => skillTree = this;

    public int[] SkillLevels;
    public int[] SkillCaps;
    public string[] SkillNames;
    public string[] SkillDescriptions;

    public List<Skill> SkillList;
    public GameObject SkillHolder;

    public int SkillPoint;


    // Start is called before the first frame update
    private void Start()
    {
        SkillPoint = 5;

        SkillLevels = new int[10];
        SkillCaps = new[] { 1 };
        SkillNames = new[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", };
        SkillDescriptions = new[] {
        "What",
        "What",
        "What",
        "What",
        "What",
        "What",
        "What",
        "What",
        "What",
        "What",
        "What",
        };

        foreach (var skill in SkillHolder.GetComponentInChildren<Skill>()) SkillList.Add(skill);
        for (var i = 0; i < SkillList.Count; i++) SkillList[i].id = i;


    }

    // Update is called once per frame
    void UpdateAllSkillUI()
    {
        foreach (var skill in SkillList) skill.UpdateUI();
    }
}
