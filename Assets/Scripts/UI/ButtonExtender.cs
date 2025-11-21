using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonExtender : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public TMP_Text textAsset;

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector3 localPos = textAsset.GetComponent<RectTransform>().localPosition;
        localPos.y = 0;
        textAsset.GetComponent<RectTransform>().localPosition = localPos;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Vector3 localPos = textAsset.GetComponent<RectTransform>().localPosition;
        localPos.y = 5f;
        textAsset.GetComponent<RectTransform>().localPosition = localPos;
    }
}
