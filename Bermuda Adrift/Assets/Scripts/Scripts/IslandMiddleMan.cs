using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandMiddleMan : MonoBehaviour
{
    public void DeathIsland()
    {
        FindObjectOfType<Centerpiece>().SendMessage("bulletTakeDamage", 10);
    }
}
