using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindingsMenu : MonoBehaviour, IDataPersistence
{
    [SerializeField] private InputActionReference primaryAction = null;
    [SerializeField] private InputActionReference secondaryAction = null;
    [SerializeField] private InputActionReference utilityAction = null;
    [SerializeField] private InputActionReference specialAction = null;

    [SerializeField] private SettingsTracker settingsTracker;

    private InputActionRebindingExtensions.RebindingOperation rebindOperation;

    private void OnEnable()
    {
        for (int i = 0; i < 4; i++)
        {
            InputActionReference input;
            if (i == 0) input = primaryAction;
            else if (i == 1) input = secondaryAction;
            else if (i == 2) input = utilityAction;
            else if (i == 3) input = specialAction;
            else input = primaryAction;

            int bindingIndex = input.action.GetBindingIndexForControl(input.action.controls[0]);

            transform.GetChild(i).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = InputControlPath.ToHumanReadableString(input.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        }
    }

    public void StartRebinding(int i)
    {
        InputActionReference input;
        if (i == 0) input = primaryAction;
        else if (i == 1) input = secondaryAction;
        else if (i == 2) input = utilityAction;
        else if (i == 3) input = specialAction;
        else input = primaryAction;

        transform.GetChild(4).position = transform.GetChild(i).GetChild(2).position;

        rebindOperation = input.action.PerformInteractiveRebinding()
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => RebindComplete(i)).Start();
    }
    private void RebindComplete(int i)
    {
        rebindOperation.Dispose();

        transform.GetChild(4).gameObject.SetActive(false);
        transform.GetChild(i).GetChild(2).gameObject.SetActive(true);

        InputActionReference input;
        if (i == 0)
        {
            input = primaryAction;
            settingsTracker.setBinding(0, primaryAction.action.SaveBindingOverridesAsJson());
        }
        else if (i == 1)
        {
            input = secondaryAction;
            settingsTracker.setBinding(1, secondaryAction.action.SaveBindingOverridesAsJson());
        }
        else if (i == 2)
        {
            input = utilityAction;
            settingsTracker.setBinding(2, utilityAction.action.SaveBindingOverridesAsJson());
        }
        else if (i == 3)
        {
            input = specialAction;
            settingsTracker.setBinding(3, specialAction.action.SaveBindingOverridesAsJson());
        }
        else input = primaryAction;

        int bindingIndex = input.action.GetBindingIndexForControl(input.action.controls[0]);

        transform.GetChild(i).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = InputControlPath.ToHumanReadableString(input.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

    public void LoadData(S_O_Saving saver)
    {
        primaryAction.action.LoadBindingOverridesFromJson(saver.primaryRebinds);
        secondaryAction.action.LoadBindingOverridesFromJson(saver.secondaryRebinds);
        utilityAction.action.LoadBindingOverridesFromJson(saver.utilityRebinds);
        specialAction.action.LoadBindingOverridesFromJson(saver.specialRebinds);
    }

    public void SaveData(S_O_Saving saver)
    {
        saver.primaryRebinds = primaryAction.action.SaveBindingOverridesAsJson();
        saver.secondaryRebinds = secondaryAction.action.SaveBindingOverridesAsJson();
        saver.utilityRebinds = utilityAction.action.SaveBindingOverridesAsJson();
        saver.specialRebinds = specialAction.action.SaveBindingOverridesAsJson();
    }
}
