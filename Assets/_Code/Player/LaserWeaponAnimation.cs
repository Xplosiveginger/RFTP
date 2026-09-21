using UnityEngine;

public class LaserWeaponAnimation : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 1f;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Material material;
    private Vector2 offset;

    private void Start()
    {
        material = spriteRenderer.material;

        Debug.Log("Material: " + material.name);
        Debug.Log("Initial Offset: " + material.mainTextureOffset);
    }

    private void Update()
    {
        offset.x += scrollSpeed * Time.deltaTime;

        material.mainTextureOffset = offset;
        Debug.Log("Offset: " + material.mainTextureOffset);

    }
}
