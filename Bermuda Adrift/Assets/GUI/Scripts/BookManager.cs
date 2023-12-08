using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookManager : MonoBehaviour
{
    Animator animator;

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
}
