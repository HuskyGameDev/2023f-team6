using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretMiddleMan : MonoBehaviour
{
    public void fireBullet()
    {
        SendMessageUpwards("fire");
    }
}
