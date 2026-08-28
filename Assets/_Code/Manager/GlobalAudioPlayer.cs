using UnityEngine;
using UnityEngine.Audio;

public class GlobalAudioPlayer : MonoBehaviour
{
    public static GlobalAudioPlayer Instance { get; private set; }

    [Header("Audio Settings")]
    [SerializeField] private AudioMixerGroup defaultSFXOutput;
    [SerializeField] private AudioMixerGroup defaultMusicOutput;
    [SerializeField] private AudioSource gameMusicSource; 
    [SerializeField] private AudioClip gameMusicClip;
    public bool isGameOver = false;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        if (!isGameOver)
        {
            gameMusicSource.outputAudioMixerGroup = defaultMusicOutput;
            gameMusicSource.clip = gameMusicClip;
            gameMusicSource.Play();
        }
        DontDestroyOnLoad(gameObject);
    }
    public void PlayAudio(AudioClip audioClip, Transform location = null, float pitch = 1)
    {
        if (audioClip == null)
        {
            Debug.LogWarning("GlobalAudioPlayer: AudioClip is null.");
            return;
        }

        if (isGameOver)
        {
            // Stop every AudioSource currently playing
            AudioSource[] audioSources = GetComponentsInChildren<AudioSource>();

            foreach (AudioSource source in audioSources)
            {
                source.Stop();
            }

            // Destroy every child of GlobalAudioPlayer
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        GameObject audioObject = new GameObject("TempAudio_" + audioClip.name);

        // Keep temporary audio objects organized
        audioObject.transform.SetParent(transform);

        // Always keep temporary audio at the GlobalAudioPlayer position
        audioObject.transform.position = transform.position;

        AudioSource audioSource = audioObject.AddComponent<AudioSource>();

        audioSource.clip = audioClip;
        audioSource.outputAudioMixerGroup = defaultSFXOutput;

        // 2D audio
        audioSource.spatialBlend = 0f;
        audioSource.pitch = pitch;
        audioSource.Play();

        // Destroy after the clip finishes
        Destroy(audioObject, audioClip.length);
    }
}