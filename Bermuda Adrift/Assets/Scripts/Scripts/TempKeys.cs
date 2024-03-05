using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempKeys : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        //  X - Add all
        if (Input.GetKeyDown("x"))
            FindObjectOfType<LevelGUIManager>().addAll();

        //  Numbers 1-0 - Add a specific blueprint
        for (int i = 1; i < 10; i++)
        {
            if (Input.GetKeyDown(i.ToString())) FindObjectOfType<LevelGUIManager>().addIndex(i - 1);
        }

        //  P - Kill all enemies
        if (Input.GetKeyDown("p"))
        {
            foreach (AI ai in FindObjectsOfType<AI>())
                ai.SendMessage("death");
        }

        //  F - Start a Round
        if (Input.GetKeyDown("f"))
            FindObjectOfType<GameManager>().startRound();
        
        //  N - Add 100 Scrap
        if (Input.GetKeyDown("n"))
            FindObjectOfType<GameManager>().addScrap(100);
        
        //  Z - Add 1000 Scrap
        if (Input.GetKeyDown("z"))
            FindObjectOfType<GameManager>().addScrap(1000);
        
        //  B - Add 100 XP
        if (Input.GetKeyDown("b"))
            FindObjectOfType<GameManager>().addXP(100);

        //  M - Load Save
        if (Input.GetKeyDown("m"))
            FindObjectOfType<SaveManager>().LoadPlayer();

        //  = - Delete Current Save
        if (Input.GetKeyDown("="))
            FindObjectOfType<SaveManager>().DeletePlayer();

        //  V - Unlock the Old Man's Achievement
        if (Input.GetKeyDown("v"))
            FindObjectOfType<AchievementHandler>().SendMessage("unlockAchievement", "Ach_Man_In_The_Mirror");
    }
}
