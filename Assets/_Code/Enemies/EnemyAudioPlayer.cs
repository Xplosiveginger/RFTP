using System;
using UnityEngine;

public class EnemyAudioPlayer : MonoBehaviour
{
    public AudioClip audioClip;
    private HealthSystem healthSystem;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
    }

    private void OnEnable()
    {
        healthSystem?.onDeath.AddListener(PlayAudio);
    }

    private void OnDisable()
    {
        healthSystem?.onDeath.RemoveListener(PlayAudio);
    }

    private void PlayAudio()
    {
        GlobalAudioPlayer.Instance.PlayAudio(audioClip);
    }
}