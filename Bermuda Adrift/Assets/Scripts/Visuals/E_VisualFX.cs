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
        enemy.OnCrit += CritDamagePopupSetup;
        enemy.OnStatusDamage += DOTDamagePopupSetup;
    }

    private void OnDisable()
    {
        enemy.OnEnemyHurt -= DamagePopupSetup;
        enemy.OnCrit -= CritDamagePopupSetup;
        enemy.OnStatusDamage -= DOTDamagePopupSetup;
    }

    private void Awake()
    {
        textMesh = dmgPopupTxt.GetComponent<TextMeshPro>();
        enemy = gameObject.GetComponent<AI>();
    }

    public void DamagePopupSetup(int damageAmount)
    {
        if (damageAmount >= 0)
        {
            textMesh.SetText("-" + damageAmount.ToString());
            textMesh.color = Color.gray;

            Instantiate(dmgPopupTxt, new Vector3(Random.Range(transform.position.x + 1f, transform.position.x - 1f), Random.Range(transform.position.y + 0.1f, transform.position.y - 0.1f)), Quaternion.identity);
        } else
        {
            textMesh.SetText("+" + (-damageAmount).ToString());
            textMesh.color = Color.green;
            Instantiate(dmgPopupTxt, new Vector3(Random.Range(transform.position.x + 1f, transform.position.x - 1f), Random.Range(transform.position.y + 0.1f, transform.position.y - 0.1f)), Quaternion.identity);
        }
    }
    public void CritDamagePopupSetup(int damageAmount)
    {
        textMesh.SetText("-" + damageAmount.ToString());
        
        textMesh.color = Color.red;

        Instantiate(dmgPopupTxt, new Vector3(Random.Range(transform.position.x + 1f, transform.position.x - 1f), Random.Range(transform.position.y + 0.1f, transform.position.y - 0.1f)), Quaternion.identity);
    }
    public void DOTDamagePopupSetup(int damageAmount, Color color)
    {
        textMesh.SetText("-" + damageAmount.ToString());

        textMesh.color = color;

        Instantiate(dmgPopupTxt, new Vector3(Random.Range(transform.position.x + 1f, transform.position.x - 1f), Random.Range(transform.position.y + 0.1f, transform.position.y - 0.1f)), Quaternion.identity);
    }
}
