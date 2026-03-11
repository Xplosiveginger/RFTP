using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{

    [SerializeField] private AudioSource soundFXObject;

    [Header("Audioclips")]
    
    [SerializeField] private AudioClip MainMusic;
    [SerializeField] private AudioClip UIButtonSFX;
    [SerializeField] private AudioClip UIBuyButtonSFX;
    [SerializeField] private AudioClip UIRefundButtonSFX;
    [SerializeField] private AudioClip UIPauseButtonSFX;

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

    public void PlaySoundFxClip(AudioClip audioClip,Transform spawnTransform,float volume)
    {

        //spawn in GameObject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        //assign the Audioclip
        audioSource.clip = audioClip;

        //assign volume
        audioSource.volume = volume;
        //play sound
        audioSource.Play();

        //get length of sound effect 
        float clipLength = audioSource.clip.length;

        //destroy the clip after it is done playing
        Destroy(audioSource.gameObject,clipLength);

    }
}
