using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyResolution
{
    public int width;
    public int height;
    public RefreshRate hz;

    override public string ToString()
    {
        return width.ToString() + "x" + height.ToString() + " @ " + hz.ToString();
    }
}
