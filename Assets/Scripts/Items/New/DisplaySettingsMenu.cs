using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Unity.VisualScripting;

public class DisplaySettingsMenu : MonoBehaviour
{
    [Header("Panel")]
    public GameObject settingsPanel;

    [Header("Display")]
    public Toggle fullscreenToggle;
    public TMP_Dropdown resolutionDropdown;

    public GameObject videoPanel;
    public GameObject audioPanel;

    Resolution[] resolutions;
    List<Resolution> uniqueResolutions = new List<Resolution>();

    // =====================================================
    void Start()
    {
        SetupResolutions();

        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        resolutionDropdown.onValueChanged.AddListener(SetResolution);

        fullscreenToggle.isOn = Screen.fullScreen;
    }

    // =====================================================
    // PANEL
    // =====================================================
    public void OpenSettings() => settingsPanel.SetActive(true);
    public void CloseSettings() => settingsPanel.SetActive(false);

    // =====================================================
    // RESOLUTION
    // =====================================================
    void SetupResolutions()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        HashSet<string> added = new HashSet<string>();
        List<string> options = new List<string>();

        int currentIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            Resolution r = resolutions[i];
            string label = r.width + " x " + r.height;

            if (!added.Contains(label))
            {
                added.Add(label);
                uniqueResolutions.Add(r);
                options.Add(label);

                if (r.width == Screen.currentResolution.width &&
                    r.height == Screen.currentResolution.height)
                {
                    currentIndex = uniqueResolutions.Count - 1;
                }
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentIndex;
        resolutionDropdown.RefreshShownValue();
    }

    void SetResolution(int index)
    {
        Resolution r = uniqueResolutions[index];

        Screen.SetResolution(r.width, r.height, Screen.fullScreen);

        Debug.Log("Resolution changed to: " + r.width + " x " + r.height);
    }

    // =====================================================
    // FULLSCREEN
    // =====================================================
    void SetFullscreen(bool isFull)
    {
        Screen.fullScreenMode = isFull
     ? FullScreenMode.FullScreenWindow
     : FullScreenMode.Windowed;

        Debug.Log("Fullscreen: " + Screen.fullScreen);
    }

    public void OnVideoClick()
    {
        ShowPanel(videoPanel);
    }

    public void OnAudioClick()
    {
        ShowPanel(audioPanel);
    }

    void ShowPanel(GameObject panelToShow)
    {
        videoPanel.SetActive(false);
        audioPanel.SetActive(false);

        panelToShow.SetActive(true);
    }
}