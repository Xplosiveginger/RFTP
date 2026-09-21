using System;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewController : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    [SerializeField] private RectTransform viewport;

    private void Start()
    {
        UpdateScrollState();
    }

    [ContextMenu("UpdateScrollState")]
    public void UpdateScrollState()
    {
        Canvas.ForceUpdateCanvases();

        scrollRect.vertical = content.rect.height > viewport.rect.height;
    }
}