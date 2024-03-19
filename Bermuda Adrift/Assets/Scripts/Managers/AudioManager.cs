using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System;
using TMPro;

public class AudioManager : MonoBehaviour, IDataPersistence
{
    private static AudioManager _instance;

    public static AudioManager Instance
    {
        get { return _instance; }
    }


    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider mainSlider;
    [SerializeField] TextMeshProUGUI mainText, musicText, SFXText;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider SFXSlider;
    public Sound[] musicSounds, sfxSounds;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);

            foreach (Sound s in musicSounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.outputAudioMixerGroup = mixer.FindMatchingGroups("Music")[0];

                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
            }

            foreach (Sound s in sfxSounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];

                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
            }
        }

        PlayMusic("Pirate Theme");
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, sound => sound.name == name);
        s.source.Play();
        s.source.loop = true;
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, sound => sound.name == name);
        if (s != null)
            s.source.Play();
    }


    public void ChangeMainVolume()
    {
        mixer.SetFloat("MasterVolume", Mathf.Log10(mainSlider.value) * 20);
        mainText.text = Mathf.Round(Mathf.Clamp(100 * mainSlider.value, 0f, 100f)).ToString();
    }
    public void ChangeSFXVolume()
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(SFXSlider.value) * 20);
        SFXText.text = Mathf.Round(Mathf.Clamp(100 * SFXSlider.value, 0f, 100f)).ToString();
    }

    public void ChangeMusicVolume()
    {
        mixer.SetFloat("MusicVolume", Mathf.Log10(musicSlider.value) * 20);
        musicText.text = Mathf.Round(Mathf.Clamp(100 * musicSlider.value, 0f, 100f)).ToString();
    }

    public void LoadData(S_O_Saving saver)
    {
        mainSlider.value = saver.audioData.MasterVolume;
        //Debug.Log(mainSlider.value);
        mainText.text = Mathf.Round(Mathf.Clamp(100 * mainSlider.value, 0f, 100f)).ToString();
        ChangeMainVolume();

        musicSlider.value = saver.audioData.MusicVolume;
        musicText.text = Mathf.Round(Mathf.Clamp(100 * musicSlider.value, 0f, 100f)).ToString();
        ChangeMusicVolume();

        SFXSlider.value = saver.audioData.SFXVolume;
        SFXText.text = Mathf.Round(Mathf.Clamp(100 * SFXSlider.value, 0f, 100f)).ToString();
        ChangeSFXVolume();
    }

    public void SaveData(S_O_Saving saver)
    {
        saver.audioData.MasterVolume = mainSlider.value;
        saver.audioData.MusicVolume = musicSlider.value;
        saver.audioData.SFXVolume = SFXSlider.value;
    }
}
