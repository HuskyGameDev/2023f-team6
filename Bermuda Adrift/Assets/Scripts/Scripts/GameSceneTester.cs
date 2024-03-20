using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneTester : MonoBehaviour    //This script is so you don't have to go back to the main menu
{
    public int raftSelected;
    public Player character;
    public CenterpieceScriptable centerpiece;
    public BarrierScriptable barrier1;
    public BarrierScriptable barrier2;

    // Start is called before the first frame update
    void Start()
    {
        if (FindObjectOfType<SettingsTracker>() != null)
            Destroy(gameObject);
        else
            StartCoroutine(setSettingsInScene());
    }

    IEnumerator setSettingsInScene()
    {
        if (raftSelected >= 4)
            raftSelected = 3;
        if (raftSelected < 0)
            raftSelected = 0;

        GameObject masterLayout = GameObject.Find("Raft Layouts");
        for (int i = 0; i < masterLayout.transform.childCount; i++)
            masterLayout.transform.GetChild(i).gameObject.SetActive(false);

        masterLayout.transform.GetChild(raftSelected).gameObject.SetActive(true);

        yield return new WaitForEndOfFrame();

        AstarPath.active.Scan();

        GameObject.Find("Player").SendMessage("setCharacter", character);
        FindObjectOfType<BuildManager>().setBarrier1(barrier1);
        FindObjectOfType<BuildManager>().setBarrier2(barrier2);
        FindObjectOfType<BuildManager>().reloadBuyables();
        FindObjectOfType<Centerpiece>().setCenterpiece(centerpiece);
    }
}
