using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplaySettings : MonoBehaviour
{
    [Header("Resolution")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    [Header("Screen Mode")]
    [SerializeField] private TMP_Dropdown screenModeDropdown;

    private readonly Vector2Int[] resolutions =
    {
        new Vector2Int(1920, 1080),
        new Vector2Int(2560, 1440),
        new Vector2Int(3840, 2160)
    };

    private void Start()
    {
        SetupResolutionDropdown();
        SetupScreenModeDropdown();
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

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (Screen.width == resolutions[i].x &&
                Screen.height == resolutions[i].y)
            {
                currentResolutionIndex = i;
                break;
            }
        }

        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    private void SetupScreenModeDropdown()
    {
        screenModeDropdown.ClearOptions();

        screenModeDropdown.AddOptions(new List<string>
        {
            "Fullscreen",
            "Windowed"
        });

        screenModeDropdown.value =
            Screen.fullScreenMode == FullScreenMode.FullScreenWindow ? 0 : 1;

        screenModeDropdown.RefreshShownValue();
    }

    public void ApplySettings()
    {
        Vector2Int resolution = resolutions[resolutionDropdown.value];

        FullScreenMode screenMode =
            screenModeDropdown.value == 0
                ? FullScreenMode.FullScreenWindow
                : FullScreenMode.Windowed;

        Screen.SetResolution(
            resolution.x,
            resolution.y,
            screenMode
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