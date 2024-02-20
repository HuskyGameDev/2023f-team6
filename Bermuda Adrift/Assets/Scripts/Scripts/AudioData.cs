using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioData
{
    public float MasterVolume;
    public float SFXVolume;
    public float MusicVolume;

    public AudioData()
    {
        MasterVolume = 100f;
        SFXVolume = 100f;
        MusicVolume = 100f;
    }
}
