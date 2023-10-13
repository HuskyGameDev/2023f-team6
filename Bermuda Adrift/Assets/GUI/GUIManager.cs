using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Text.RegularExpressions;

public class GUIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI roundNo;
    [SerializeField] TextMeshProUGUI enemyCnt;
    [SerializeField] TextMeshProUGUI scrapAmt;
    [SerializeField] TextMeshProUGUI xpAmt;

    Resolution[] resolutions;

    private void OnEnable()
    {
        EnemyManager.onRoundEnd += addToRound;
        EnemyManager.onEnemyDeath += updateEnemyCount;
        EnemyManager.onEnemySpawn += updateEnemyCount;
        GameManager.onScrapCollect += addScrap;
        GameManager.onXPCollect += addXP;
    }

    private void OnDisable()
    {
        EnemyManager.onRoundEnd -= addToRound;
        EnemyManager.onEnemyDeath -= updateEnemyCount;
        EnemyManager.onEnemySpawn -= updateEnemyCount;
        GameManager.onScrapCollect -= addScrap;
        GameManager.onXPCollect -= addXP;
    }

    private void Start()
    {
        resolutions = Screen.resolutions;
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void SetActiveTrue(GameObject screen)
    {
        screen.SetActive(true);
    }

    public void SetActiveFalse(GameObject screen)
    {
        screen.SetActive(false);
    }

    public void addToRound(int round)
    {
        roundNo.text = "Round " + round;
    }

    public void updateEnemyCount(int count)
    {
       enemyCnt.text = count.ToString();
    }

    public void addScrap(int scrap)
    {
        string regexMatch = Regex.Match(scrapAmt.text, @"\d+").Value;
        int resultNum = int.Parse(regexMatch);
        scrapAmt.text = "Scrap: " + (resultNum += scrap).ToString();
    }

    public void addXP(int xp)
    {
        string regexMatch = Regex.Match(xpAmt.text, @"\d+").Value;
        int resultNum = int.Parse(regexMatch);
        xpAmt.text = "XP: " + (resultNum += xp).ToString();
    }
}
