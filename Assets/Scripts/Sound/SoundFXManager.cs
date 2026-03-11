using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class SoundFXManager : MonoBehaviour
{

    [SerializeField] private AudioSource soundFXObject;

    [Header("Audio Clips")]
    
    [SerializeField] private AudioClip MainMusic;
    [SerializeField] private AudioClip UIButtonSFX;
    [SerializeField] private AudioClip UIBuyButtonSFX;
    [SerializeField] private AudioClip UIRefundButtonSFX;
    [SerializeField] private AudioClip UIPauseButtonSFX;

    [SerializeField] private AudioClip UILevelUpBarSFX;
    [SerializeField] private AudioClip UICardSelectionSFX;
    [SerializeField] private AudioClip FurnitureSFX;
    [SerializeField] private AudioClip EnemyKilledSFX;
    [SerializeField] private AudioClip EXPStarSFX;
    [SerializeField] private AudioClip PlayerDmgSFX;
    [SerializeField] private AudioClip GlassShatterSFX;
    [SerializeField] private AudioClip SmokeSFX;
    [SerializeField] private AudioClip PoofSFX;
    [SerializeField] private AudioClip AcidRainSFX;
    [SerializeField] private AudioClip LaserSFX;
    [SerializeField] private AudioClip PlanetSFX;
    [SerializeField] private AudioClip PrismSFX;
    [SerializeField] private AudioClip MagnetSFX;
    [SerializeField] private AudioClip BacteriaSFX;
    [SerializeField] private AudioClip TuningForkSFX;
    [SerializeField] private AudioClip LithiumIonBatterySFX;
    [SerializeField] private AudioClip HealthSFX;
    [SerializeField] private AudioClip TestTubeSFX;
    [SerializeField] private AudioClip BurningSFX;

    public static SoundFXManager instance;
    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void PlaySoundFxClip(AudioClip audioClip,Transform spawnTransform,float volume)
    {
        //
        //spawn in GameObject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        //assign the Audioclip
        audioSource.clip = audioClip;
        audioSource.pitch = 3;
        //assign volume
        audioSource.volume = volume;
        //play sound
        audioSource.Play();

        //get length of sound effect 
        float clipLength = audioSource.clip.length;

        //destroy the clip after it is done playing
        Destroy(audioSource.gameObject,clipLength);

    }
    public void PlayMainMusic(Transform transform, float volume = 1f)
    {
        PlaySoundFxClip(MainMusic, transform, volume);
    }
    public void PlayUIButtonSFX(Transform transform,float volume = 1f)
    {
        PlaySoundFxClip(UIButtonSFX, transform, volume);
    }

    public void PlayUIBuyButtonSFX(Transform transform, float volume = 1f)
    {
        PlaySoundFxClip(UIBuyButtonSFX, transform, volume);
    }
    public void PlayUIRefundButtonSFX(Transform transform, float volume = 1f)
    {
        PlaySoundFxClip(UIRefundButtonSFX, transform, volume);
    }
    public void PlayUIPauseButtonSFX(Transform transform, float volume = 1f)
    {
        PlaySoundFxClip(UIPauseButtonSFX, transform, volume);
    }
    public void PlayUILevelUpBarSFX(Transform transform, float volume = 1f)
    {
        PlaySoundFxClip(UILevelUpBarSFX, transform, volume);
    }
    public void PlayUICardSelectionSFX(Transform transform, float volume = 1f)
    {
        PlaySoundFxClip(UICardSelectionSFX, transform, volume);
    }
    public void PlayFurnitureSFX(Transform transform, float volume = 1f)
    {
        PlaySoundFxClip(FurnitureSFX, transform, volume);
    }
    public void PlayEnemyKilledSFX(Transform transform, float volume = 1f)
    {
        PlaySoundFxClip(EnemyKilledSFX, transform, volume);
    }
    public void PlayEXPStarSFX(Transform transform, float volume = 1f)
    {
        PlaySoundFxClip(EXPStarSFX, transform, volume);
    }
    public void PlayPlayerDmgSFX(Transform transform, float volume = 1f)
    {
        PlaySoundFxClip(PlayerDmgSFX, transform, volume);
    }
    public void PlayGlassShatterSFX(Transform transform, float volume = 1f)
    {
        PlaySoundFxClip(GlassShatterSFX, transform, volume);
    }
    public void PlaySmokeSFX(Transform transform, float volume = 1f)
    {
        PlaySoundFxClip(SmokeSFX, transform, volume);
    }
    public void PlayPoofSFX(Transform transform, float volume = 1f)
    {
        PlaySoundFxClip(PoofSFX, transform, volume);
    }
    public void PlayAcidRainSFX(Transform transform, float volume = 1f)
    {
        PlaySoundFxClip(AcidRainSFX, transform, volume);
    }
    public void PlayLaserSFX(Transform transform, float volume = 1f)
    {
        PlaySoundFxClip(LaserSFX, transform, volume);
    }
    public void PlayPlanetSFX(Transform transform, float volume = 1f)
    {
        PlaySoundFxClip(PlanetSFX, transform, volume);
    }
    public void PlayPrismSFX(Transform transform, float volume = 1f)
    {
        PlaySoundFxClip(PrismSFX, transform, volume);
    }
    public void PlayMagnetSFX(Transform transform, float volume = 1f)
    {
        PlaySoundFxClip(MagnetSFX, transform, volume);
    }
    public void PlayBacteriaSFX(Transform transform, float volume = 1f)
    {
        PlaySoundFxClip(BacteriaSFX, transform, volume);
    }
    public void PlayTuningForkSFX(Transform transform, float volume = 1f)
    {
        PlaySoundFxClip(TuningForkSFX, transform, volume);
    }
    public void PlayLithiumIonBatterySFX(Transform transform, float volume = 1f)
    {
        PlaySoundFxClip(LithiumIonBatterySFX, transform, volume);
    }
    public void PlayHealthSFX(Transform transform, float volume = 1f)
    {
        PlaySoundFxClip(HealthSFX, transform, volume);
    }
    public void PlayTestTubeSFX(Transform transform, float volume = 1f)
    {
        PlaySoundFxClip(TestTubeSFX, transform, volume);
    }
    public void PlayBurningSFX(Transform transform, float volume = 1f)
    {
        PlaySoundFxClip(BurningSFX, transform, volume);
    }



}
