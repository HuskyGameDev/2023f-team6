using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookManager : MonoBehaviour
{
    Animator animator;
    int numPagesUnlocked = 6;

    private void OnEnable()
    {
        Unlocks.enemyUnlocked += addEntry;
    }
    private void OnDisable()
    {
        Unlocks.enemyUnlocked -= addEntry;
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void FlipPageLeft()
    {
        animator.SetTrigger("FlipLeft");
    }

    public void FlipPageRight()
    {
        animator.SetTrigger("FlipRight");
    }

    public void showPage(GameObject page)
    {
        StartCoroutine(waitThenShowPage(page));
    }

    public void hidePage(GameObject page)
    {
        page.SetActive(false);
    }

    IEnumerator waitThenShowPage(GameObject page)
    {
        yield return new WaitForSeconds(0.25f);
        page.SetActive(true);
    }

    public void createLockedEntries()
    {

    }

    public void addEntry(Enemy e)
    {
        GameObject newEntry = new GameObject();
    }
}
