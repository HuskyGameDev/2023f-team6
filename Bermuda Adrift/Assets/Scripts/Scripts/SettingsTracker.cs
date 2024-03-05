using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsTracker : MonoBehaviour, IDataPersistence
{
    private Player character;
    [SerializeField] private Player[] characterChoices;

    [SerializeField] private BarrierScriptable barrier1;
    [SerializeField] private BarrierScriptable barrier2;

    int raftSelected;
    bool resettingScene;

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

    bool noScreenShake;
    public void setScreenShake(bool screenShake) { noScreenShake = screenShake; }
    public bool getScreenShake() { return noScreenShake; }

    public void LoadData(S_O_Saving saver)
    {
        foreach (Player player in characterChoices)
        {
            player.setUnlock(saver.getCharacterSave(player.name).unlocked);
        }
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
