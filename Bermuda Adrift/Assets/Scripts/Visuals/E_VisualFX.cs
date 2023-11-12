using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class E_VisualFX : MonoBehaviour
{
    [SerializeField] GameObject dmgPopupTxt;
    private TextMeshPro textMesh;
    private AI enemy;
    private bool crit;

    private void OnEnable()
    {
        enemy.OnEnemyHurt += DamagePopupSetup;
        Hitscan.onCrit += critHit;
        Weapon.onCrit += critHit;
    }

    private void OnDisable()
    {
        enemy.OnEnemyHurt -= DamagePopupSetup;
        Hitscan.onCrit -= critHit;
        Weapon.onCrit -= critHit;
    }

    private void Awake()
    {
        textMesh = dmgPopupTxt.GetComponent<TextMeshPro>();
        enemy = gameObject.GetComponent<AI>();
    }

    public void critHit() { crit = true; }

    public void DamagePopupSetup(int damageAmount)
    {
        if (damageAmount >= 0)
        {
            textMesh.SetText("-" + damageAmount.ToString());
            if (crit)
            {
                crit = false;
                textMesh.color = Color.red;
                Debug.Log("Crit for " + damageAmount + " damage");
            }
            else
                textMesh.color = Color.gray;

            Instantiate(dmgPopupTxt, new Vector3(Random.Range(transform.position.x + 1f, transform.position.x - 1f), Random.Range(transform.position.y + 0.1f, transform.position.y - 0.1f)), Quaternion.identity);
        } else
        {
            textMesh.SetText("+" + (-damageAmount).ToString());
            textMesh.color = Color.green;
            Instantiate(dmgPopupTxt, new Vector3(Random.Range(transform.position.x + 1f, transform.position.x - 1f), Random.Range(transform.position.y + 0.1f, transform.position.y - 0.1f)), Quaternion.identity);
        }
    }
}
