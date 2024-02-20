using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VideoData
{
    public int aspectRatio;
    public int resolution;
    public bool fullscreen;

    public VideoData()
    {
        aspectRatio = 1;
        resolution = 0;
        fullscreen = true;
    }
}
