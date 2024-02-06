using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogbookManager : MonoBehaviour
{
    public static event Action<Enemy> EnemyUnlocked;
    public static event Action<Tower> TowerUnlocked;

    [SerializeField] RectTransform contentEne;
    [SerializeField] RectTransform contentBos;
    [SerializeField] RectTransform contentTow;
    [SerializeField] RectTransform contentCha;
    [SerializeField] Canvas canvasMain;
    [SerializeField] Canvas canvasEne;
    [SerializeField] Canvas canvasBos;
    [SerializeField] Canvas canvasTow;
    [SerializeField] Canvas canvasCha;
    [SerializeField] GameObject contentButtonEne;
    [SerializeField] GameObject contentButtonTow;
    [SerializeField] GameObject contentButtonCha;
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
    public Player[] allCharacters;
    public List<Enemy> unlockedEnemies;
    public List<Enemy> unlockedBosses;
    public List<Tower> unlockedTowers;
    public List<Player> unlockedCharacters;
    private void OnEnable()
    {
        EnemyManager.onEnemySpawn += unlockEnemy;
        UnlockableButtonEnemy.UnlockableEnemyButtonClicked += openEnemyInformation;
    }

    private void OnDisable()
    {
        EnemyManager.onEnemySpawn -= unlockEnemy;
        UnlockableButtonEnemy.UnlockableEnemyButtonClicked += openEnemyInformation;
    }

    private void Start()
    {
        foreach(Enemy e in allEnemies)
        {
            GameObject newButton = Instantiate(contentButtonEne, contentEne);
            newButton.GetComponent<UnlockableButtonEnemy>().enemy = e;
            newButton.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "???";
            newButton.transform.GetChild(0).GetComponent<Image>().sprite = e.getSprite();
        }

        foreach (Enemy e in allBosses)
        {
            GameObject newButton = Instantiate(contentButtonEne, contentBos);
            newButton.GetComponent<UnlockableButtonEnemy>().enemy = e;
            newButton.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "???";
            newButton.transform.GetChild(0).GetComponent<Image>().sprite = e.getSprite();
        }

        foreach (Tower t in allTowers)
        {
            GameObject newButton = Instantiate(contentButtonTow, contentTow);
            newButton.GetComponent<UnlockableButtonTower>().tower = t;
            newButton.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "???";
            newButton.transform.GetChild(0).GetComponent<Image>().sprite = t.getImage();
        }

        foreach (Player p in allCharacters)
        {
            GameObject newButton = Instantiate(contentButtonCha, contentCha);
            newButton.GetComponent<UnlockableButtonCharacter>().player = p;
            newButton.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "???";
            newButton.transform.GetChild(0).GetComponent<Image>().sprite = p.getMainBodySprite();
        }

        txtLoggedBos.GetComponent<TextMeshProUGUI>().text = unlockedBosses.Count + " / " + allBosses.Length;
        txtLoggedEne.GetComponent<TextMeshProUGUI>().text = unlockedEnemies.Count + " / " + allEnemies.Length;
        txtLoggedTow.GetComponent<TextMeshProUGUI>().text = unlockedTowers.Count + " / " + allTowers.Length;
        txtLoggedCha.GetComponent<TextMeshProUGUI>().text = unlockedCharacters.Count + " / " + allCharacters.Length;
    }

    public void unlockEnemy(Enemy e)
    {
        if (e.getType() == Enemy.Types.WaterBoss || e.getType() == Enemy.Types.AirborneBoss)
        {
            if (!unlockedBosses.Contains(e))
            {
                unlockedBosses.Add(e);

                Debug.Log("Added Boss: " + e.getName());

                txtLoggedBos.GetComponent<TextMeshProUGUI>().text = unlockedBosses.Count + " / " + allBosses.Length;
            }
        }
        else
        {
            if (!unlockedEnemies.Contains(e))
            {
                unlockedEnemies.Add(e);

                Debug.Log("Added Enemy: " + e.getName());

                txtLoggedEne.GetComponent<TextMeshProUGUI>().text = unlockedEnemies.Count + " / " + allEnemies.Length;
            }
        }

        EnemyUnlocked?.Invoke(e);
    }

    public void unlockTower(Tower t)
    {
        if (!unlockedTowers.Contains(t))
        {
            unlockedTowers.Add(t);

            Debug.Log("Added Tower: " + t.getName());

            txtLoggedTow.GetComponent<TextMeshProUGUI>().text = unlockedTowers.Count + " / " + allTowers.Length;
        }
    }

    public void unlockCharacter(Player p)
    {
        if (!unlockedCharacters.Contains(p))
        {
            unlockedCharacters.Add(p);

            Debug.Log("Added Character: " + p.getName());
            txtLoggedCha.GetComponent<TextMeshProUGUI>().text = unlockedCharacters.Count + " / " + allCharacters.Length;
        }
    }

    public void openEnemyInformation(Enemy e)
    {
        informationMask.SetActive(true);
        entryDescription.GetComponent<TextMeshProUGUI>().text = e.getDescription();
        entryName.GetComponent<TextMeshProUGUI>().text = "Enemy: " + e.getName();
        entryIcon.GetComponent<Image>().sprite = e.getSprite();
        canvasMain.enabled = false;
        canvasEne.enabled = false;
        canvasBos.enabled = false;
        canvasTow.enabled = false;
        canvasCha.enabled = false;
    }
}
