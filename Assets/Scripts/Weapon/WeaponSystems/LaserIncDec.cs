using UnityEngine;

public class LaserIncDec : MonoBehaviour
{
    [Header("Target Object")]
    public Transform targetObject; // Object to scale

    [Header("Scale Settings")]
    public Vector3 targetScale = new Vector3(1f, 1f, 1f); // Max scale
    public float scaleSpeed = 2f; // Units per second
    private Vector3 initialScale;

    private bool scalingUp = false;
    private bool scalingDown = false;

    private void Awake()
    {
        if (targetObject == null)
            targetObject = transform;

        initialScale = targetObject.localScale;
    }

    private void OnEnable()
    {
        scalingUp = true;
        scalingDown = false;
    }

    private void OnDisable()
    {
        scalingDown = true;
        scalingUp = false;
    }

    private void Update()
    {
        if (targetObject == null) return;

        if (scalingUp)
        {
            targetObject.localScale = Vector3.MoveTowards(targetObject.localScale, targetScale, scaleSpeed * Time.deltaTime);

            // Stop when reached target
            if (targetObject.localScale == targetScale)
                scalingUp = false;
        }
        else if (scalingDown)
        {
            targetObject.localScale = Vector3.MoveTowards(targetObject.localScale, initialScale, scaleSpeed * Time.deltaTime);

            if (targetObject.localScale == initialScale)
                scalingDown = false;
        }
    }
}
