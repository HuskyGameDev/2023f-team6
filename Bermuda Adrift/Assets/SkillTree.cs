using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillTree : MonoBehaviour
{
    public static SkillTree skillTree;
    private void Awake() => skillTree = this;
    private void Start()
    {
        UpdateAllSkillUI(0);
    }

    public Skill[] Section1;
    public Skill[] Section2;
    public Skill[] Section3;
    public Skill[] Section4;
    public Skill[] PirateSkills;
    public Skill[] PilotSkills;
    public Skill[] OldManSkills;
    public Skill[] Last3;

    [SerializeField] private GameObject cardPrefab;

    Color lockedColor = new Color(190f / 255f, 190f / 255f, 190f / 255f);




    // Update is called once per frame
    public void UpdateAllSkillUI(int j)
    {
        clearSkillCards();

        Skill[] activeList;
        GameObject activeCard;

        if (j == 0) activeList = null;
        else if (j == 1) activeList = Section1;
        else if (j == 2) activeList = Section2;
        else if (j == 3) activeList = Section3;
        else if (j == 4) activeList = Section4;
        else if (j == 5) activeList = PirateSkills;
        else if (j == 6) activeList = PilotSkills;
        else if (j == 7) activeList = OldManSkills;
        else if (j == 8) activeList = Last3;
        else activeList = null;

        if (activeList == null)
        {
            for (int i = 1; i < 5; i++)
            {
                newCard(i, i.ToString());
            }
            newCard(5, "Pirate");
            newCard(6, "Pilot");
            newCard(7, "Old Man");
            newCard(8, "Final Three");
        }
        else
        {
            Skill skill;
            bool furthest = false;
            for (int i = 0; i < activeList.Length; i++)
            {
                skill = activeList[i];

                activeCard = newCard(skill);

                if (!furthest)  //Only the available skills can be unlocked
                {
                    if (!skill.getUnlocked())   //First unlocked skill
                    {
                        activeCard.GetComponent<Button>().interactable = true;
                        activeCard.GetComponent<Image>().color = Color.white;
                        activeCard.GetComponent<Button>().onClick.AddListener(() => UpdateAllSkillUI(j));

                        if (skill.getPairs() > 0)   //Allow other buttons if it's marked
                        {
                            for (int k = 0; k < skill.getPairs(); k++)
                            {
                                i++;
                                activeCard = newCard(activeList[i]);
                                activeCard.GetComponent<Image>().color = activeList[i].getUnlocked() ? Color.white : lockedColor;
                                activeCard.GetComponent<Button>().onClick.AddListener(() => UpdateAllSkillUI(j));
                            }
                        }

                        furthest = true;
                    }
                }
            }

            if (!cardAvailable(activeList)) //Can look at future skills, but you can't unlock them until the prerequisites are met
            {
                for (int i = 0; i < transform.GetChild(0).GetChild(0).childCount; i++)
                {
                    transform.GetChild(0).GetChild(0).GetChild(i).GetComponent<Button>().interactable = false;
                    transform.GetChild(0).GetChild(0).GetChild(i).GetComponent<Image>().color = lockedColor;
                }
            }
        }
        StartCoroutine(scrollbarReset());
    }
    IEnumerator scrollbarReset()
    {
        transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2((float)(20 + (300 + 10.4 * 2) * transform.GetChild(0).GetChild(0).childCount), transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta.y);

        yield return new WaitForEndOfFrame();

        transform.GetChild(1).GetComponent<Scrollbar>().value = 0;
    }
    void clearSkillCards()
    {
        for (int i = 0; i < transform.GetChild(0).GetChild(0).childCount; i++)
        {
            Destroy(transform.GetChild(0).GetChild(0).GetChild(i).gameObject);
        }
    }
    GameObject newCard(Skill skill)
    {
        GameObject newCard = Instantiate(cardPrefab, transform.GetChild(0).GetChild(0));

        if (skill == null)
            return newCard;

        newCard.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = skill.getTitleText();
        newCard.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = skill.getDescriptionText();
        newCard.transform.GetChild(2).GetComponent<Image>().sprite = skill.getSprite();

        newCard.GetComponent<Button>().onClick.AddListener(() => FindObjectOfType<SkillManager>().buySkill(skill));

        newCard.GetComponent<Button>().interactable = skill.getUnlocked();
        newCard.GetComponent<Image>().color = lockedColor;
        return newCard;
    }
    void newCard(int i, string title)   //Used for the main cards in the base menu
    {
        GameObject newCard = Instantiate(cardPrefab, transform.GetChild(0).GetChild(0));

        newCard.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = title;
        newCard.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
        newCard.transform.GetChild(2).GetComponent<Image>().enabled = false;

        newCard.GetComponent<Button>().onClick.AddListener(() => UpdateAllSkillUI(i));
        newCard.GetComponent<Button>().interactable = true;
        newCard.GetComponent<Image>().color = Color.white;
    }

    bool cardDone(Skill[] skillSet) //Returns true if every skill in the list is unlocked
    {
        foreach (Skill skill in skillSet)
        {
            if (!skill.getUnlocked())
                return false;
        }
        return true;
    }
    bool cardAvailable(Skill[] card)
    {
        if (card == null) return false;

        if (card == Section1) return true;

        if (card == Section2) return cardDone(Section1);

        if (card == Section3) return cardDone(Section1) && cardDone(Section2);

        if (card == Section4) return cardDone(Section1) && cardDone(Section2) && cardDone(Section3);

        if (card == PirateSkills) return true;

        if (card == PilotSkills) return FindObjectOfType<SettingsTracker>().getCharacterUnlock("Pilot");

        if (card == OldManSkills) return FindObjectOfType<SettingsTracker>().getCharacterUnlock("Old Man");

        if (card == Last3) return cardDone(Section1) && cardDone(Section2) && cardDone(Section3) && cardDone(Section4) && cardDone(PirateSkills) && cardDone(PilotSkills) && cardDone(OldManSkills);

        return false;
    }
}
