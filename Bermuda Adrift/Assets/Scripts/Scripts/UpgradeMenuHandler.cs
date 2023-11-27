using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UpgradeMenuHandler : MonoBehaviour
{
    public static event Action OnDestroy;

    private TextMeshProUGUI headerText;
    private GameObject OneOption;
    private GameObject TwoOptions;
    private GameObject upgrade1;
    private GameObject upgrade2;
    private GameObject destroyButton;
    private TowerAI currentTower;

    private void Start()
    {
        headerText = transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        OneOption = transform.GetChild(1).gameObject;
        TwoOptions = transform.GetChild(2).gameObject;
        upgrade1 = transform.GetChild(3).gameObject;
        upgrade2 = transform.GetChild(4).gameObject;
        destroyButton = transform.GetChild(5).gameObject;
    }

    private void OnEnable()
    {
        AnimationHandler.onUpgradeOpened += updateMenu;
        AnimationHandler.onUpgradeClosed += nonInteractable;
        TowerAI.OnUpgraded += updateMenu;
    }
    private void OnDisable()
    {
        AnimationHandler.onUpgradeOpened -= updateMenu;
        AnimationHandler.onUpgradeClosed -= nonInteractable;
        TowerAI.OnUpgraded -= updateMenu;
    }

    void updateMenu(TowerAI towerAI)
    {
        //Setup buton descriptions for hover text
        BroadcastMessage("setTower", towerAI.getTower());
        BroadcastMessage("setUpgradeLevel", towerAI.getUpgradeLevel());

        int upgradeLevel = towerAI.getUpgradeLevel();
        Tower tower = towerAI.getTower();
        currentTower = towerAI;

        //Make buttons interactable
        upgrade1.GetComponent<Button>().interactable = true;
        upgrade2.GetComponent<Button>().interactable = true;
        destroyButton.GetComponent<Button>().interactable = true;
        upgrade1.SetActive(true);

        if (upgradeLevel >= 4)  //Displays only the destroy button when it's fully upgraded
        {
            upgrade1.SetActive(false);
            if (upgradeLevel == 4)
                headerText.text = tower.UA2getName();
            else
                headerText.text = tower.UB2getName();

            upgrade2.SetActive(false);
            OneOption.SetActive(true);
            TwoOptions.SetActive(false);

            destroyButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(17, -243.2f);
            destroyButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Destroy: " + (getReturnScrap(tower, upgradeLevel) / 2) + " scrap";
        }
        else if (upgradeLevel == 1)  //Have 2 options when the tower has one upgrade
        {

            upgrade2.SetActive(true);
            OneOption.SetActive(false);
            TwoOptions.SetActive(true);
            destroyButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(17, -249.7f);
            upgrade1.SendMessage("setUpgradeLevel", 2);
            upgrade2.SendMessage("setUpgradeLevel", 3);

            headerText.text = "Lvl 1 " + tower.getName();
            destroyButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Destroy: " + (getReturnScrap(tower, upgradeLevel) / 2) + " scrap";

            upgrade1.transform.GetChild(1).GetComponent<Image>().sprite = tower.UA1getImage();
            upgrade1.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = tower.UA1getName();
            upgrade1.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Cost: " + tower.UA1getCost() + " scrap";

            upgrade2.transform.GetChild(1).GetComponent<Image>().sprite = tower.UB1getImage();
            upgrade2.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = tower.UB1getName();
            upgrade2.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Cost: " + tower.UB1getCost() + " scrap";
        }
        else
        {
            upgrade2.SetActive(false);
            OneOption.SetActive(true);
            TwoOptions.SetActive(false);
            destroyButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(17, -245.45f);
            upgrade1.SendMessage("setUpgradeLevel", towerAI.getUpgradeLevel() + 1);

            if (upgradeLevel == 0)
            {
                upgrade1.transform.GetChild(1).GetComponent<Image>().sprite = tower.U1getImage();
                upgrade1.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = tower.U1getName();
                upgrade1.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Cost: " + tower.U1getCost() + " scrap";

                headerText.text = tower.getName();
                destroyButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Destroy: " + (getReturnScrap(tower, upgradeLevel) / 2) + " scrap";
            } else if (upgradeLevel == 2)
            {
                upgrade1.transform.GetChild(1).GetComponent<Image>().sprite = tower.UA2getImage();
                upgrade1.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = tower.UA2getName();
                upgrade1.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Cost: " + tower.UA2getCost() + " scrap";

                headerText.text = "Lvl 2 " + tower.getName();
                destroyButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Destroy: " + (getReturnScrap(tower, upgradeLevel) / 2) + " scrap";
            } else if (upgradeLevel == 3)
            {
                upgrade1.transform.GetChild(1).GetComponent<Image>().sprite = tower.UB2getImage();
                upgrade1.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = tower.UB2getName();
                upgrade1.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Cost: " + tower.UB2getCost() + " scrap";

                headerText.text = "Lvl 2 " + tower.getName();
                destroyButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Destroy: " + (getReturnScrap(tower, upgradeLevel) / 2) + " scrap";
            }
        }
    }
    private int getReturnScrap(Tower tower, int upgradeLevel)
    {
        int returnScrap = 0;
        if (upgradeLevel >= 0)  //Unupgraded
            returnScrap += tower.getCost();
        if (upgradeLevel >= 1) //1 upgrade
            returnScrap += tower.U1getCost();
        if (upgradeLevel == 2 || upgradeLevel == 4) //A1 upgrade
            returnScrap += tower.UA1getCost();
        if (upgradeLevel == 3 || upgradeLevel == 5) //B1 upgrade
            returnScrap += tower.UB1getCost();
        if (upgradeLevel == 4)  //A2 upgrade
            returnScrap += tower.UA2getCost();
        if (upgradeLevel == 5)  //B2 upgrade
            returnScrap += tower.UB2getCost();

        return returnScrap;
    }

    private void nonInteractable()
    {
        upgrade1.GetComponent<Button>().interactable = false;
        upgrade2.GetComponent<Button>().interactable = false;
        destroyButton.GetComponent<Button>().interactable = false;
    }
    public void destroyClicked()
    {
        OnDestroy?.Invoke();
        currentTower.destroyTower();
    }
    public void upgrade(bool path)
    {
        currentTower.SendMessage("upgrade", path);
    }
    public void closeMenu()
    {
        OnDestroy?.Invoke();
    }
}
