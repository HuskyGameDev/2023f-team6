using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldMiddleMan : MonoBehaviour
{
    public void shieldDestroyed() { SendMessageUpwards("barrierDestroyed"); }
}
