using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShaderManager : MonoBehaviour
{
    [SerializeField] private GameObject rain;
    [SerializeField] private Material normalWater;
    [SerializeField] private Material bloodWater;
    [SerializeField] private GameObject water;

    private void Update()
    {
        if (Input.GetKeyDown("l"))
            startBossRound();
        if (Input.GetKeyDown(KeyCode.Backspace))
            endBossRound();
    }
    public void startBossRound()
    {
        rain.SetActive(true);
        water.GetComponent<TilemapRenderer>().material = bloodWater;
    }
    public void endBossRound()
    {
        rain.SetActive(false);
        water.GetComponent<TilemapRenderer>().material = normalWater;
    }
}
