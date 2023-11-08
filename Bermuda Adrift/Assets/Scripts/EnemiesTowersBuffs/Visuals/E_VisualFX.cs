using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class E_VisualFX : MonoBehaviour
{
    [SerializeField] GameObject dmgPopupTxt;
    private TextMeshPro textMesh;
    private AI enemy;

    private void OnEnable()
    {
        enemy.OnEnemyHurt += DamagePopupSetup;
    }

    private void OnDisable()
    {
        enemy.OnEnemyHurt -= DamagePopupSetup;
    }

    private void Awake()
    {
        textMesh = dmgPopupTxt.GetComponent<TextMeshPro>();
        enemy = gameObject.GetComponent<AI>();
    }

    public void DamagePopupSetup(int damageAmount)
    {
        textMesh.SetText("-" + damageAmount.ToString());
        Instantiate(dmgPopupTxt, new Vector3(Random.Range(transform.position.x + 1f, transform.position.x - 1f), Random.Range(transform.position.y + 0.1f, transform.position.y - 0.1f)), Quaternion.identity);
    }
}
