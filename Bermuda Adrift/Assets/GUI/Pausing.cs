using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pausing : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject main;
    [SerializeField] GameObject settings;
    [SerializeField] GameObject sound;
    [SerializeField] GameObject controls;
    [SerializeField] GameObject video;
    [SerializeField] GameObject credits;

    public void OnPauseInputAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // Open & Close pause menu
            pauseMenu.SetActive(!pauseMenu.activeSelf);

            //Stop & Start time
            if (Time.timeScale == 0)
            {
                sound.SetActive(true);
                controls.SetActive(false);
                video.SetActive(false);
                credits.SetActive(false);
                settings.SetActive(false);
                main.SetActive(true);

                Time.timeScale = 1;
            }
            else
            {
                Time.timeScale = 0;
            }
        }
    }

    public void OnPause()
    {
        // Open & Close pause menu
        pauseMenu.SetActive(!pauseMenu.activeSelf);

        //Stop & Start time
        if (Time.timeScale == 0)
        {
            sound.SetActive(true);
            controls.SetActive(false);
            video.SetActive(false);
            credits.SetActive(false);
            settings.SetActive(false);
            main.SetActive(true);

            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
    }
}
