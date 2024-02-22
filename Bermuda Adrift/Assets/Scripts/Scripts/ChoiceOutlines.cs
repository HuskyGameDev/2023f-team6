using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceOutlines : MonoBehaviour
{
    [SerializeField] GameObject Outline1;
    [SerializeField] GameObject Outline2;
    Vector3 prevOutline1;
    Vector3 prevOutline2;

    private void Start()
    {
        if (prevOutline1 == Vector3.zero)
            prevOutline1 = Outline1.transform.position;

        if (prevOutline2 == Vector3.zero)
            prevOutline2 = Outline2.transform.position;

        revertOutline1();
        revertOutline2();
    }

    public void updateOutline1(Transform newPosition)
    {
        prevOutline1 = Outline1.transform.position;
        Outline1.transform.position = newPosition.position;
    }
    public void updateOutline2(Transform newPosition)
    {
        prevOutline2 = Outline2.transform.position;
        Outline2.transform.position = newPosition.position;
    }
    public void revertOutline1() { Outline1.transform.position = prevOutline1; }
    public void revertOutline2() { Outline2.transform.position = prevOutline2; }
}
