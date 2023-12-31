using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTracker : MonoBehaviour
{
    [SerializeField] private Player defaultCharacter;
    private Player character;
    // Start is called before the first frame update

    private void OnEnable()
    {
        CharacterModelHandler.onCharacterChange += setCharacter;
    }
    private void OnDisable()
    {
        CharacterModelHandler.onCharacterChange -= setCharacter;
    }
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        setCharacter(defaultCharacter);
    }
    public void setCharacter(Player character) { this.character = character; }
    private void OnLevelWasLoaded(int level)
    {
        if (level == 1)
            StartCoroutine(setCharacterInScene());
    }
    IEnumerator setCharacterInScene()
    {
        yield return new WaitForEndOfFrame();
        GameObject.Find("Player").SendMessage("setCharacter", character);
    }
    public Player getCharacter() { return character; }
}
