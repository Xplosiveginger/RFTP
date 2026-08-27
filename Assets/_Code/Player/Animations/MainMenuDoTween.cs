using UnityEngine;
using DG.Tweening;

public class MainMenuDoTween : MonoBehaviour
{
    private RectTransform rectTransform;

    [Header("Animation Type")]
    public bool isScaling;
    public bool isSliding;
    public bool isBouncing;
    public bool isUpDown;

    [Header("General Settings")]
    public bool infinite = true;
    public float duration = 0.5f;

    [Header("Scaling")]
    [Tooltip("Scale multiplier for the beat effect.")]
    public float scale = 1.1f;

    [Header("Sliding")]
    public Vector2 startPosition;
    public Vector2 finalPosition;

    [Header("Bouncing")]
    public Vector2 bounceStartPosition;
    public Vector2 bounceFinalPosition;

    [Header("Up Down")]
    [Tooltip("How far the object moves up and down.")]
    public float upDownAmount = 20f;

    [Tooltip("Time taken to move from one position to the other.")]
    public float upDownDuration = 1f;

    private Vector3 originalScale;
    private Tween animationTween;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
    }

    private void Start()
    {
        if (isScaling)
        {
            PlayScaling();
        }
        else if (isSliding)
        {
            PlaySliding();
        }
        else if (isBouncing)
        {
            PlayBouncing();
        }
        else if (isUpDown)
        {
            PlayUpDown();
        }
    }

    private void PlayScaling()
    {
        rectTransform.localScale = originalScale;

        Vector3 targetScale = originalScale * scale;

        animationTween = rectTransform
            .DOScale(targetScale, duration)
            .SetEase(Ease.InOutSine);

        if (infinite)
        {
            animationTween.SetLoops(-1, LoopType.Yoyo);
        }
    }

    private void PlaySliding()
    {
        rectTransform.anchoredPosition = startPosition;

        animationTween = rectTransform
            .DOAnchorPos(finalPosition, duration)
            .SetEase(Ease.OutCubic);

        if (infinite)
        {
            animationTween.SetLoops(-1, LoopType.Restart);
        }
    }

    private void PlayBouncing()
    {
        rectTransform.anchoredPosition = bounceStartPosition;

        animationTween = rectTransform
            .DOAnchorPos(bounceFinalPosition, duration)
            .SetEase(Ease.OutBounce);

        if (infinite)
        {
            animationTween.SetLoops(-1, LoopType.Restart);
        }
    }

    private void PlayUpDown()
    {
        Vector2 startPosition = rectTransform.anchoredPosition;
        Vector2 upPosition = startPosition + Vector2.up * upDownAmount;

        animationTween = rectTransform
            .DOAnchorPos(upPosition, upDownDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDestroy()
    {
        animationTween?.Kill();
    }
}