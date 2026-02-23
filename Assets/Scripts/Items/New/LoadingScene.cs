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

    [Header("Progress UI (Optional)")]
    public Slider progressBar;
    public TextMeshProUGUI progressText;

    [Header("Fake Loading Speed")]
    public float fakeSpeed = 0.5f;
    private Scene oldScene;

    // =========================================
    // PUBLIC CALL
    // =========================================
    public void LoadScene(int sceneId)
    {
        StartCoroutine(LoadRoutine(sceneId));
    }

    // =========================================
    // MAIN ROUTINE (NO FADE)
    // =========================================
    IEnumerator LoadRoutine(int sceneId)
    {
        oldScene = SceneManager.GetActiveScene();

        // show loading UI
        currentPanel.SetActive(false);
        loadingScreen.SetActive(true);

        UpdateProgress(0f);

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneId, LoadSceneMode.Additive);
        op.allowSceneActivation = false;

        float fakeProgress = 0f;

        while (fakeProgress < 1f)
        {
            // real progress (0–1)
            float realProgress = Mathf.Clamp01(op.progress / 0.9f);

            // smooth move towards real progress
            fakeProgress = Mathf.MoveTowards(fakeProgress, realProgress, Time.deltaTime * 0.5f);

            UpdateProgress(fakeProgress);

            // break only when fully loaded
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

        // hide loading UI
        loadingScreen.SetActive(false);
    }

    // =========================================
    // PROGRESS BAR + TEXT
    // =========================================
    void UpdateProgress(float value)
    {
        if (progressBar != null)
            progressBar.value = value;

        if (progressText != null)
            progressText.text = Mathf.RoundToInt(value * 100f) + "%";
    }
}