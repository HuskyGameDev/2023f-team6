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
    private GameObject PlatformOption;
    private GameObject upgrade1;
    private GameObject upgrade2;
    private GameObject destroyButton;
    private Slider repairSlider;
    private Slider platformRepairSlider;
    private GameObject repairButton;
    private GameObject platformRepairButton;
    private GameObject priorityChanger;

    private int healedSoFar;

    private TowerAI currentTower;
    private Barriers currentBarrier;
    private Centerpiece centerpiece;

    private void Start()
    {
        headerText = transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        OneOption = transform.GetChild(1).gameObject;
        TwoOptions = transform.GetChild(3).gameObject;
        PlatformOption = transform.GetChild(2).gameObject;
        upgrade1 = transform.GetChild(4).gameObject;
        upgrade2 = transform.GetChild(5).gameObject;
        destroyButton = transform.GetChild(6).gameObject;
        repairSlider = transform.GetChild(7).GetComponent<Slider>();
        platformRepairSlider = transform.GetChild(8).GetComponent<Slider>();
        repairButton = transform.GetChild(9).gameObject;
        platformRepairButton = transform.GetChild(10).gameObject;
        priorityChanger = transform.GetChild(11).gameObject;

        closePlatformRepair();
    }

    private void OnEnable()
    {
        AnimationHandler.onUpgradeOpened += updateMenu;
        AnimationHandler.onUpgradeOpenedB += updateMenu;
        AnimationHandler.onUpgradeOpenedC += updateMenu;


        AnimationHandler.onUpgradeClosed += nonInteractable;
        TowerAI.OnUpgraded += updateMenu;
    }
    private void OnDisable()
    {
        AnimationHandler.onUpgradeOpened -= updateMenu;
        AnimationHandler.onUpgradeOpenedB -= updateMenu;
        AnimationHandler.onUpgradeOpenedC -= updateMenu;

        AnimationHandler.onUpgradeClosed -= nonInteractable;
        TowerAI.OnUpgraded -= updateMenu;
    }

    private void Update()
    {
        if (repairSlider.isActiveAndEnabled)
            updateRepairMenu();
    }

    void updateMenu(TowerAI towerAI)
    {
        //Setup button descriptions for hover text
        BroadcastMessage("setTower", towerAI.getTower());
        BroadcastMessage("setUpgradeLevel", towerAI.getUpgradeLevel());

        int upgradeLevel = towerAI.getUpgradeLevel();
        Tower tower = towerAI.getTower();

        currentTower = towerAI;
        currentBarrier = null;

        //Make buttons interactable
        upgrade1.GetComponent<Button>().interactable = true;
        upgrade2.GetComponent<Button>().interactable = true;
        destroyButton.GetComponent<Button>().interactable = true;

        upgrade1.SetActive(true);
        destroyButton.SetActive(true);
        repairSlider.gameObject.SetActive(false);
        repairButton.SetActive(false);
        priorityChanger.SetActive(true);
        priorityChanger.transform.GetChild(1).GetComponent<Button>().interactable = true;
        priorityChanger.transform.GetChild(2).GetComponent<Button>().interactable = true;
        
        
        TextMeshProUGUI text = priorityChanger.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        if (currentTower.getPriority() == TowerAI.Priority.Closest) text.text = "Closest";
        else if (currentTower.getPriority() == TowerAI.Priority.Furthest) text.text = "Furthest";
        else if (currentTower.getPriority() == TowerAI.Priority.Strongest) text.text = "Strongest";
        else if (currentTower.getPriority() == TowerAI.Priority.Fastest) text.text = "Fastest";
        else if (currentTower.getPriority() == TowerAI.Priority.OnlyWater) text.text = "Water Only";
        else if (currentTower.getPriority() == TowerAI.Priority.OnlyAir) text.text = "Air Only";
        else if (currentTower.getPriority() == TowerAI.Priority.None) text.text = "";
        else text.text = "No text";

        if (onAPlatform() != null)
            openPlatformRepair(onAPlatform());
        else
            closePlatformRepair();

        if (tower.getNoUpgrades())
        {
            upgrade1.SetActive(false);
            headerText.text = tower.getName();

            upgrade2.SetActive(false);
            OneOption.SetActive(true);
            TwoOptions.SetActive(false);

            destroyButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(17, -243.2f);
            destroyButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Dismiss";
        }
        else if (upgradeLevel >= 4)  //Displays only the destroy button when it's fully upgraded
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

            BroadcastMessage("setTower", towerAI.getTower());
            upgrade1.SendMessage("setUpgradeLevel", 2);
            upgrade2.SendMessage("setUpgradeLevel", 3);

            headerText.text = "Lvl 1 " + tower.getName();
            destroyButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Destroy: " + (getReturnScrap(tower, upgradeLevel) / 2) + " scrap";

            upgrade1.transform.GetChild(1).GetComponent<Image>().sprite = tower.UA1getImage();
            upgrade1.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = tower.UA1getName();
            upgrade1.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Cost: " + tower.UA1getCost() + " scrap";

            upgrade2.transform.GetChild(1).GetComponent<Image>().sprite = tower.UB1getImage();
            upgrade2.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = tower.UB1getName();
            upgrade2.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Cost: " + tower.UB1getCost() + " scrap";
        }
        else
        {
            upgrade2.SetActive(false);
            OneOption.SetActive(true);
            TwoOptions.SetActive(false);
            destroyButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(17, -245.45f);

            if (upgradeLevel == 0)
            {
                upgrade1.transform.GetChild(1).GetComponent<Image>().sprite = tower.U1getImage();
                upgrade1.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = tower.U1getName();
                upgrade1.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Cost: " + tower.U1getCost() + " scrap";
                upgrade1.SendMessage("setUpgradeLevel", towerAI.getUpgradeLevel() + 1);

                headerText.text = tower.getName();
                destroyButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Destroy: " + (getReturnScrap(tower, upgradeLevel) / 2) + " scrap";
            } else if (upgradeLevel == 2)
            {
                upgrade1.transform.GetChild(1).GetComponent<Image>().sprite = tower.UA2getImage();
                upgrade1.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = tower.UA2getName();
                upgrade1.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Cost: " + tower.UA2getCost() + " scrap";
                upgrade1.SendMessage("setUpgradeLevel", towerAI.getUpgradeLevel() + 2);

                headerText.text = "Lvl 2 " + tower.getName();
                destroyButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Destroy: " + (getReturnScrap(tower, upgradeLevel) / 2) + " scrap";
            } else if (upgradeLevel == 3)
            {
                upgrade1.transform.GetChild(1).GetComponent<Image>().sprite = tower.UB2getImage();
                upgrade1.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = tower.UB2getName();
                upgrade1.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Cost: " + tower.UB2getCost() + " scrap";
                upgrade1.SendMessage("setUpgradeLevel", towerAI.getUpgradeLevel() + 2);

                headerText.text = "Lvl 2 " + tower.getName();
                destroyButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Destroy: " + (getReturnScrap(tower, upgradeLevel) / 2) + " scrap";
            }
        }
        if (headerText.text.CompareTo("") == 0) headerText.text = tower.getName();
    }
    void updateMenu(Barriers barrier)
    {
        currentTower = null;
        centerpiece = null;
        currentBarrier = barrier;

        destroyButton.GetComponent<Button>().interactable = true;

        closePlatformRepair();
        

        BroadcastMessage("setBarrier", barrier.getBarrier());
        //BroadcastMessage("setUpgradeLevel", towerAI.getUpgradeLevel());
        

        if (barrier.getEffect() == BarrierScriptable.Effect.Blockade)
        {
            upgrade1.SetActive(false);
            upgrade2.SetActive(false);
            TwoOptions.SetActive(false);
            OneOption.SetActive(true);
            repairSlider.gameObject.SetActive(true);
            repairSlider.interactable = true;
            repairButton.SetActive(true);
            destroyButton.SetActive(true);
            destroyButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(17, -245.45f);

            priorityChanger.SetActive(true);
            priorityChanger.transform.GetChild(1).GetComponent<Button>().interactable = false;
            priorityChanger.transform.GetChild(2).GetComponent<Button>().interactable = false;
            priorityChanger.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Drag to repair";

            destroyButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Destroy: " + (barrier.getBarrier().getCost() / 2) + " Scrap";
            headerText.text = barrier.getName();

            repairSlider.maxValue = barrier.getMaxHealth() - barrier.getHealth();

            if (barrier.getMaxHealth() - barrier.getHealth() > 0)
            {
                repairSlider.maxValue = barrier.getMaxHealth() - barrier.getHealth();
                repairSlider.value = 0;
                priorityChanger.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Drag bar to repair";
            }
            else
            {
                priorityChanger.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "No Repair Needed";
                repairSlider.maxValue = 1;
                repairSlider.value = 0;
                repairSlider.interactable = false;
            }
            
            updateRepairMenu();
        }
        else if (barrier.getEffect() == BarrierScriptable.Effect.Effect)
        {
            upgrade1.SetActive(false);
            upgrade2.SetActive(false);
            TwoOptions.SetActive(false);
            OneOption.SetActive(true);
            destroyButton.SetActive(true);
            repairSlider.gameObject.SetActive(false);
            repairButton.SetActive(false);
            destroyButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(17, -243.2f);
            priorityChanger.SetActive(false);

            destroyButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Destroy: " + (barrier.getBarrier().getCost() / 2) + " Scrap";
            headerText.text = barrier.getName();

            
            updateRepairMenu();
        }
    }
    void updateMenu(Centerpiece center)
    {
        if (center.getCenterpiece().getName().CompareTo("Regenerating Centerpiece") == 0)   //Can't repair regen centerpiece
            return;

        centerpiece = center;
        currentTower = null;
        currentBarrier = null;

        closePlatformRepair();

        BroadcastMessage("clearDescription");

        upgrade1.SetActive(false);
        upgrade2.SetActive(false);
        TwoOptions.SetActive(false);
        OneOption.SetActive(true);
        repairSlider.gameObject.SetActive(true);
        repairSlider.interactable = true;
        repairButton.SetActive(true);
        destroyButton.SetActive(false);
        headerText.text = "Centerpiece";

        priorityChanger.SetActive(true);
        priorityChanger.transform.GetChild(1).GetComponent<Button>().interactable = false;
        priorityChanger.transform.GetChild(2).GetComponent<Button>().interactable = false;

        if (center.getMaxHealth() - center.getHealth() <= 0)
            priorityChanger.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "No Repair Needed";
        else
            priorityChanger.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Drag bar to repair";

        if (center.getMaxHealth() - center.getHealth() > 0)
        {
            repairSlider.maxValue = center.getMaxHealth() - center.getHealth();
            repairSlider.value = 0;
        }
        else
        {
            repairSlider.maxValue = 1;
            repairSlider.value = 0;
            repairSlider.interactable = false;
        }

        updateRepairMenu();
    }

    Barriers onAPlatform()
    {
        RaycastHit2D[] colliders = Physics2D.RaycastAll(currentTower.transform.position, Vector2.down, 0.5f);
        
        foreach (RaycastHit2D collider in colliders)
        {
            if (collider.collider.gameObject.GetComponent<Barriers>() != null)
            {
                Debug.Log("On platform: " + collider.collider.gameObject.GetComponent<Barriers>().getBarrier().getName());
                return collider.collider.gameObject.GetComponent<Barriers>();
            }
        }

        return null;
    }
    void openPlatformRepair(Barriers barrier)
    {
        PlatformOption.SetActive(true);
        platformRepairButton.SetActive(true);
        platformRepairSlider.gameObject.SetActive(true);
        currentBarrier = barrier;

        if (barrier.getMaxHealth() - barrier.getHealth() > 0)
        {
            platformRepairSlider.maxValue = barrier.getMaxHealth() - barrier.getHealth();
            platformRepairSlider.value = 0;
        }
        else
        {
            platformRepairSlider.maxValue = 1;
            platformRepairSlider.value = 0;
            platformRepairSlider.interactable = false;
        }

        updatePlatformRepairMenu();
    }
    void closePlatformRepair()
    {
        PlatformOption.SetActive(false);
        platformRepairButton.SetActive(false);
        platformRepairSlider.gameObject.SetActive(false);
    }

    public void updateRepairMenu()
    {
        /*
        if (repairSlider.value <= currentHealth) 
        { 
            repairSlider.value = currentHealth;
            repairButton.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Repair 0 Damage: 0 Scrap";
            return;
        }
        */

        repairButton.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Repair " + Mathf.RoundToInt(repairSlider.value) + " Damage: " + getRepairScrap(Mathf.RoundToInt(repairSlider.value)) + " Scrap";
    }
    public void updatePlatformRepairMenu()
    {
        platformRepairButton.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Repair " + Mathf.RoundToInt(platformRepairSlider.value) + " Damage: " + getRepairScrap(Mathf.RoundToInt(platformRepairSlider.value)) + " Scrap";
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
    private int getRepairScrap(int toBeRepaired)
    {
        int repairScrap = 0;
        int costPerLevel = 5;

        //Costs more if you've repaired before
        int startingPrice = healedSoFar % 10 + 1;


        if (FindObjectOfType<Centerpiece>().getCenterpiece().getName().CompareTo("Reinforced Centerpiece") == 0)
            costPerLevel = 10;

        //First 10 health is 5 scrap per HP, then 10, then 15, and so on
        for (int i = startingPrice; toBeRepaired > 0; i++)
        {
            if (toBeRepaired < 10)
            {
                repairScrap += toBeRepaired * costPerLevel * i;
                toBeRepaired = 0;
            }
            else
            {
                repairScrap += 10 * costPerLevel * i;
                toBeRepaired -= 10;
            }
        }

        return repairScrap;
    }
    public void repair()
    {
        int cost = getRepairScrap((int)repairSlider.value);
        GameManager manager = FindObjectOfType<GameManager>();
        if (!manager.cost(cost)) return;

        healedSoFar += (int) repairSlider.value;

        if (currentBarrier != null)
        {
            currentBarrier.SendMessage("repair", repairSlider.value);
            updateMenu(currentBarrier);
        }
        else if (centerpiece != null)
        {
            centerpiece.SendMessage("repair", repairSlider.value);
            updateMenu(centerpiece);
        }
        manager.spendScrap(cost);
    }
    public void platformRepair()
    {
        int cost = getRepairScrap((int) platformRepairSlider.value);
        GameManager manager = FindObjectOfType<GameManager>();
        if (!manager.cost(cost)) return;

        healedSoFar += (int) platformRepairSlider.value;

        currentBarrier.SendMessage("repair", platformRepairSlider.value);

        manager.spendScrap(cost);

        openPlatformRepair(currentBarrier);
    }
    public void changePriority(bool left)
    {
        if (currentTower == null) return;

        currentTower.SendMessage("priorityUpdate", left);

        TextMeshProUGUI text = priorityChanger.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();

        if (currentTower.getPriority() == TowerAI.Priority.Closest) text.text = "Closest";
        if (currentTower.getPriority() == TowerAI.Priority.Furthest) text.text = "Furthest";
        if (currentTower.getPriority() == TowerAI.Priority.Strongest) text.text = "Strongest";
        if (currentTower.getPriority() == TowerAI.Priority.Weakest) text.text = "Weakest";
        if (currentTower.getPriority() == TowerAI.Priority.Fastest) text.text = "Fastest";
        if (currentTower.getPriority() == TowerAI.Priority.OnlyWater) text.text = "Water Only";
        if (currentTower.getPriority() == TowerAI.Priority.OnlyAir) text.text = "Air Only";
    }

    private void nonInteractable()  //Makes all buttons non-interactable
    {
        upgrade1.GetComponent<Button>().interactable = false;
        upgrade2.GetComponent<Button>().interactable = false;
        destroyButton.GetComponent<Button>().interactable = false;
        priorityChanger.transform.GetChild(1).GetComponent<Button>().interactable = false;
        priorityChanger.transform.GetChild(2).GetComponent<Button>().interactable = false;
    }
    public void destroyClicked()
    {
        OnDestroy?.Invoke();

        if (currentTower != null)
            currentTower.destroyTower();
        else if (currentBarrier != null)
            currentBarrier.destroyBarrier();
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
