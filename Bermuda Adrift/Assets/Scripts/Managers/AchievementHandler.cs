using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AchievementHandler : MonoBehaviour
{
    public static event Action<string, string> OnAchievementUnlocked;

    [SerializeField] GameObject achievementPopup;
    Stack<Achievement> achievementLog;
    bool achievementPlaying;

    private void Update()
    {
        if (achievementLog == null)
            achievementLog = new Stack<Achievement>();

        if (!achievementPlaying && achievementLog != null && achievementLog.Count > 0)
            StartCoroutine(popupAchievement(achievementLog.Pop()));
    }
    void unlockAchievement(Achievement toBeUnlocked)
    {
        if (toBeUnlocked.getUnlocked()) return;  //Return if the achievement is already unlocked

        Debug.Log("Unlocking achievement " + toBeUnlocked);

        toBeUnlocked.setUnlocked(true);

        if (toBeUnlocked.getAssociatedTower() != null)
            toBeUnlocked.getAssociatedTower().setUnlocked(true);
        else if (toBeUnlocked.getAssociatedBarrier() != null)
            toBeUnlocked.getAssociatedBarrier().setUnlock(true);
        else if (toBeUnlocked.getAssociatedCharacter() != null)
            toBeUnlocked.getAssociatedCharacter().setUnlock(true);
        else if (toBeUnlocked.getAssociatedCenterpiece() != null)
            toBeUnlocked.getAssociatedCenterpiece().setUnlock(true);
        //Some achievements have none, unlocks anyway

        //Add points
        FindObjectOfType<SkillManager>().addPoints(toBeUnlocked.getSkillPoints());

        //Play achievement popup
        if (achievementPopup != null)
            achievementLog.Push(toBeUnlocked);

        if (FindObjectOfType<DataPersistenceManager>() != null)
            FindObjectOfType<DataPersistenceManager>().SaveGame();
    }
    IEnumerator popupAchievement(Achievement achievement)
    {
        achievementPlaying = true;

        achievementPopup.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = achievement.getName();
        achievementPopup.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = achievement.getDescription();
        achievementPopup.GetComponent<Animator>().Play("AchievementPopup");

        yield return new WaitForSeconds(5f);

        achievementPopup.GetComponent<Animator>().Play("AchievementPopdown");

        yield return new WaitForSeconds(1);

        achievementPlaying = false;
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
