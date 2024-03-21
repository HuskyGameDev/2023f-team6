using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SaveDataText : MonoBehaviour
{
    public TMP_Text rounds;
    public TMP_Text scrap;
    public TMP_Text health;
    public TMP_Text level;

    PlayerData data;
    // Start is called before the first frame update
    void Start()
    {
        data = SaveSystem.loadPlayer();
        if(data == null)
        {
            rounds.text = "Round: --";
            scrap.text = "Scrap: --";
            health.text = "Health: --";
            level.text = "Level: --";
        }
        else
        {
            rounds.text = "Round: " + data.getRound();
            scrap.text = "Scrap: " + data.getScrap();
            health.text = "Health: " + data.getHealth();
            level.text = "Level: " + data.getLevel();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
