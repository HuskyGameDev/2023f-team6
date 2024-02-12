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
    [SerializeField] TextMeshProUGUI towerName;
    [SerializeField] TextMeshProUGUI towerCost;
    [SerializeField] Canvas logbookCanvas;
    [SerializeField] GameObject logbookGrid;

    Resolution[] resolutions;

    private void OnEnable()
    {
        EnemyManager.onRoundEnd += addToRound;
        EnemyManager.onRoundEnd += clearTowerTextInt;
        EnemyManager.onEnemyDeath += updateEnemyCount;
        EnemyManager.allEnemiesSpawned += updateEnemyCount;
        GameManager.onScrapCollect += addScrapGUI;
        GameManager.onXPCollect += addXPGUI;
        GameManager.onRoundEnd += enableStrategizeGUI;
        GameManager.OnRoundStart += activateCooldownIndicators;
        GameManager.OnGameEnd += enableGameOverUI;
        GameManager.OnLevelUp += addLevelGUI;
        BuildManager.OnTowerPicked += cancelPrompt;
        BuildManager.OnBarrierPicked += cancelPromptB;
        ButtonHover.OnHoverEnter += changeTowerText;
        ButtonHover.OnHoverEnterB += changeTowerText;
        TowerAI.OnCancel += clearTowerText;
    }

    private void OnDisable()
    {
        EnemyManager.onRoundEnd -= addToRound;
        EnemyManager.onRoundEnd -= clearTowerTextInt;
        EnemyManager.onEnemyDeath -= updateEnemyCount;
        EnemyManager.allEnemiesSpawned -= updateEnemyCount;
        GameManager.onScrapCollect -= addScrapGUI;
        GameManager.onXPCollect -= addXPGUI;
        GameManager.onRoundEnd -= enableStrategizeGUI;
        GameManager.OnRoundStart -= activateCooldownIndicators;
        GameManager.OnGameEnd -= enableGameOverUI;
        GameManager.OnLevelUp -= addLevelGUI;
        BuildManager.OnTowerPicked -= cancelPrompt;
        BuildManager.OnBarrierPicked -= cancelPromptB;
        ButtonHover.OnHoverEnter -= changeTowerText;
        ButtonHover.OnHoverEnterB -= changeTowerText;
        TowerAI.OnCancel -= clearTowerText;
    }

    private void Start()
    {
        resolutions = Screen.resolutions;

        clearTowerText();
    }

    public void LoadScene(int sceneIndex)
    {
        if (FindObjectOfType<SettingsTracker>() == null || FindObjectOfType<SettingsTracker>().getCharacter().getUnlocked())
            SceneManager.LoadScene(sceneIndex);
        else
            Debug.Log("Not unlocked!");
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

    public void changeTowerText(Tower tower)
    {
        if (tower == null)
        {
            towerName.text = "Owned Blueprints";
            towerCost.text = "";
            return;
        }

        towerName.text = tower.getName();

        if (gameObject.GetComponent<GameManager>().cost(tower.getCost()))
            towerCost.color = Color.yellow;
        else
            towerCost.color = Color.red;

        towerCost.text = "Cost: " + tower.getCost().ToString() + " Scrap";
    }

    public void changeTowerText(BarrierScriptable barrier)
    {
        if (barrier == null)
        {
            towerName.text = "Owned Blueprints";
            towerCost.text = "";
            return;
        }

        towerName.text = barrier.getName();

        if (gameObject.GetComponent<GameManager>().cost(barrier.getCost()))
            towerCost.color = Color.yellow;
        else
            towerCost.color = Color.red;

        towerCost.text = "Cost: " + barrier.getCost().ToString() + " Scrap";
    }
    void clearTowerTextInt(int i) { clearTowerText(); }
    public void clearTowerText()
    {
        if (towerName != null && towerCost != null)
        {
            towerName.text = "Owned Blueprints";
            towerCost.text = "";
        }
    }
    public void cancelPrompt(Tower tower)
    {
        towerName.text = "Right Click";
        towerCost.text = "To Cancel";
        towerCost.color = Color.white;
    }
    public void cancelPromptB(BarrierScriptable b)
    {
        towerName.text = "Right Click";
        towerCost.text = "To Cancel";
        towerCost.color = Color.white;
    }

    public void enableCanvas(Canvas c)
    {
        c.enabled = true;
    }

    public void disableCanvas(Canvas c)
    {
        c.enabled = false;
    }

    public void openLogbook()
    {
        logbookCanvas.enabled = true;
        logbookGrid.SetActive(true);
    }

    public void closeLogbook()
    {
        logbookCanvas.enabled = false;
        logbookGrid.SetActive(false);
    }
}
