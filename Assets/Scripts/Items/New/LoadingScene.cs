using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingScene : MonoBehaviour
{
    [Header("Panels")]
    public GameObject currentPanel;
    public GameObject loadingScreen;

    [Header("Progress Image")]
    public Image progressImage;

  

    [Header("Loading Speed")]
    public float fakeSpeed = 0.5f;

    private Scene oldScene;
    private bool isLoading = false;

    // =========================================
    // PUBLIC CALL
    // =========================================
    public void LoadScene(int sceneId)
    {
        if (isLoading) return;

        StartCoroutine(LoadRoutine(sceneId));
    }

    // =========================================
    // MAIN LOADING ROUTINE
    // =========================================
    IEnumerator LoadRoutine(int sceneId)
    {
        isLoading = true;

        oldScene = SceneManager.GetActiveScene();

        currentPanel.SetActive(false);
        loadingScreen.SetActive(true);

        UpdateProgress(0f);

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneId, LoadSceneMode.Additive);
        op.allowSceneActivation = false;

        float fakeProgress = 0f;

        while (!op.isDone)
        {
            float realProgress = Mathf.Clamp01(op.progress / 0.9f);

            fakeProgress = Mathf.MoveTowards(fakeProgress, realProgress, Time.deltaTime * fakeSpeed);

            UpdateProgress(fakeProgress);

            if (realProgress >= 1f && fakeProgress >= 1f)
                break;

            yield return null;
        }

        UpdateProgress(1f);

        op.allowSceneActivation = true;

        while (!op.isDone)
            yield return null;

        Scene newScene = SceneManager.GetSceneByBuildIndex(sceneId);
        SceneManager.SetActiveScene(newScene);

        if (oldScene.isLoaded)
            yield return SceneManager.UnloadSceneAsync(oldScene);

        loadingScreen.SetActive(false);

        isLoading = false;
    }

    // =========================================
    // UPDATE UI
    // =========================================
    void UpdateProgress(float value)
    {
        if (progressImage != null)
            progressImage.fillAmount = value;

       
    }
}