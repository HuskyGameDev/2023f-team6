using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class GUIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI roundNo;
    [SerializeField] TextMeshProUGUI enemyCnt;
    [SerializeField] TextMeshProUGUI scrapAmt;
    [SerializeField] TextMeshProUGUI xpAmt;
    [SerializeField] Button readyBtn;

    Resolution[] resolutions;

    private void OnEnable()
    {
        EnemyManager.onRoundEnd += addToRound;
        EnemyManager.onEnemyDeath += updateEnemyCount;
        EnemyManager.onEnemySpawn += updateEnemyCount;
        GameManager.onScrapCollect += addScrapGUI;
        GameManager.onXPCollect += addXPGUI;
        GameManager.onRoundEnd += enableStrategizeGUI;
    }

    private void OnDisable()
    {
        EnemyManager.onRoundEnd -= addToRound;
        EnemyManager.onEnemyDeath -= updateEnemyCount;
        EnemyManager.onEnemySpawn -= updateEnemyCount;
        GameManager.onScrapCollect -= addScrapGUI;
        GameManager.onXPCollect -= addXPGUI;
        GameManager.onRoundEnd -= enableStrategizeGUI;
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

    public void addScrapGUI(int scrap)
    {
        scrapAmt.text = "Scrap: " + scrap.ToString();
    }

    public void addXPGUI(int xp)
    {
        xpAmt.text = "XP: " + xp.ToString();
    }

    public void enableStrategizeGUI()
    {

    }
}
