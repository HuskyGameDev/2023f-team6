using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutOutlines : MonoBehaviour
{
    [SerializeField] private GameObject outline;

    public void updateOutline()
    {
        outline.transform.position = gameObject.transform.position;
    }
}
