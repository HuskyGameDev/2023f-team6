using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class SettingsTracker : MonoBehaviour, IDataPersistence
{
    private Player character;
    [SerializeField] private Player[] characterChoices;

    [SerializeField] private BarrierScriptable barrier1;
    [SerializeField] private BarrierScriptable barrier2;

    [SerializeField] private CenterpieceScriptable centerpiece;

    string primary;
    string secondary;
    string utility;
    string special;

    int raftSelected;
    bool resettingScene;
    bool screenShake;

    #region Setup
    private void OnEnable()
    {
        CharacterModelHandler.onCharacterChange += setCharacter;
        //Events to change difficulty settings
    }
    private void OnDisable()
    {
        CharacterModelHandler.onCharacterChange -= setCharacter;
    }
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        setCharacter(characterChoices[0]);
    }
    #endregion

    #region Character tracking
    public void setCharacter(Player character) { this.character = character; }
    public Player getCharacter() { return character; }
    #endregion

    IEnumerator setSettingsInScene()
    {
        yield return new WaitForEndOfFrame();
        GameObject masterLayout = GameObject.Find("Raft Layouts");
        for (int i = 0; i < masterLayout.transform.childCount; i++)
            masterLayout.transform.GetChild(i).gameObject.SetActive(false);

        masterLayout.transform.GetChild(raftSelected).gameObject.SetActive(true);

        yield return new WaitForEndOfFrame();

        AstarPath.active.Scan();

        GameObject.Find("Player").SendMessage("setCharacter", character);
        FindObjectOfType<BuildManager>().setBarrier1(barrier1);
        FindObjectOfType<BuildManager>().setBarrier2(barrier2);
        FindObjectOfType<BuildManager>().reloadBuyables();
        FindObjectOfType<Centerpiece>().setCenterpiece(centerpiece);

        Camera.main.GetComponent<CameraEffects>().setEnabled(screenShake);

        FindObjectOfType<Attack>().primaryButton.LoadBindingOverridesFromJson(primary);
        FindObjectOfType<Attack>().secondaryButton.LoadBindingOverridesFromJson(secondary);
        Debug.Log(secondary);

        FindObjectOfType<Attack>().utilityButton.LoadBindingOverridesFromJson(utility);
        FindObjectOfType<Attack>().specialButton.LoadBindingOverridesFromJson(special);
    }

    public void setRaft(int raft)
    {
        if (raft < 0) raft = 0;
        if (raft >= 4) raft = 3;
        raftSelected = raft;
    }
    public void setBarrier1(BarrierScriptable barrier)
    {
        if (barrier == barrier2)
        {
            FindObjectOfType<ChoiceOutlines>().revertOutline1();
            return;
        }
        barrier1 = barrier;
    }
    public void setBarrier2(BarrierScriptable barrier) 
    {
        if (barrier == barrier1) 
        {
            FindObjectOfType<ChoiceOutlines>().revertOutline2();
            return;
        }
        barrier2 = barrier;
    }
    public void setScreenShake(bool screenShake) { this.screenShake = screenShake; }
    public bool getScreenShake() { return screenShake; }
    public void setBinding(int attack, string bindings)
    {
        if (attack == 0) primary = bindings;
        if (attack == 1) secondary = bindings;
        if (attack == 2) utility = bindings;
        if (attack == 3) special = bindings;
    }

    public void setCenterpiece(CenterpieceScriptable centerpiece) { this.centerpiece = centerpiece; }

    public BarrierScriptable getBarrier1() { return barrier1; }
    public BarrierScriptable getBarrier2() { return barrier2; }
    public CenterpieceScriptable getCenterpiece() { return centerpiece; }
    public bool getCharacterUnlock(string name)
    {
        foreach (Player player in characterChoices)
        {
            if (player.getName().CompareTo(name) == 0)
                return player.getUnlocked();
        }
        return false;
    }

    public void LoadData(S_O_Saving saver)
    {
        foreach (Player player in characterChoices)
            player.setUnlock(saver.getCharacterSave(player.name).unlocked);

        setScreenShake(saver.screenShake);

        
    }

    public void SaveData(S_O_Saving saver)
    {
        foreach (Player player in characterChoices)
        {
            saver.getCharacterSave(player.name).unlocked = player.getUnlocked();
        }

        
    }
    private void OnLevelWasLoaded(int level)
    {
        if (level >= 1 && !resettingScene)
        {
            StartCoroutine(setSettingsInScene());
        }
    }
}
