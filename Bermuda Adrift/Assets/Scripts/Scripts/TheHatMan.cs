using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TheHatMan : MonoBehaviour, IPointerDownHandler
{
    private void Start()
    {
        transform.position = FindObjectOfType<BuildManager>().randomApprovedTowerPosition();
        AddPhysics2DRaycaster();
        StartCoroutine(delayedDisappear());
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        AI[] enemies = FindObjectsOfType<AI>();

        foreach (AI enemy in enemies)
            enemy.SendMessage("Forget");

        Destroy(gameObject);
    }

    private void AddPhysics2DRaycaster()
    {
        Physics2DRaycaster physicsRaycaster = FindObjectOfType<Physics2DRaycaster>();
        if (physicsRaycaster == null)
        {
            Camera.main.gameObject.AddComponent<Physics2DRaycaster>();
        }
    }

    private IEnumerator delayedDisappear()
    {
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }
}
