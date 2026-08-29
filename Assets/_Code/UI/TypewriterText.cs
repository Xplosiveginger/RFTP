using System.Collections;
using TMPro;
using UnityEngine;

public class TypewriterText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float completionTime = 20f;

    private Coroutine writingCoroutine;


    private void Start()
    {
        StartWriting();
    }
    public void StartWriting()
    {
        if (writingCoroutine != null)
            StopCoroutine(writingCoroutine);

        writingCoroutine = StartCoroutine(WriteText());
    }

    private IEnumerator WriteText()
    {
        text.maxVisibleCharacters = 0;

        int characterCount = text.textInfo.characterCount;

        if (characterCount == 0)
            yield break;

        float characterInterval = completionTime / characterCount;

        for (int i = 0; i <= characterCount; i++)
        {
            text.maxVisibleCharacters = i;
            yield return new WaitForSeconds(characterInterval);
        }

        text.maxVisibleCharacters = characterCount;
        writingCoroutine = null;
    }
}