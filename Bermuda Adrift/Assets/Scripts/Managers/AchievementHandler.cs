using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AchievementHandler : MonoBehaviour, IDataPersistence
{
    public static event Action<string, string> OnAchievementUnlocked;

    [SerializeField] private Achievement[] achievements;
    int saveBits = 0;

    [SerializeField] GameObject achievementPopup;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
            unlockAchievement("Ach_Man_In_The_Mirror");
    }

    Achievement getAchievement(string name)
    {
        foreach (Achievement a in achievements)
        {
            if (a.name.CompareTo(name) == 0)
                return a;
        }
        Debug.Log(name + " isn't in the list");
        return null;
    }
    int getAchievementIndex(string name)
    {
        for (int i = 0; i < 32; i++)
        {
            if (achievements[i].name.CompareTo(name) == 0)
                return i;
        }
        Debug.Log(name + " isn't in the list");
        return 32;
    }

    void unlockAchievement(string name)
    {
        int index = getAchievementIndex(name);
        if ((saveBits & (1 << index)) == (1 << index)) return;  //Return if the achievement is already unlocked

        Achievement toBeUnlocked = achievements[index];
        toBeUnlocked.setUnlocked(true);

        if (toBeUnlocked.getAssociatedTower() != null)
            toBeUnlocked.getAssociatedTower().setUnlocked(true);
        else if (toBeUnlocked.getAssociatedBarrier() != null)
            toBeUnlocked.getAssociatedBarrier().setUnlock(true);
        else if (toBeUnlocked.getAssociatedCharacter() != null)
            toBeUnlocked.getAssociatedCharacter().setUnlock(true);
        //Some achievements have none, unlocks anyway

        //Play achievement popup
        StartCoroutine(popupAchievement(toBeUnlocked.getName(), toBeUnlocked.getDescription()));
    }
    IEnumerator popupAchievement(string title, string description)
    {
        achievementPopup.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = title;
        achievementPopup.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = description;
        achievementPopup.GetComponent<Animator>().Play("AchievementPopup");

        yield return new WaitForSeconds(5f);

        achievementPopup.GetComponent<Animator>().Play("AchievementPopdown");
    }



    void unlockAchievement(int index)   //No popups, for loading
    {
        Achievement toBeUnlocked = achievements[index];

        if (toBeUnlocked.getAssociatedTower() != null)
            toBeUnlocked.getAssociatedTower().setUnlocked(true);
        else if (toBeUnlocked.getAssociatedBarrier() != null)
            toBeUnlocked.getAssociatedBarrier().setUnlock(true);
        else if (toBeUnlocked.getAssociatedCharacter() != null)
            toBeUnlocked.getAssociatedCharacter().setUnlock(true);
    }

    public void LoadData(S_O_Saving saver)
    {
        foreach (Achievement ach in achievements)
        {
            ach.setUnlocked(saver.getAchievementSave(ach.name).unlocked);
        }
    }

    public void SaveData(S_O_Saving saver)
    {
        foreach (Achievement ach in achievements)
        {
            saver.getAchievementSave(ach.name).unlocked = ach.getUnlocked();
        }
    }
}
