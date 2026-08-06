using System;
using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    public AudioMixer audioMixer;

    private float masterVolume = 100f;
    private float musicVolume = 100f;
    private float sfxVolume = 100f;

    private bool isMasterMuted = false;
    

    public void SetMasterVolume(float value)
    {
        masterVolume = value;

        value = Mathf.Clamp(value, 0.0001f, 100f);
        float volume = value / 100f;

        if (!isMasterMuted)
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;

        value = Mathf.Clamp(value, 0.0001f, 100f);
        float volume = value / 100f;

        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;

        value = Mathf.Clamp(value, 0.0001f, 100f);
        float volume = value / 100f;

        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }

    public void ToggleMasterMute()
    {
        isMasterMuted = !isMasterMuted;

        if (isMasterMuted)
        {
            audioMixer.SetFloat("MasterVolume", -80f);
        }
        else
        {
            float volume = Mathf.Clamp(masterVolume, 0.0001f, 100f) / 100f;
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        }
    }
}