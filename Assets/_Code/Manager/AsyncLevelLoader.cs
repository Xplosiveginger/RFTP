using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AsyncLevelLoader : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject loadingPanel;


    public string gameLevelName;
    public string menuLevelName;

    public float loadingTime;
    private float loadingTimer;
    
    public Image loadingMask;

    public TextMeshProUGUI hintText;

    public string[] randomHints;

    private void Start()
    {
        loadingPanel.SetActive(false);

    }
    private IEnumerator LoadLevelAsync(string levelToLoad)
    {
        loadingTimer = 0f;

        hintText.text = randomHints[Random.Range(0, randomHints.Length)];

        loadingMask.fillAmount = 1f;

        AsyncOperation operation = SceneManager.LoadSceneAsync(levelToLoad);
        operation.allowSceneActivation = false;

        bool timerFinished = false;
        bool sceneLoaded = false;

        while (!operation.isDone)
        {
            // Update timer
            if (!timerFinished)
            {
                loadingTimer += Time.unscaledDeltaTime;

                float normalized = Mathf.Clamp01(loadingTimer / loadingTime);

                // Reduce fill from 1 -> 0
                loadingMask.fillAmount = 1f - normalized;

                if (loadingTimer >= loadingTime)
                {
                    timerFinished = true;
                    loadingMask.fillAmount = 0f;
                }
            }

            // Scene has finished loading in the background
            if (operation.progress >= 0.9f)
            {
                sceneLoaded = true;
            }

            // Wait until BOTH are complete
            if (timerFinished && sceneLoaded)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }   
    public void OnPlayButtonClicked()
    {
        loadingPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        MainMenuManager.instance.OnClickPlaySound();
        StartCoroutine(LoadLevelAsync(gameLevelName));
    }
    public void OnMenuButtonClicked()
    {
        PauseManager.instance.gameScreenCanvas.SetActive(false);
        loadingPanel.SetActive(true);
        StartCoroutine(LoadLevelAsync(menuLevelName));
    }
}
