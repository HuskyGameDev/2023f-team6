using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AchievementsMenu : MonoBehaviour
{
    [SerializeField] private GameObject achievementDisplay;
    [SerializeField] private Achievement[] achievements;

    private void OnEnable()
    {
        reloadAchievements();
    }
    private void OnDisable()
    {
        deleteAchievementCards();
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
            Sprite image = null;
            if (a.getAssociatedCharacter() != null)
                image = a.getAssociatedCharacter().getMainBodySprite();
            else if (a.getAssociatedTower() != null)
                image = a.getAssociatedTower().getImage();
            else if (a.getAssociatedBarrier() != null)
                image = a.getAssociatedBarrier().getThumbnail();
            else if (a.getAssociatedCenterpiece() != null)
                image = a.getAssociatedCenterpiece().getSprite();

            currentPanel = Instantiate(achievementDisplay, transform);
            currentPanel.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
            if (image != null)
            {
                currentPanel.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
                currentPanel.transform.GetChild(1).GetChild(1).GetComponent<Image>().sprite = image;
            }


            RectTransform rt = gameObject.GetComponent(typeof(RectTransform)) as RectTransform;
            rt.sizeDelta = new Vector2(1285, (achievements.Length - 2) * 230);

            if (a.getUnlocked())
            {
                currentPanel.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
                currentPanel.transform.GetChild(1).GetChild(1).GetComponent<Image>().color = Color.white;
            }
            else
            {
                currentPanel.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.grey;
                currentPanel.transform.GetChild(1).GetChild(1).GetComponent<Image>().color = Color.grey;
            }
        }

        StartCoroutine(scrollbarReset());
    }
    void deleteAchievementCards()
    {
        for (int i = 1; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
    IEnumerator scrollbarReset()
    {
        yield return new WaitForEndOfFrame();
        transform.parent.parent.GetChild(1).GetComponent<Scrollbar>().value = 0.9760087f;
    }

    public void unlockAll()
    {
        foreach (Achievement a in achievements)
        {
            if (a.getAssociatedTower() != null) a.getAssociatedTower().setUnlocked(true);
            if (a.getAssociatedBarrier() != null) a.getAssociatedBarrier().setUnlock(true);
            if (a.getAssociatedCharacter() != null) a.getAssociatedCharacter().setUnlock(true);
            if (a.getAssociatedCenterpiece() != null) a.getAssociatedCenterpiece().setUnlock(true);

            a.setUnlocked(true);
        }

        reloadAchievements();
    }
}
