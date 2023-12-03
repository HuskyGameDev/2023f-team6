using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SC_CircularLoading : MonoBehaviour
{
    [Range(0, 1)]
    private Centerpiece centerpiece;
    private float progression;

    private void OnEnable()
    {
        Centerpiece.onCenterpieceDamaged += updateHealthbar;
    }
    private void OnDisable()
    {
        Centerpiece.onCenterpieceDamaged -= updateHealthbar;
    }
    private void Start()
    {
        centerpiece = FindObjectOfType<Centerpiece>();

        gameObject.GetComponent<Image>().fillAmount = 1;
    }

    void updateHealthbar()
    {
        progression = (float) centerpiece.getHealth() / centerpiece.getMaxHealth();
        gameObject.GetComponent<Image>().fillAmount = progression;
        transform.parent.GetChild(2).GetComponent<TextMeshProUGUI>().text = centerpiece.getHealth().ToString();

        gameObject.GetComponent<Image>().color = Color.Lerp(Color.red, Color.green, progression);
    }
}