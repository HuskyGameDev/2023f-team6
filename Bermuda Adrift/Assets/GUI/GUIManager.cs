using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GUIManager : MonoBehaviour
{
    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void SetActiveTrue(GameObject screen)
    {
        screen.SetActive(true);
    }

    public void SetActiveFalse(GameObject screen)
    {
        screen.SetActive(false);
    }
}
