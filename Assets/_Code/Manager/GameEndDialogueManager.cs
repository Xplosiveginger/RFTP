using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameEndDialogueManager : MonoBehaviour
{
    [Header("Dialogue")]
    [TextArea(2, 5)]
    [SerializeField] private List<string> dialogues = new List<string>();

    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Typewriter")]
    [SerializeField] private float typingSpeed = 0.03f;

    [Header("Dialogue Timing")]
    [SerializeField] private float dialogueGap = 1f;

    private Coroutine dialogueCoroutine;

    private void OnEnable()
    {
        dialogueCoroutine = StartCoroutine(PlayDialogue());
    }

    private void OnDisable()
    {
        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
            dialogueCoroutine = null;
        }
    }

    private IEnumerator PlayDialogue()
    {
        if (dialogueText == null)
        {
            Debug.LogWarning("[GameEndDialogueManager] Dialogue Text reference is missing!");
            yield break;
        }

        if (dialogues == null || dialogues.Count == 0)
        {
            Debug.LogWarning("[GameEndDialogueManager] No dialogues configured!");
            yield break;
        }

        foreach (string dialogue in dialogues)
        {
            dialogueText.text = "";

            // Typewriter effect
            foreach (char character in dialogue)
            {
                dialogueText.text += character;
                yield return new WaitForSeconds(typingSpeed);
            }

            // Gap before the next dialogue
            yield return new WaitForSeconds(dialogueGap);
        }

        // All dialogue completed.
        // We will add the next functionality here later.
    }
}