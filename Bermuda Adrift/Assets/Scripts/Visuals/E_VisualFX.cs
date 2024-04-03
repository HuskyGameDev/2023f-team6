using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class E_VisualFX : MonoBehaviour
{
    [SerializeField] GameObject dmgPopupTxt;
    private TextMeshPro textMesh;
    private AI enemy;
    private Centerpiece centerpiece;
    private Barriers barrier;

    private void OnEnable()
    {
        if (enemy != null)
        {
            enemy.OnEnemyHurt += DamagePopupSetup;
            enemy.OnCrit += CritDamagePopupSetup;
            enemy.OnStatusDamage += DOTDamagePopupSetup;
        }
        else if (barrier != null)
            barrier.onBarrierDamaged += CritDamagePopupSetup;
        else
            centerpiece.onCenterpieceDamage += CritDamagePopupSetup;
    }

    private void OnDisable()
    {
        if (enemy != null)
        {
            enemy.OnEnemyHurt -= DamagePopupSetup;
            enemy.OnCrit -= CritDamagePopupSetup;
            enemy.OnStatusDamage -= DOTDamagePopupSetup;
        }
        else if (barrier != null)
            barrier.onBarrierDamaged -= CritDamagePopupSetup;
        else
            centerpiece.onCenterpieceDamage -= CritDamagePopupSetup;
    }

    private void Awake()
    {
        textMesh = dmgPopupTxt.GetComponent<TextMeshPro>();
        enemy = gameObject.GetComponent<AI>();
        barrier = gameObject.GetComponent<Barriers>();
        if (enemy == null && barrier == null)
            centerpiece = gameObject.GetComponent<Centerpiece>();
    }

    public void DamagePopupSetup(int damageAmount)
    {
        if (damageAmount == 0) { return; }
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
        if (damageAmount == 0) { return; }

        textMesh.SetText("-" + damageAmount.ToString());
        
        textMesh.color = Color.red;

        Instantiate(dmgPopupTxt, new Vector3(Random.Range(transform.position.x + 1f, transform.position.x - 1f), Random.Range(transform.position.y + 0.1f, transform.position.y - 0.1f)), Quaternion.identity);
    }
    public void DOTDamagePopupSetup(int damageAmount, Color color)
    {
        if (damageAmount == 0) { return; }

        textMesh.SetText("-" + damageAmount.ToString());

        textMesh.color = color;

        Instantiate(dmgPopupTxt, new Vector3(Random.Range(transform.position.x + 1f, transform.position.x - 1f), Random.Range(transform.position.y + 0.1f, transform.position.y - 0.1f)), Quaternion.identity);
    }
}
