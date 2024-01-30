using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AchievementHandler : MonoBehaviour
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

        if (toBeUnlocked.getAssociatedTower() != null)
            toBeUnlocked.getAssociatedTower().unlock();
        else if (toBeUnlocked.getAssociatedBarrier() != null)
            toBeUnlocked.getAssociatedBarrier().unlock();
        else if (toBeUnlocked.getAssociatedCharacter() != null)
            toBeUnlocked.getAssociatedCharacter().unlock();
        //Some achievements have none, unlocks anyway

        //Play achievement popup
        StartCoroutine(popupAchievement(toBeUnlocked.getName(), toBeUnlocked.getDescription()));

        saveBits = saveBits | (1 << index);    //Set associated bit to 1
        displaySave();
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
        if (index >= 32) return;

        Achievement toBeUnlocked = achievements[index];

        if (toBeUnlocked.getAssociatedTower() != null)
            toBeUnlocked.getAssociatedTower().unlock();
        else if (toBeUnlocked.getAssociatedBarrier() != null)
            toBeUnlocked.getAssociatedBarrier().unlock();
        else if (toBeUnlocked.getAssociatedCharacter() != null)
            toBeUnlocked.getAssociatedCharacter().unlock();
    }

    void loadSave(int newSave)
    {
        saveBits = newSave;
        int check;

        for (int i = 0; i < 32; i++)
        {
            check = 1 << i;
            if ((saveBits & check) == check)
                unlockAchievement(i);
        }
    }

    private void displaySave()
    {
        string output = "";
        for (int i = 7; i >= 0; i--)
        {
            for (int j = 3; j >= 0; j--)
                output += ((saveBits & (1 << (4 * i + j))) == (1 << (4 * i + j))) ? 1 : 0;
            output += " ";
        }
        Debug.Log(output);
    }
    public int getAchievementSave() { return saveBits; }
}
