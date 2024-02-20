using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsTracker : MonoBehaviour, IDataPersistence
{
    private Player character;
    [SerializeField] private Player[] characterChoices;

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
    private void OnLevelWasLoaded(int level)
    {
        if (level >= 1)
            StartCoroutine(setCharacterInScene());
    }
    IEnumerator setCharacterInScene()
    {
        yield return new WaitForEndOfFrame();
        GameObject.Find("Player").SendMessage("setCharacter", character);
    }
    public Player getCharacter() { return character; }
    #endregion

    

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
}
