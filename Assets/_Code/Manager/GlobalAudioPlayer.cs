using UnityEngine;
using UnityEngine.Audio;

public class GlobalAudioPlayer : MonoBehaviour
{
    public static GlobalAudioPlayer Instance { get; private set; }

    [Header("Audio Settings")]
    [SerializeField] private AudioMixerGroup defaultSFXOutput;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayAudio(AudioClip audioClip, Transform location = null)
    {
        if (audioClip == null)
        {
            Debug.LogWarning("GlobalAudioPlayer: AudioClip is null.");
            return;
        }

        GameObject audioObject = new GameObject("TempAudio_" + audioClip.name);

        // Keep temporary audio objects organized
        audioObject.transform.SetParent(transform);

        // Always keep the audio source at the GlobalAudioPlayer position
        audioObject.transform.position = transform.position;

        AudioSource audioSource = audioObject.AddComponent<AudioSource>();

        audioSource.clip = audioClip;
        audioSource.outputAudioMixerGroup = defaultSFXOutput;

        // 2D audio
        audioSource.spatialBlend = 0f;

        audioSource.Play();

        // Destroy after the clip finishes
        Destroy(audioObject, audioClip.length);
    }
}