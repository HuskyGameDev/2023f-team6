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
        Debug.Log("Clicked");
        if (FindObjectOfType<GameManager>().getGameState() == GameManager.GameState.Idle) return;

        AI[] enemies = FindObjectsOfType<AI>();

        foreach (AI enemy in enemies)
            enemy.SendMessage("Forget");

        StartCoroutine(fadeAway());
    }

    private void AddPhysics2DRaycaster()
    {
        Physics2DRaycaster physicsRaycaster = FindObjectOfType<Physics2DRaycaster>();
        if (physicsRaycaster == null)
        {
            Camera.main.gameObject.AddComponent<Physics2DRaycaster>();
        }
    }
    private IEnumerator fadeAway()
    {
        int a = 10;
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        for(int i = 255; i > 0; i -= a)
        {
            sr.color -= Color.black * (a / 255);
            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
    }

    private IEnumerator delayedDisappear()
    {
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }
}
