using UnityEngine;

public class GlobalParticleFxPlayer : MonoBehaviour
{
    public static GlobalParticleFxPlayer Instance;

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

    public void PlayParticle(ParticleSystem particle, Vector2 position)
    {
        if (particle == null) return;
        ParticleSystem fx = Instantiate(particle,position,Quaternion.identity);
        fx.Play();
    }
}
