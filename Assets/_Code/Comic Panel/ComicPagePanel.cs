using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ComicPagePanel : MonoBehaviour
{
    #region Variables
    RectTransform canvas;

    [Header("Comic Page Properties")]
    public bool playOnAwake = false;
    public List<PanelProperties> panelProperties = new List<PanelProperties>();
    [Space]
    public ComicPagePanel nextPage;
    public EPageTransitionMethod pageTransition = EPageTransitionMethod.NotSet;
    public float delayBeforePageTransition = .5f;
    public float buttonVisibilityDelay = .5f;
    public Button button;

    private List<Vector2> endPositions;
    private Coroutine panelAnimationRoutine;
    #endregion

    #region Events
    public UnityEvent OnAnimationCompleted;
    #endregion

    #region Built-In Methods

    public void OnEnable()
    {
        ComicPanelExitController.stopPanelAnimation += StopAnimation;
    }

    private void OnDisable()
    {
        ComicPanelExitController.stopPanelAnimation -= StopAnimation;
    }

    private void Start()
    {
        Initialize();
    }
    #endregion

    #region Custom Methods
    private void Initialize()
    {
        Canvas.ForceUpdateCanvases();

        canvas = GetComponent<RectTransform>();
        endPositions = new List<Vector2>();

        if (button)
        {
            button.onClick.AddListener(PlayTransitionToNextPage);
            button.gameObject.SetActive(false);
        }

        foreach (PanelProperties panel in panelProperties)
        {
            RectTransform panelRect = panel.panel;
            panel.panel.DOKill();
            endPositions.Add(panelRect.anchoredPosition);
            CalculateStartPosition(panel);
        }

        if (playOnAwake)
        {
            PlayAnimation();
        }
    }

    public void CalculateStartPosition(PanelProperties panel)
    {
        ERevealDirection revDir = panel.revealDirection;
        float halfHeight = panel.panel.rect.size.y * 0.5f;
        float halfWidth = panel.panel.rect.size.x * 0.5f;

        switch (revDir)
        {
            case ERevealDirection.Top:
                float panelYPos = GetCanvasHalfHeight();
                panelYPos = panelYPos + halfHeight + 100f;
                panel.panel.anchoredPosition = new Vector2(panel.panel.anchoredPosition.x, panelYPos);
                break;
            case ERevealDirection.Bottom:
                float YPos = GetCanvasHalfHeight() * -1f;
                YPos = YPos - halfHeight - 100f;
                panel.panel.anchoredPosition = new Vector2(panel.panel.anchoredPosition.x, YPos);
                break;
            case ERevealDirection.Left:
                float panelXPos = GetCanvasHalfWidth() * -1f;
                panelXPos = panelXPos - halfWidth - 100f;
                panel.panel.anchoredPosition = new Vector2(panelXPos, panel.panel.anchoredPosition.y);
                break;
            case ERevealDirection.Right:
                float XPos = GetCanvasHalfWidth();
                XPos = XPos + halfWidth + 100f;
                panel.panel.anchoredPosition = new Vector2(XPos, panel.panel.anchoredPosition.y);
                break;
        }
    }

    public float GetCanvasHalfHeight()
    {
        float canvasHeight = canvas.rect.size.y;
        float canvasHalfHeight = canvasHeight * 0.5f;

        return canvasHalfHeight;
    }

    public float GetCanvasHalfWidth()
    {
        float canvasWidth = canvas.rect.size.x;
        float canvasHalfWidth = canvasWidth * 0.5f;

        return canvasHalfWidth;
    }

    [ContextMenu("Play Animation")]
    public void PlayAnimation()
    {
        panelAnimationRoutine = StartCoroutine(AnimateComicPanel());
    }

    [ContextMenu("Transition To Next Page")]
    public void PlayTransitionToNextPage()
    {
        Debug.Log("Transition To Next Page");
        if (nextPage == null) return;

        gameObject.SetActive(false);
        nextPage.gameObject.SetActive(true);
        nextPage.PlayAnimation();
    }

    private void EnableButton()
    {
        button.gameObject.SetActive(true);
    }

    IEnumerator AnimateComicPanel()
    {
        for (int i = 0; i < panelProperties.Count; i++)
        {
            PanelProperties panel = panelProperties[i];

            yield return new WaitForSeconds(panel.delay);

            RectTransform rect = panel.panel;

            yield return rect
                .DOAnchorPos(endPositions[i], panel.duration)
                .SetEase(panel.ease)
                .WaitForCompletion();
        }

        OnAnimationCompleted?.Invoke();

        if (nextPage != null && pageTransition == EPageTransitionMethod.UIButton)
        {
            StartCoroutine(EnableButtonGFX());
        }

        if (nextPage != null && pageTransition == EPageTransitionMethod.Auto)
        {
            StartCoroutine(TransitionTonextPage());
        }
    }

    IEnumerator TransitionTonextPage()
    {
        yield return new WaitForSeconds(delayBeforePageTransition);
        PlayTransitionToNextPage();
    }

    IEnumerator EnableButtonGFX()
    {
        yield return new WaitForSeconds(buttonVisibilityDelay);
        EnableButton();
    }

    public void StopAnimation()
    {
        if (panelAnimationRoutine != null)
        {
            StopCoroutine(panelAnimationRoutine);
            panelAnimationRoutine = null;
        }
    }
    #endregion
}

public enum ERevealDirection
{
    Top,
    Bottom,
    Left,
    Right
}

public enum EPageTransitionMethod
{
    NotSet,
    Auto,
    KeyPress,
    UIButton,
}

[System.Serializable]
public struct PanelProperties
{
    public RectTransform panel;
    public ERevealDirection revealDirection;
    public float delay;
    public float duration;
    public Ease ease;
}