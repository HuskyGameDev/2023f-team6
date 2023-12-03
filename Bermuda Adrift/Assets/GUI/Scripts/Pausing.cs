using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class Pausing : MonoBehaviour
{
    public static event Action onPause;
    public static event Action onUnpause;

    GameObject pauseMenu;
    GameObject main;
    GameObject settings;
    GameObject sound;
    GameObject controls;
    GameObject video;
    GameObject credits;


    private void Start()
    {
        pauseMenu = transform.GetChild(0).gameObject;

        Transform canvas = transform.GetChild(0).GetChild(0);

        main =     canvas.GetChild(1).gameObject;
        settings = canvas.GetChild(2).gameObject;
        sound =    canvas.GetChild(3).gameObject;
        controls = canvas.GetChild(4).gameObject;
        video =    canvas.GetChild(5).gameObject;
        credits =  canvas.GetChild(6).gameObject;

        pauseMenu.SetActive(false);
    }
    public void OnPauseInputAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // Open & Close pause menu
            pauseMenu.SetActive(!pauseMenu.activeSelf);

            //Stop & Start time
            if (Time.timeScale == 0)
            {
                //onUnpause?.Invoke();
                sound.SetActive(false);
                controls.SetActive(false);
                video.SetActive(false);
                credits.SetActive(false);
                settings.SetActive(false);
                main.SetActive(true);

                Time.timeScale = 1;
            }
            else
            {
                //onPause?.Invoke();
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
            //onUnpause?.Invoke();
            sound.SetActive(false);
            controls.SetActive(false);
            video.SetActive(false);
            credits.SetActive(false);
            settings.SetActive(false);
            main.SetActive(true);

            Time.timeScale = 1;
        }
        else
        {
            //onPause?.Invoke();
            Time.timeScale = 0;
        }
    }
}
