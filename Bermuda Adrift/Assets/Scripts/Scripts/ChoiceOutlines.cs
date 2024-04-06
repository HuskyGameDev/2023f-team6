using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceOutlines : MonoBehaviour
{
    [SerializeField] GameObject Outline1;
    [SerializeField] GameObject Outline2;
    [SerializeField] GameObject Outline3;
    Vector3 prevOutline1;
    Vector3 prevOutline2;
    Vector3 prevOutline3;

    private void Start()
    {
        if (prevOutline1 == Vector3.zero)
            prevOutline1 = Outline1.transform.position;

        if (prevOutline2 == Vector3.zero)
            prevOutline2 = Outline2.transform.position;

        if (prevOutline3 == Vector3.zero)
            prevOutline3 = Outline3.transform.position;

        revertOutline1();
        revertOutline2();
        revertOutline3();
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
    public void updateOutline3(Transform newPosition)
    {
        prevOutline3 = Outline3.transform.position;
        Outline3.transform.position = newPosition.position;
    }
    public void revertOutline1() { Outline1.transform.position = prevOutline1; }
    public void revertOutline2() { Outline2.transform.position = prevOutline2; }
    public void revertOutline3() { Outline3.transform.position = prevOutline3; }
}
