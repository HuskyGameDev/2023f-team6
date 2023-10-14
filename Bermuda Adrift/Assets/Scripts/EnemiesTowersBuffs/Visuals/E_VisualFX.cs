using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class E_VisualFX : MonoBehaviour
{
    [SerializeField] GameObject dmgPopupTxt;
    private TextMeshPro textMesh;

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    private void Awake()
    {
        textMesh = dmgPopupTxt.GetComponent<TextMeshPro>();
    }

    public void DamagePopupSetup(int damageAmount)
    {
        Instantiate(dmgPopupTxt, new Vector3(Random.Range(transform.position.x + 1f, transform.position.x - 1f), Random.Range(transform.position.y + 0.1f, transform.position.y - 0.1f)), Quaternion.identity);
        textMesh.SetText("-" + damageAmount.ToString());
    }
}
