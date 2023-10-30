using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletMiddleMan : MonoBehaviour
{
    public void destroy()
    {
        gameObject.SendMessageUpwards("destroyBullet");
    }
}
