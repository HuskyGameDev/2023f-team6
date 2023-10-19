using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullscreenToggler : MonoBehaviour
{
    Toggle toggler;
    void Start()
    {
        toggler = gameObject.GetComponent<Toggle>();
    }

    public void Toggle()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}
