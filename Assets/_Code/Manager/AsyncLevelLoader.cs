using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AsyncLevelLoader : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject loadingPanel;

    public Image loadingBar;

    public string levelName;

    public float loadingTime;
    private float loadingTimer;

    public TextMeshProUGUI hintText;
    private void Start()
    {
        loadingPanel.SetActive(false);
    }


    private IEnumerator LoadLevelAsync(string levelToLoad)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelToLoad);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingBar.fillAmount = progress;
            
            if (operation.progress >= 0.9f)
            {
                loadingTimer += Time.deltaTime;

                hintText.text = "Here is a random hint...";

                if (loadingTimer >= loadingTime)
                {
                    operation.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }
    public void OnPlayButtonClicked()
    {
        loadingPanel.SetActive(true);
        StartCoroutine(LoadLevelAsync(levelName));
    }
}
