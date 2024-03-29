using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    Animator animator;
    int levelIndex;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void FadeToLevel(int index)
    {
        levelIndex = index;
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(levelIndex);
    }
}
