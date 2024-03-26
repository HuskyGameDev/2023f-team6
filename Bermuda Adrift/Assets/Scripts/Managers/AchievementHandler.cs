using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AchievementHandler : MonoBehaviour
{
    public static event Action<string, string> OnAchievementUnlocked;

    [SerializeField] GameObject achievementPopup;

    void unlockAchievement(Achievement toBeUnlocked)
    {
        if (toBeUnlocked.getUnlocked()) return;  //Return if the achievement is already unlocked

        toBeUnlocked.setUnlocked(true);

        if (toBeUnlocked.getAssociatedTower() != null)
            toBeUnlocked.getAssociatedTower().setUnlocked(true);
        else if (toBeUnlocked.getAssociatedBarrier() != null)
            toBeUnlocked.getAssociatedBarrier().setUnlock(true);
        else if (toBeUnlocked.getAssociatedCharacter() != null)
            toBeUnlocked.getAssociatedCharacter().setUnlock(true);
        //Some achievements have none, unlocks anyway

        //Play achievement popup
        StartCoroutine(popupAchievement(toBeUnlocked));
    }
    IEnumerator popupAchievement(Achievement achievement)
    {
        achievementPopup.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = achievement.getName();
        achievementPopup.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = achievement.getDescription();
        achievementPopup.GetComponent<Animator>().Play("AchievementPopup");

        yield return new WaitForSeconds(5f);

        achievementPopup.GetComponent<Animator>().Play("AchievementPopdown");
    }



    /*
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
    */
}
