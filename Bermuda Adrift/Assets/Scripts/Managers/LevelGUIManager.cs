using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LevelGUIManager : MonoBehaviour, IDataPersistence
{
    public static event Action OnLevelUpOpen;
    public static event Action OnLevelUpClose;
    public static event Action OnLevelUpOpen1Option;
    public static event Action OnLevelUpClose1Option;
    public static event Action OnLevelUpOpen2Options;
    public static event Action OnLevelUpClose2Options;

    private BuildManager buildManager;
    [SerializeField] private GameObject ChoicesCanvas;
    [SerializeField] private Tower[] allChoices;

    private Tower option1;
    private Tower option2;
    private Tower option3;
    int previousLevel = 1;

    Tower activeTower;

    private void OnEnable()
    {
        GameManager.onRoundEnd += levelUpScreen;
    }
    private void OnDisable()
    {
        GameManager.onRoundEnd -= levelUpScreen;
    }
    private void Start()
    {
        buildManager = FindObjectOfType<BuildManager>();
    }
    private void Update()
    {
        
    }

    private void levelUpScreen()
    {
        //Delay by a couple frames so if the last enemy killed levels you up, you immediately get the level up screen
        StartCoroutine(delayedLevelUpScreen());
    }
    private IEnumerator delayedLevelUpScreen() 
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        if (FindObjectOfType<GameManager>().getLevel() <= previousLevel || buildManager.blueprintNumber() >= 10) yield break;

        previousLevel = FindObjectOfType<GameManager>().getLevel();

        if (numOfUnlocked() == 1)
        {
            foreach (Tower t in allChoices)
            {
                if (t.getUnlocked())
                {
                    option2 = t;
                    activeTower = t;
                    break;
                }
            }

            Transform grid;
            grid = ChoicesCanvas.transform.GetChild(1).GetChild(0);

            //Random background
            int randomBackground = Random.Range(0, 2);
            grid.GetChild(0).GetChild(0).gameObject.SetActive(false);
            grid.GetChild(0).GetChild(1).gameObject.SetActive(false);
            grid.GetChild(0).GetChild(randomBackground).gameObject.SetActive(true);

            grid.GetChild(3).GetComponent<Image>().sprite = activeTower.getImage();                                 //Image
            grid.GetChild(4).GetComponent<TextMeshProUGUI>().text = activeTower.getName();                          //Title
            grid.GetChild(5).GetComponent<TextMeshProUGUI>().text = activeTower.getDescription();                   //Description
            grid.GetChild(6).GetComponent<TextMeshProUGUI>().text = "Cost: " + activeTower.getCost() + " Scrap";    //Cost

            OnLevelUpOpen1Option?.Invoke();
        }
        else if (numOfUnlocked() == 2)
        {
            int[] random = new int[2];
            random[0] = Random.Range(0, allChoices.Length);
            random[1] = Random.Range(0, allChoices.Length);

            while (!allChoices[random[0]].getUnlocked())
                random[0] = Random.Range(0, allChoices.Length);

            //Make it 2 unique choices
            while (random[0] == random[1] || !allChoices[random[1]].getUnlocked())
                random[1] = Random.Range(0, allChoices.Length);

            option1 = allChoices[random[0]];
            option2 = allChoices[random[1]];

            Transform grid;
            for (int i = 0; i < 2; i++)
            {
                grid = ChoicesCanvas.transform.GetChild(i).GetChild(0);

                //Random background
                int randomBackground = Random.Range(0, 2);
                grid.GetChild(0).GetChild(0).gameObject.SetActive(false);
                grid.GetChild(0).GetChild(1).gameObject.SetActive(false);
                grid.GetChild(0).GetChild(randomBackground).gameObject.SetActive(true);

                activeTower = allChoices[random[i]];

                grid.GetChild(3).GetComponent<Image>().sprite = activeTower.getImage();                                 //Image
                grid.GetChild(4).GetComponent<TextMeshProUGUI>().text = activeTower.getName();                          //Title
                grid.GetChild(5).GetComponent<TextMeshProUGUI>().text = activeTower.getDescription();                   //Description
                grid.GetChild(6).GetComponent<TextMeshProUGUI>().text = "Cost: " + activeTower.getCost() + " Scrap";    //Cost


                grid.GetChild(7).GetComponent<Button>().interactable = true;
            }

            OnLevelUpOpen2Options?.Invoke();
        }
        else
        {
            int[] random = new int[3];
            random[0] = Random.Range(0, allChoices.Length);
            random[1] = Random.Range(0, allChoices.Length);
            random[2] = Random.Range(0, allChoices.Length);

            while (!allChoices[random[0]].getUnlocked())
                random[0] = Random.Range(0, allChoices.Length);

            //Make it 3 unique choices if possible
            while ((random[0] == random[1] || !allChoices[random[1]].getUnlocked()))
                random[1] = Random.Range(0, allChoices.Length);

            while (random[2] == random[0] || random[2] == random[1] || !allChoices[random[2]].getUnlocked())
                random[2] = Random.Range(0, allChoices.Length);

            option1 = allChoices[random[0]];
            option2 = allChoices[random[1]];
            option3 = allChoices[random[2]];

            Transform grid;
            for (int i = 0; i < 3; i++)
            {
                grid = ChoicesCanvas.transform.GetChild(i).GetChild(0);

                //Random background
                int randomBackground = Random.Range(0, 2);
                grid.GetChild(0).GetChild(0).gameObject.SetActive(false);
                grid.GetChild(0).GetChild(1).gameObject.SetActive(false);
                grid.GetChild(0).GetChild(randomBackground).gameObject.SetActive(true);

                activeTower = allChoices[random[i]];

                grid.GetChild(3).GetComponent<Image>().sprite = activeTower.getImage();                                 //Image
                grid.GetChild(4).GetComponent<TextMeshProUGUI>().text = activeTower.getName();                          //Title
                grid.GetChild(5).GetComponent<TextMeshProUGUI>().text = activeTower.getDescription();                   //Description
                grid.GetChild(6).GetComponent<TextMeshProUGUI>().text = "Cost: " + activeTower.getCost() + " Scrap";    //Cost


                grid.GetChild(7).GetComponent<Button>().interactable = true;
            }

            OnLevelUpOpen?.Invoke();
        }
    }

    public void choice(int i)
    {
        if (numOfUnlocked() == 1)
        {
            buildManager.addToList(option2);

            OnLevelUpClose1Option?.Invoke();

            ChoicesCanvas.transform.GetChild(1).GetChild(0).GetChild(7).GetComponent<Button>().interactable = false;
        }
        else if (numOfUnlocked() == 2)
        {
            if (i == 0) buildManager.addToList(option1);
            else if (i == 1) buildManager.addToList(option2);

            OnLevelUpClose2Options?.Invoke();

            ChoicesCanvas.transform.GetChild(0).GetChild(0).GetChild(7).GetComponent<Button>().interactable = false;
            ChoicesCanvas.transform.GetChild(1).GetChild(0).GetChild(7).GetComponent<Button>().interactable = false;
        }
        else
        {
            if (i == 0) buildManager.addToList(option1);
            else if (i == 1) buildManager.addToList(option2);
            else if (i == 2) buildManager.addToList(option3);

            OnLevelUpClose?.Invoke();

            ChoicesCanvas.transform.GetChild(0).GetChild(0).GetChild(7).GetComponent<Button>().interactable = false;
            ChoicesCanvas.transform.GetChild(1).GetChild(0).GetChild(7).GetComponent<Button>().interactable = false;
            ChoicesCanvas.transform.GetChild(2).GetChild(0).GetChild(7).GetComponent<Button>().interactable = false;
        }
    }
    private int numOfUnlocked()
    {
        int total = 0;
        foreach (Tower tower in allChoices)
        {
            if (tower.getUnlocked())
                total++;
            if (total > 2)
                return 3;
        }
        return total;
    }
    public void addRandom()
    {
        Tower random = allChoices[Random.Range(0, allChoices.Length)];
        buildManager.addToList(random);
    }
    public void addIndex(int i)
    {
        if (i < 0)
            i = 10;
        buildManager.addToList(allChoices[i]);
    }
    public void addAll()
    {
        foreach (Tower x in allChoices)
            buildManager.addToList(x);
    }

    public void LoadData(S_O_Saving saver)
    {
        foreach (Tower tower in allChoices)
        {
            if (saver.getTowerSave(tower.name) != null)
                tower.setUnlocked(saver.getTowerSave(tower.name).unlocked);
        }
    }

    public void SaveData(S_O_Saving saver)
    {
        foreach (Tower tower in allChoices)
        {
            if (saver.getTowerSave(tower.name) != null)
                saver.getTowerSave(tower.name).unlocked = tower.getUnlocked();
        }
    }
}
