using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTree : MonoBehaviour
{
    public static SkillTree skillTree;
    private void Awake() => skillTree = this;

    //public int[] SkillLevels;
    //public int[] SkillCaps;
    //public string[] SkillNames;
    //public string[] SkillDescriptions;

    [SerializeField] private Skill[] SkillList;
    [SerializeField] private GameObject cardPrefab;

    //public int SkillPoint;

    //public TMP_Text TitleText;
    //public TMP_Text DescriptionText;


    // Start is called before the first frame update
    private void Start()
    {
        //I don't think we'll need a start function

        /*
        //foreach (var skill in SkillHolder.GetComponentInChildren<Skill>()) SkillList.Add(skill);
        for (var i = 0; i < SkillList.Length; i++) SkillList[i].id = i;

        foreach (connector : RectTransform  in ConnectorHolder.GetComponentsInChildren<RectTransform>()) ConnectionList.Add(connector, GameObject);

        SkillList[0].ConnectedSkills = new[] (1, 2, 3);
        SkillList[1].ConnectedSkills = new[] (1, 2, 3);
        SkillList[2].ConnectedSkills = new[] (1, 2, 3);
        */
    }

    // Update is called once per frame
    void UpdateAllSkillUI()
    {
        clearSkillCards();
        //Progress left to right, bool to keep track of when the skills aren't unlocked anymore?

        //Maybe specific code for the first level of skill cards
        //else
        foreach (Skill skill in SkillList)
        {
            //if (skill.pairs.length <= 0)
            //Create card as a child of TreeContent (child 0 of child 0 of gameObject)
            //Change Title text to skill.title
            //Change HoverText to skill.description
            //Change cost text to skill.cost
            //else
            //Create 2 or 3 card
            //Perform same steps

            //if (skill.unlocked)
            //Remove cost text
            //Change color of card background (just changing the tilemap behind it should do the effect well enough)
        }

        /*
        foreach (var skill in SkillList)
        {
            
            int id = skill.getID();

            TitleText.text = $"{skillTree.SkillLevels[id]}/{skillTree.SkillCaps[id]}\n{skillTree.SkillNames[id]}";
            DescriptionText.text = $"{skillTree.SkillDescriptions[id]}\n Cost: {skillTree.SkillPoint}/1 SP";

            GetComponent<Image>().color = skillTree.SkillLevels[id] >= skillTree.SkillCaps[id] ? Color.yellow
                : skillTree.SkillPoint >= 1 ? Color.green : Color.white;
            
        }
        */
    }
    void clearSkillCards()
    {
        //Foreach child of TreeContent
        //Destroy the child
    }

    public void Buy(Skill skill)
    {
        //If (gameManager.skillpoints >= skill.cost)
        //Unlock skill (effects work from there)
        //Remove skill points
        //Update UI


        /*
        int id = skill.getID();
        if (skillTree.SkillPoint < 1 || skillTree.SkillLevels[id] >= skillTree.SkillCaps[id]) return;
        skillTree.SkillPoint -= 1;
        skillTree.SkillLevels[id]++;
        */
    }

    public Skill getSkill(string skillName)
    {
        foreach (Skill skill in SkillList)
        {
            if (skill.getTitleText().CompareTo(skillName) == 0)
                return skill;
        }

        Debug.Log("No match for " + skillName);
        return null;
    }

}
