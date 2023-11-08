using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SC_CircularLoading : MonoBehaviour
{
    private Image Healthbar;
    private TextMeshProUGUI health;
    [Range(0, 1)]
    private int maxHealth;
    private Centerpiece centerpiece;
    private float progression;

    private void Start()
    {
        Healthbar = gameObject.GetComponent<Image>();
        health = transform.parent.GetChild(2).GetComponent<TextMeshProUGUI>();
        centerpiece = GameObject.Find("TempCenter").GetComponent<Centerpiece>();
        maxHealth = centerpiece.getHealth();
    }

    // Update is called once per frame
    void Update()       //Will probably be more efficient if this uses Events. I'll set that up later
    {
        progression = (float) centerpiece.getHealth() / maxHealth;
        Healthbar.fillAmount = progression;
        health.text = centerpiece.getHealth().ToString();

        Healthbar.color = Color.Lerp(Color.red, Color.green, progression);
    }
}