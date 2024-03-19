using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempKeys : MonoBehaviour
{
    [SerializeField] private int testRound;
    void Update()
    {
    #if UNITY_EDITOR

        //  X - Add all
        if (Input.GetKeyDown(KeyCode.X))
            FindObjectOfType<LevelGUIManager>().addAll();

        //  Numbers 1-0 - Add a specific blueprint
        for (int i = 1; i < 10; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
                FindObjectOfType<LevelGUIManager>().addIndex(i - 1);
        }

        //  P - Kill all enemies
        if (Input.GetKeyDown(KeyCode.P))
        {
            foreach (AI ai in FindObjectsOfType<AI>())
                ai.SendMessage("death");
        }

        //  F - Start a Round
        if (Input.GetKeyDown(KeyCode.F))
            FindObjectOfType<GameManager>().startRound();
        
        //  N - Add 100 Scrap
        if (Input.GetKeyDown(KeyCode.N))
            FindObjectOfType<GameManager>().addScrap(100);
        
        //  Z - Add 1000 Scrap
        if (Input.GetKeyDown(KeyCode.Z))
            FindObjectOfType<GameManager>().addScrap(1000);
        
        //  B - Add 100 XP
        if (Input.GetKeyDown(KeyCode.B))
            FindObjectOfType<GameManager>().addXP(100);

        //  M - Load Save
        if (Input.GetKeyDown(KeyCode.M))
            FindObjectOfType<SaveManager>().LoadPlayer();

        //  = - Delete Current Save
        if (Input.GetKeyDown(KeyCode.Equals))
            FindObjectOfType<SaveManager>().DeletePlayer();

        //  V - Unlock the Old Man's Achievement
        if (Input.GetKeyDown(KeyCode.V))
            FindObjectOfType<AchievementHandler>().SendMessage("unlockAchievement", "Ach_Man_In_The_Mirror");

        //  O - Skip to the given round
        if (Input.GetKeyDown(KeyCode.O)) 
            FindObjectOfType<GameManager>().skipToRound(testRound);

        //  I - Spawn an island
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (FindObjectOfType<IslandInteractions>() != null)
                FindObjectOfType<IslandInteractions>().deleteIsland();

            FindObjectOfType<IslandManager>().summonIsland();
        }

    #endif
    }
}
