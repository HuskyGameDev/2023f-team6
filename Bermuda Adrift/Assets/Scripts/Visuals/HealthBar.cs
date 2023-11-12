using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider slider;
    private AI enemy;

    private void OnEnable()
    {
        enemy.OnEnemyHurt += updateHealthBar;
        enemy.SetupHealthBar += setupHealthBar;
    }

    private void OnDisable()
    {
        enemy.OnEnemyHurt -= updateHealthBar;
        enemy.SetupHealthBar -= setupHealthBar;
    }

    void Awake()
    {
        slider = gameObject.transform.GetChild(0).gameObject.GetComponent<Slider>();
        enemy = gameObject.transform.parent.GetComponent<AI>();
    }

    private void updateHealthBar(int damage)
    {
        slider.value -= damage;
    }

    private void setupHealthBar(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }
}
