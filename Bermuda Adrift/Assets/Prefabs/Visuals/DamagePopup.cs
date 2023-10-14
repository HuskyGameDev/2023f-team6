using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    private float MAX_DISAPPEAR_TIMER;
    [SerializeField] float moveYSpeed;
    [SerializeField] float disappearTimer;
    [SerializeField] float disappearSpeed;
    [SerializeField] float scaleUpSpeed;
    [SerializeField] float scaleDownSpeed;
    private TextMeshPro textMesh;
    private Color textColor;

    private void Awake()
    {
        textMesh = gameObject.GetComponent<TextMeshPro>();
        textColor = textMesh.color;
        MAX_DISAPPEAR_TIMER = disappearTimer * 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime;

        disappearTimer -= Time.deltaTime;

        if (disappearTimer > MAX_DISAPPEAR_TIMER)
        {
            gameObject.transform.localScale += Vector3.one * scaleUpSpeed * Time.deltaTime;
        }
        else
        {
            gameObject.transform.localScale -= Vector3.one * scaleDownSpeed * Time.deltaTime;
        }

        if (disappearTimer < 0)
        {
            // Start diappearing
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;

            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
