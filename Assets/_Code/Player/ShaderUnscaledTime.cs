using UnityEngine;

public class ShaderUnscaledTime : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private float scrollSpeed = 1f;

    void Update()
    {
        material.SetFloat("_UnscaledTime", Time.unscaledTime);
    }
}