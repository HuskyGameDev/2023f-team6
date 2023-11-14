using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonDescription : MonoBehaviour
{
    public static event Action onMouseExitButton;
    public static event Action<string, string> onDescriptionUpdate;

    [SerializeField] private Tower tower;
    [SerializeField] private Ability ability;
    private int upgradeLevel;

    public void mouseEnter()
    {
        string title = "";
        string description = "";

        if (tower != null)
        {
            if (upgradeLevel == 0)
            {
                title = tower.getName();
                description = tower.getDescription();
            }
            else if (upgradeLevel == 1)
            {
                title = tower.U1getName();
                description = tower.U1getDescription();
            }
            else if (upgradeLevel == 2)
            {
                title = tower.UA1getName();
                description = tower.UA1getDescription();
            }
            else if (upgradeLevel == 3)
            {
                title = tower.UB1getName();
                description = tower.UB1getDescription();
            }
            else if (upgradeLevel == 4)
            {
                title = tower.UA2getName();
                description = tower.UA2getDescription();
            }
            else if (upgradeLevel == 5)
            {
                title = tower.UB2getName();
                description = tower.UB2getDescription();
            }
        } else if (ability != null)
        {
            title = ability.getName();
            description = ability.getDescription();
        }
        onDescriptionUpdate?.Invoke(title, description);
    }
    public void mouseExit()
    {
        onMouseExitButton?.Invoke();
    }
    private void setUpgradeLevel(int level) { upgradeLevel = level; }
    private void incrementUpgradeLevel() { upgradeLevel++; }
    private void setTower(Tower tower) { ability = null; this.tower = tower; }
    private void setAbility(Ability ability) { tower = null; this.ability = ability; }
}
