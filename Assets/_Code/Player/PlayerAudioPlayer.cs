using UnityEngine;

public class PlayerAudioPlayer : MonoBehaviour
{
    public AudioClip audioClip;
    private HealthSystem healthSystem;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
    }

    private void OnEnable()
    {
        healthSystem?.onDamageTaken.AddListener(PlayAudio);
    }

    private void OnDisable()
    {
        healthSystem?.onDamageTaken.RemoveListener(PlayAudio);
    }

    private void PlayAudio(int x)
    {
        GlobalAudioPlayer.Instance.PlayAudio(audioClip);
    }
}
