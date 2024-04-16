using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SC_CircularLoading : MonoBehaviour
{
    [Range(0, 1)]
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
        gameObject.GetComponent<Image>().fillAmount = 1;
        transform.parent.GetChild(2).gameObject.GetComponent<Image>().fillAmount = 0;
    }

    void updateHealthbar()
    {
        Centerpiece centerpiece = FindObjectOfType<Centerpiece>();

        progression = (float) centerpiece.getCurrentHealth() / centerpiece.getMaxHealth();
        if (centerpiece.getHealth() > centerpiece.getCurrentHealth())
        {
            transform.parent.GetChild(2).gameObject.GetComponent<Image>().fillAmount = (float)(centerpiece.getHealth() - centerpiece.getCurrentHealth()) / centerpiece.getMaxBarrier();
            transform.parent.GetChild(2).gameObject.GetComponent<Image>().color = Color.cyan;
        }
        else
        {
            transform.parent.GetChild(2).gameObject.GetComponent<Image>().fillAmount = 0;
            gameObject.GetComponent<Image>().fillAmount = progression;
            gameObject.GetComponent<Image>().color = Color.Lerp(new Color(0.745098039f, 0.262745098f, 0.301960784f), new Color(0.290196078f, 0.745098039f, 0.262745098f), progression);
        }

        transform.parent.GetChild(3).GetComponent<TextMeshProUGUI>().text = centerpiece.getHealth().ToString();
    }
}