using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySettings : MonoBehaviour
{
    [Header("Resolution")]
    public TMP_Dropdown resolutionDropdown;

    [Header("Graphics Quality")]
    public TMP_Dropdown qualityDropdown;

    [Header("Fullscreen")]
    public Button fullscreenButton;
    public Image fullscreenButtonImage;
    public Color fullscreenOnColor = Color.green;
    public Color fullscreenOffColor = Color.black;

    private readonly Vector2Int[] resolutions =
    {
        new Vector2Int(1920, 1080),
        new Vector2Int(2560, 1440),
        new Vector2Int(3840, 2160)
    };

    private bool isFullscreen;

    private void Start()
    {
        isFullscreen = Screen.fullScreen;

        SetupResolutionDropdown();
        SetupQualityDropdown();
        UpdateFullscreenButton();
    }

    private void SetupResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        foreach (Vector2Int resolution in resolutions)
        {
            options.Add($"{resolution.x} x {resolution.y}");
        }

        resolutionDropdown.AddOptions(options);

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (Screen.currentResolution.width == resolutions[i].x &&
                Screen.currentResolution.height == resolutions[i].y)
            {
                resolutionDropdown.value = i;
                break;
            }
        }

        resolutionDropdown.RefreshShownValue();
    }

    private void SetupQualityDropdown()
    {
        qualityDropdown.ClearOptions();

        qualityDropdown.AddOptions(new List<string>()
        {
            "Low",
            "Medium",
            "High"
        });

        qualityDropdown.value = Mathf.Clamp(QualitySettings.GetQualityLevel(), 0, 2);
        qualityDropdown.RefreshShownValue();
    }

    public void ToggleFullscreen()
    {
        isFullscreen = !isFullscreen;
        UpdateFullscreenButton();
    }

    private void UpdateFullscreenButton()
    {
        fullscreenButtonImage.color =
            isFullscreen ? fullscreenOnColor : fullscreenOffColor;
    }

    public void ApplySettings()
    {
        Vector2Int resolution = resolutions[resolutionDropdown.value];

        Screen.SetResolution(
            resolution.x,
            resolution.y,
            isFullscreen
        );

        QualitySettings.SetQualityLevel(
            qualityDropdown.value,
            true
        );
    }

    // Assign this to the Exit button in the Main Menu
    public void ExitToMainMenu()
    {

    }

    // Assign this to the Exit button in the Pause Menu
    public void ExitToPauseMenu()
    {

    }
}