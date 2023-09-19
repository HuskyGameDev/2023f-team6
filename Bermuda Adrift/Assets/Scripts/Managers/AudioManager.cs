using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider masterSlider;

    public void PlaySound()
    {

    }

    public void ChangeMasterVolume()
    {
        mixer.SetFloat("MasterVolume", Mathf.Log10(masterSlider.value) * 20);
    }
}
