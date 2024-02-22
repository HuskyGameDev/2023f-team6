using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AchievementsMenu : MonoBehaviour
{
    [SerializeField] private GameObject achievementDisplay;
    [SerializeField] private Achievement[] achievements;

    private void Start()
    {
        reloadAchievements();
    }

    public void reloadAchievements()
    {
        for (int i = 2; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        GameObject currentPanel;
        string text;
        foreach (Achievement a in achievements)
        {
            text = a.getName() + "\n" + a.getDescription();
            if (a.getAssociatedCharacter() != null)
                text += ". Unlocks " + a.getAssociatedCharacter().getName();
            else if (a.getAssociatedTower() != null)
                text += ". Unlocks " + a.getAssociatedTower().getName();
            else if (a.getAssociatedBarrier() != null)
                text += ". Unlocks " + a.getAssociatedBarrier().getName();

            currentPanel = Instantiate(achievementDisplay, transform);
            currentPanel.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = text;


            RectTransform rt = gameObject.GetComponent(typeof(RectTransform)) as RectTransform;
            rt.sizeDelta = new Vector2(1285, (achievements.Length - 2) * 230);

            transform.parent.parent.GetChild(1).GetComponent<Scrollbar>().value = 1;

            if (a.getUnlocked())
                currentPanel.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
            else
                currentPanel.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.grey;
        }
    }

    public void unlockAll()
    {
        foreach (Achievement a in achievements)
        {
            if (a.getAssociatedTower() != null) a.getAssociatedTower().setUnlocked(true);
            if (a.getAssociatedBarrier() != null) a.getAssociatedBarrier().setUnlock(true);
            if (a.getAssociatedCharacter() != null) a.getAssociatedCharacter().setUnlock(true);

            a.setUnlocked(true);
        }

        reloadAchievements();
    }
}
