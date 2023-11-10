using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GUIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI roundNo;
    [SerializeField] TextMeshProUGUI enemyCnt;
    [SerializeField] TextMeshProUGUI scrapAmt;
    [SerializeField] TextMeshProUGUI xpAmt;
    [SerializeField] TextMeshProUGUI playerLvl;
    [SerializeField] Slider xpSlider;
    [SerializeField] Button readyBtn;
    [SerializeField] GameObject gameOverGUI;
    [SerializeField] GameObject CooldownIndicatorGUI;

    Resolution[] resolutions;

    private void OnEnable()
    {
        EnemyManager.onRoundEnd += addToRound;
        EnemyManager.onEnemyDeath += updateEnemyCount;
        EnemyManager.onEnemySpawn += updateEnemyCount;
        GameManager.onScrapCollect += addScrapGUI;
        GameManager.onXPCollect += addXPGUI;
        GameManager.onRoundEnd += enableStrategizeGUI;
        GameManager.OnRoundStart += activateCooldownIndicators;
        GameManager.OnGameEnd += enableGameOverUI;
        GameManager.OnLevelUp += addLevelGUI;
    }

    private void OnDisable()
    {
        EnemyManager.onRoundEnd -= addToRound;
        EnemyManager.onEnemyDeath -= updateEnemyCount;
        EnemyManager.onEnemySpawn -= updateEnemyCount;
        GameManager.onScrapCollect -= addScrapGUI;
        GameManager.onXPCollect -= addXPGUI;
        GameManager.onRoundEnd -= enableStrategizeGUI;
        GameManager.OnRoundStart -= activateCooldownIndicators;
        GameManager.OnGameEnd -= enableGameOverUI;
        GameManager.OnLevelUp -= addLevelGUI;
    }

    private void Start()
    {
        resolutions = Screen.resolutions;
        xpSlider.interactable = false;
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

    public void addXPGUI(int xp, float xpNeeded)
    {
        xpAmt.text = "XP: " + xp.ToString();

        xpSlider.maxValue = xpNeeded;
        xpSlider.value = xp;
    }

    public void addLevelGUI(int level)
    {
        playerLvl.text = "Lv. " + level.ToString();
    }

    public void activateCooldownIndicators()
    {
        CooldownIndicatorGUI.SetActive(true);
    }

    public void enableStrategizeGUI()
    {
        readyBtn.gameObject.SetActive(true);
        CooldownIndicatorGUI.SetActive(false);
    }

    public void enableGameOverUI()
    {
        Time.timeScale = 0;
        gameOverGUI.SetActive(true);
    }
}
