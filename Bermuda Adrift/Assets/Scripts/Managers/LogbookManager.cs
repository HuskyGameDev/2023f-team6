using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogbookManager : MonoBehaviour
{
    public static event Action<Enemy> enemyUnlocked;

    [SerializeField] RectTransform contentEne;
    [SerializeField] RectTransform contentBos;
    [SerializeField] RectTransform contentTow;
    [SerializeField] RectTransform contentCha;
    [SerializeField] GameObject contentButton;
    [SerializeField] GameObject informationMask;
    [SerializeField] GameObject entryDescription;
    [SerializeField] GameObject entryName;
    [SerializeField] GameObject entryIcon;
    [SerializeField] GameObject txtLoggedEne;
    [SerializeField] GameObject txtLoggedBos;
    [SerializeField] GameObject txtLoggedTow;
    [SerializeField] GameObject txtLoggedCha;

    public Enemy[] allEnemies;
    public Enemy[] allBosses;
    public Tower[] allTowers;
    public CharacterInfo[] allCharacters;
    public List<Enemy> unlockedEnemies;
    public List<Enemy> unlockedBosses;
    public List<Tower> unlockedTowers;
    public List<CharacterInfo> unlockedCharacters;
    private void OnEnable()
    {
        EnemyManager.onEnemySpawn += unlockEnemy;
        UnlockableButton.unlockableButtonClicked += openInformation;
    }

    private void OnDisable()
    {
        EnemyManager.onEnemySpawn -= unlockEnemy;
        UnlockableButton.unlockableButtonClicked -= openInformation;
    }

    private void Start()
    {
        foreach(Enemy e in allEnemies)
        {
            GameObject newButton = Instantiate(contentButton, contentEne);
            newButton.GetComponent<UnlockableButton>().enemy = e;
            newButton.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "???";
            newButton.transform.GetChild(0).GetComponent<Image>().sprite = e.getSprite();
        }

        foreach (Enemy e in allBosses)
        {
            GameObject newButton = Instantiate(contentButton, contentBos);
            newButton.GetComponent<UnlockableButton>().enemy = e;
            newButton.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "???";
            newButton.transform.GetChild(0).GetComponent<Image>().sprite = e.getSprite();
        }
    }

    public void unlockEnemy(Enemy e)
    {
        if (!unlockedEnemies.Contains(e))
        {
            unlockedEnemies.Add(e);

            Debug.Log("Added Enemy: " + e.getName());

            txtLoggedEne.GetComponent<TextMeshProUGUI>().text = unlockedEnemies.Count + " / " + allEnemies.Length;

            enemyUnlocked?.Invoke(e);
        }
    }

    public void openInformation(Enemy e)
    {
        informationMask.SetActive(true);
        entryDescription.GetComponent<TextMeshProUGUI>().text = e.getDescription();
        entryName.GetComponent<TextMeshProUGUI>().text = "Enemy: " + e.getName();
        entryIcon.GetComponent<Image>().sprite = e.getSprite();

    }
}
