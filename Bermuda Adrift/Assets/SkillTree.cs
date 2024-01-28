using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTree : MonoBehaviour
{
    public static SkillTree skillTree;
    private void Awake() => skillTree = this;

    public int[] SkillLevels;
    public int[] SkillCaps;
    public string[] SkillNames;
    public string[] SkillDescriptions;

    public Skill[] SkillList;
    public GameObject SkillHolder;

    public int SkillPoint;

    public TMP_Text TitleText;
    public TMP_Text DescriptionText;




    // Start is called before the first frame update
    private void Start()
    {

        //foreach (var skill in SkillHolder.GetComponentInChildren<Skill>()) SkillList.Add(skill);
        for (var i = 0; i < SkillList.Length; i++) SkillList[i].id = i;


    }

    // Update is called once per frame
    void UpdateAllSkillUI()
    {

        foreach (var skill in SkillList)
        {
            int id = skill.getID();

            TitleText.text = $"{skillTree.SkillLevels[id]}/{skillTree.SkillCaps[id]}\n{skillTree.SkillNames[id]}";
            DescriptionText.text = $"{skillTree.SkillDescriptions[id]}\n Cost: {skillTree.SkillPoint}/1 SP";

            GetComponent<Image>().color = skillTree.SkillLevels[id] >= skillTree.SkillCaps[id] ? Color.yellow
                : skillTree.SkillPoint >= 1 ? Color.green : Color.white;
        }
    }

    public void Buy(Skill skill)
    {
        int id = skill.getID();
        if (skillTree.SkillPoint < 1 || skillTree.SkillLevels[id] >= skillTree.SkillCaps[id]) return;
        skillTree.SkillPoint -= 1;
        skillTree.SkillLevels[id]++;
    }

}
