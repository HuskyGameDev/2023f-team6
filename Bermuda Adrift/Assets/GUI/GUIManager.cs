using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GUIManager : MonoBehaviour
{

    Resolution[] resolutions;

    private void OnEnable()
    {
        EnemyManager.onRoundEnd += addToRound;
        EnemyManager.onEnemyDeath += updateEnemyCount;
        EnemyManager.onEnemySpawn += updateEnemyCount;
    }

    private void OnDisable()
    {
        EnemyManager.onRoundEnd -= addToRound;
        EnemyManager.onEnemyDeath -= updateEnemyCount;
        EnemyManager.onEnemySpawn -= updateEnemyCount;
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
        //obj[0].GetComponent<TextMeshProUGUI>().text = "Round " + round;
    }

    public void updateEnemyCount(int count)
    {
       // obj[1].GetComponent<TextMeshProUGUI>().text = "Enemies Remaining: " + count;
    }
}
