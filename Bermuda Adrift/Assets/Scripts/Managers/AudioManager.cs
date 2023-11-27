using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider SFXSlider;

    [SerializeField] AudioClip backgroundMusic; //Will need to change when there's more songs
    AudioSource source;

    private void Awake()
    {
        source = gameObject.GetComponent<AudioSource>();

        source.clip = backgroundMusic;
        quiet();
    }

    public void PlaySound()
    {
        source.Play();
        source.loop = true;
    }

    public void StopSound()
    {
        source.Stop();
    }

    public void quiet() { source.volume = 0.25f; }
    public void fullVolume() { source.volume = 1f; }

    public void ChangeSFXVolume()
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(musicSlider.value) * 20);
    }

    public void ChangeMusicVolume()
    {
        mixer.SetFloat("MusicVolume", Mathf.Log10(SFXSlider.value) * 20);
    }
}
