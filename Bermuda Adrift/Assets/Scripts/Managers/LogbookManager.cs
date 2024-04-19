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
    public static event Action<Player> CharacterUnlocked;
    public static event Action logbookFull;

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
        AI.OnUnlockEnemyDeath += unlockEnemy;
        AI.OnUnlockEnemyDeath += unlockCharacter;
        TowerAI.OnDoubleUpgraded += unlockTower;

        UnlockableButtonEnemy.UnlockableEnemyButtonClicked += openEnemyInformation;
        UnlockableButtonTower.UnlockableTowerButtonClicked += openTowerInformation;
        UnlockableButtonCharacter.UnlockableTowerButtonClicked += openCharacterInformation;
    }

    private void OnDisable()
    {
        AI.OnUnlockEnemyDeath -= unlockEnemy;
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

            if (e.getLogged())
            {
                StartCoroutine(delayedUnlock(e));
            }
        }

        foreach (Enemy e in allBosses)
        {
            GameObject newButton = Instantiate(contentButtonEne, contentBos);
            newButton.GetComponent<UnlockableButtonEnemy>().enemy = e;
            newButton.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "???";
            newButton.transform.GetChild(0).GetComponent<Image>().sprite = e.getSprite();

            if (e.getLogged())
            {
                StartCoroutine(delayedUnlock(e));
            }
        }

        foreach (Tower t in allTowers)
        {
            GameObject newButton = Instantiate(contentButtonTow, contentTow);
            newButton.GetComponent<UnlockableButtonTower>().tower = t;
            newButton.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "???";
            newButton.transform.GetChild(0).GetComponent<Image>().sprite = t.getImage();

            if (t.getLogged())
            {
                StartCoroutine(delayedUnlock(t));
            }
        }

        foreach (Player p in allCharacters)
        {
            GameObject newButton = Instantiate(contentButtonCha, contentCha);
            newButton.GetComponent<UnlockableButtonCharacter>().player = p;
            newButton.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "???";
            newButton.transform.GetChild(0).GetComponent<Image>().sprite = p.getMainBodySprite();

            if (p.getLogged())
            {
                StartCoroutine(delayedUnlock(p));
            }
        }

        //refreshLogbook();

        txtLoggedBos.GetComponent<TextMeshProUGUI>().text = unlockedBosses.Count + " / " + allBosses.Length;
        txtLoggedEne.GetComponent<TextMeshProUGUI>().text = unlockedEnemies.Count + " / " + allEnemies.Length;
        txtLoggedTow.GetComponent<TextMeshProUGUI>().text = unlockedTowers.Count + " / " + allTowers.Length;
        txtLoggedCha.GetComponent<TextMeshProUGUI>().text = unlockedCharacters.Count + " / " + allCharacters.Length;
    }
    IEnumerator delayedUnlock(Enemy e)
    {
        yield return new WaitForEndOfFrame();
        unlockEnemy(e);
    }
    IEnumerator delayedUnlock(Tower t)
    {
        yield return new WaitForEndOfFrame();
        unlockTower(t);
    }
    IEnumerator delayedUnlock(Player p)
    {
        yield return new WaitForEndOfFrame();
        unlockCharacter(p);
    }

    public void unlockEnemy(Enemy e)
    {
        if (e.getType() == Enemy.Types.WaterBoss || e.getType() == Enemy.Types.AirborneBoss)
        {
            if (!unlockedBosses.Contains(e))
            {
                unlockedBosses.Add(e);
                e.setLogged(true);

                //Debug.Log("Added Boss: " + e.getName());

                txtLoggedBos.GetComponent<TextMeshProUGUI>().text = unlockedBosses.Count + " / " + allBosses.Length;
            }
        }
        else
        {
            if (!unlockedEnemies.Contains(e))
            {
                unlockedEnemies.Add(e);
                e.setLogged(true);

                //Debug.Log("Added Enemy: " + e.getName());

                txtLoggedEne.GetComponent<TextMeshProUGUI>().text = unlockedEnemies.Count + " / " + allEnemies.Length;
            }
        }

        if (logbookFilled())
            logbookFull?.Invoke();
        EnemyUnlocked?.Invoke(e);
    }

    public void unlockTower(Tower t)
    {
        if (!unlockedTowers.Contains(t))
        {
            unlockedTowers.Add(t);
            t.setLogged(true);

            //Debug.Log("Added Tower: " + t.getName());

            if (logbookFilled())
                logbookFull?.Invoke();

            txtLoggedTow.GetComponent<TextMeshProUGUI>().text = unlockedTowers.Count + " / " + allTowers.Length;
        }

        if (logbookFilled())
            logbookFull?.Invoke();
        TowerUnlocked?.Invoke(t);
    }
    public void unlockCharacter(Enemy enemy)
    {
        if (enemy.getName().CompareTo("The Maestro") == 0)
            unlockCharacter(FindObjectOfType<Attack>().getCharacter());
    }
    public void unlockCharacter(Player p)
    {
        if (!unlockedCharacters.Contains(p))
        {
            unlockedCharacters.Add(p);
            p.setLogged(true);

            if (logbookFilled())
                logbookFull?.Invoke();

            //Debug.Log("Added Character: " + p.getName());
            txtLoggedCha.GetComponent<TextMeshProUGUI>().text = unlockedCharacters.Count + " / " + allCharacters.Length;
        }

        if (logbookFilled())
            logbookFull?.Invoke();
        CharacterUnlocked?.Invoke(p);
    }

    /*
    private void refreshLogbook()
    {
        foreach (Enemy e in allEnemies)
        {
            if (e.getLogged())
                unlockEnemy(e);
        }

        foreach (Enemy boss in allBosses)
        {
            if (boss.getLogged())
                unlockEnemy(boss);
        }

        foreach (Tower t in allTowers)
        {
            if (t.getLogged())
                unlockTower(t);
        }

        foreach (Player p in allCharacters)
        {
            if (p.getLogged())
                unlockCharacter(p);
        }
    }
    */

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
    public void openTowerInformation(Tower t)
    {
        informationMask.SetActive(true);
        entryDescription.GetComponent<TextMeshProUGUI>().text = t.getLog().Replace("\\n", "\n\n");
        entryName.GetComponent<TextMeshProUGUI>().text = "Tower: " + t.getName();
        entryIcon.GetComponent<Image>().sprite = t.getImage();
        canvasMain.enabled = false;
        canvasEne.enabled = false;
        canvasBos.enabled = false;
        canvasTow.enabled = false;
        canvasCha.enabled = false;
    }
    public void openCharacterInformation(Player c)
    {
        informationMask.SetActive(true);
        entryDescription.GetComponent<TextMeshProUGUI>().text = c.getLog();
        entryName.GetComponent<TextMeshProUGUI>().text = "Character: " + c.getName();
        entryIcon.GetComponent<Image>().sprite = c.getMainBodySprite();
        canvasMain.enabled = false;
        canvasEne.enabled = false;
        canvasBos.enabled = false;
        canvasTow.enabled = false;
        canvasCha.enabled = false;
    }

    bool logbookFilled()
    {
        if (unlockedBosses.Count >= allBosses.Length
            &&
            unlockedEnemies.Count >= allEnemies.Length
            &&
            unlockedTowers.Count >= allTowers.Length
            &&
            unlockedCharacters.Count >= allCharacters.Length)
            return true;
        else
        {
            Debug.Log("Returning false");
            return false;
        }
    }
}
