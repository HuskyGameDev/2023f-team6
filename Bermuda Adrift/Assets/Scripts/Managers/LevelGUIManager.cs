using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LevelGUIManager : MonoBehaviour
{
    public static event Action OnLevelUpOpen;
    public static event Action OnLevelUpClose;

    private BuildManager buildManager;
    [SerializeField] private GameObject ChoicesCanvas;
    [SerializeField] private int[] allChoicesInt;
    [SerializeField] private ScriptableObject[] allChoicesSO;
    private (int, ScriptableObject)[] allChoices;

    [SerializeField] private Tower testTower;

    private (int, ScriptableObject) option1;
    private (int, ScriptableObject) option2;
    private (int, ScriptableObject) option3;
    int previousLevel;

    Tower activeTower;
    BarrierScriptable activeBarrier;
    // Start is called before the first frame update

    private void OnEnable()
    {
        GameManager.onRoundEnd += levelUpScreen;
    }
    private void OnDisable()
    {
        GameManager.onRoundEnd -= levelUpScreen;
    }
    void Start()
    {
        buildManager = FindObjectOfType<BuildManager>();

        allChoices = new (int, ScriptableObject)[allChoicesInt.Length];
        previousLevel = 1;

        for (int i = 0; i < allChoicesSO.Length && i < allChoicesInt.Length; i++)
            allChoices[i] = (allChoicesInt[i], allChoicesSO[i]);
    }
    private void Update()
    {
        if (Input.GetKeyDown("x"))
            addAll();
        if (Input.GetKeyDown(KeyCode.Alpha5))
            buildManager.addToList((0, testTower));
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

        int[] random = new int[3];
        random[0] = Random.Range(0, allChoices.Length);
        random[1] = Random.Range(0, allChoices.Length);
        random[2] = Random.Range(0, allChoices.Length);

        //Make it 3 unique choices if possible
        while (random[0] == random[1] && allChoices.Length >= 3)
            random[1] = Random.Range(0, allChoices.Length);

        while ((random[2] == random[0] || random[2] == random[1]) && allChoices.Length >= 3)
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

            //Image
            if (allChoices[random[i]].Item1 == 0)
            {
                activeTower = (Tower)allChoices[random[i]].Item2;
                activeBarrier = null;

                grid.GetChild(3).GetComponent<Image>().sprite = activeTower.getImage();                                 //Image
                grid.GetChild(4).GetComponent<TextMeshProUGUI>().text = activeTower.getName();                          //Title
                grid.GetChild(5).GetComponent<TextMeshProUGUI>().text = activeTower.getDescription();                   //Description
                grid.GetChild(6).GetComponent<TextMeshProUGUI>().text = "Cost: " + activeTower.getCost() + " Scrap";    //Cost
            }
            else if (allChoices[random[i]].Item1 == 1)
            {
                activeTower = null;
                activeBarrier = (BarrierScriptable)allChoices[random[i]].Item2;

                grid.GetChild(3).GetComponent<Image>().sprite = activeBarrier.getThumbnail();                      //Image
                grid.GetChild(4).GetComponent<TextMeshProUGUI>().text = activeBarrier.getName();                        //Title
                grid.GetChild(5).GetComponent<TextMeshProUGUI>().text = activeBarrier.getDescription();                 //Description
                grid.GetChild(6).GetComponent<TextMeshProUGUI>().text = "Cost: " + activeBarrier.getCost() + " Scrap";  //Cost
            }

            grid.GetChild(7).GetComponent<Button>().interactable = true;
        }

        OnLevelUpOpen?.Invoke();
    }

    public void choice(int i)
    {
        if (i == 0) buildManager.addToList(option1);
        if (i == 1) buildManager.addToList(option2);
        if (i == 2) buildManager.addToList(option3);

        OnLevelUpClose?.Invoke();

        ChoicesCanvas.transform.GetChild(0).GetChild(0).GetChild(7).GetComponent<Button>().interactable = false;
        ChoicesCanvas.transform.GetChild(1).GetChild(0).GetChild(7).GetComponent<Button>().interactable = false;
        ChoicesCanvas.transform.GetChild(2).GetChild(0).GetChild(7).GetComponent<Button>().interactable = false;
    }
    public void addRandom()
    {
        (int, ScriptableObject) random = allChoices[Random.Range(0, allChoices.Length)];
        buildManager.addToList(random);
    }

    void addAll()
    {
        foreach ((int, ScriptableObject) x in allChoices)
            buildManager.addToList(x);
    }
}
