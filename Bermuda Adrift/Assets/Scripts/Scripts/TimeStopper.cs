using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeStopper : MonoBehaviour
{
    public void stopTime() { Time.timeScale = 0; }
    public void startTime() { Time.timeScale = 1; }
}
