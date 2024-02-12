using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsTracker : MonoBehaviour
{
    [SerializeField] private Player defaultCharacter;
    private Player character;
    char diffData;

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
        setCharacter(defaultCharacter);

        diffData = (char)0;
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

    #region Difficulty Settings
    /* Bits meaning:
     * Bit 0: More enemy health
     * Bit 1: More enemy damage
     * Bit 2: More Enemy speed
     * Bit 3: Less scrap from enemies
     * Bit 4: Less starting health
     * Bit 5: More expensive to repair centerpiece
     * Bit 6: Enemies can appear in earlier rounds than they should
     */
    public void setBit0(bool on)
    {
        if (on)
            diffData = (char)(diffData | 1);
        else
            diffData = (char)(diffData & ~1);
    }
    public void setBit1(bool on)
    {
        if (on)
            diffData = (char)(diffData | (1 << 1));
        else
            diffData = (char)(diffData & ~(1 << 1));
    }
    public void setBit2(bool on)
    {
        if (on)
            diffData = (char)(diffData | (1 << 2));
        else
            diffData = (char)(diffData & ~(1 << 2));
    }
    public void setBit3(bool on)
    {
        if (on)
            diffData = (char)(diffData | (1 << 3));
        else
            diffData = (char)(diffData & ~(1 << 3));
    }
    public void setBit4(bool on)
    {
        if (on)
            diffData = (char)(diffData | (1 << 4));
        else
            diffData = (char)(diffData & ~(1 << 4));
    }
    public void setBit5(bool on)
    {
        if (on)
            diffData = (char)(diffData | (1 << 5));
        else
            diffData = (char)(diffData & ~(1 << 5));
    }
    public void setBit6(bool on)
    {
        if (on)
            diffData = (char)(diffData | (1 << 6));
        else
            diffData = (char)(diffData & ~(1 << 6));
    }
    public bool getBit0() { return (diffData & (1 << 0)) != 0; }
    public bool getBit1() { return (diffData & (1 << 1)) != 0; }
    public bool getBit2() { return (diffData & (1 << 2)) != 0; }
    public bool getBit3() { return (diffData & (1 << 3)) != 0; }
    public bool getBit4() { return (diffData & (1 << 4)) != 0; }
    public bool getBit5() { return (diffData & (1 << 5)) != 0; }
    public bool getBit6() { return (diffData & (1 << 6)) != 0; }
    public void setDifficulty(char settings) { diffData = settings; }
    #endregion

    bool noScreenShake;
    public void setScreenShake(bool screenShake) { noScreenShake = screenShake; }
    public bool getScreenShake() { return noScreenShake; }
}
