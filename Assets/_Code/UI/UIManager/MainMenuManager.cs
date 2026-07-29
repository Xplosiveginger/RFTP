using System;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject settingsPanel;

    [Header("Setting Sub Menus")] 
    public GameObject displaySettingPanel;
    public GameObject gameSettingsPanel;
    public GameObject audioSettingsPanel;
    public GameObject graphicsSettingsPanel;

    private void Start()
    {
        startPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void OnClickedSettingButton()
    {
        startPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }
    
    public void OnClickedSettingExitButton()
    {
        settingsPanel.SetActive(false);
        startPanel.SetActive(true);
    }

    public void OnClickedDisplaySettingButton()
    {
        displaySettingPanel.SetActive(true);
        gameSettingsPanel.SetActive(false);
        audioSettingsPanel.SetActive(false);
        graphicsSettingsPanel.SetActive(false);
    }
    public void OnClickedGameSettingButton()
    {
        displaySettingPanel.SetActive(false);
        gameSettingsPanel.SetActive(true);
        audioSettingsPanel.SetActive(false);
        graphicsSettingsPanel.SetActive(false);
    }
    public void OnClickedAudioSettingButton()
    {
        displaySettingPanel.SetActive(false);
        gameSettingsPanel.SetActive(false);
        audioSettingsPanel.SetActive(true);
        graphicsSettingsPanel.SetActive(false);
    }
    public void OnClickedGraphicsSettingButton()
    {
        displaySettingPanel.SetActive(false);
        gameSettingsPanel.SetActive(false);
        audioSettingsPanel.SetActive(false);
        graphicsSettingsPanel.SetActive(true);
    }
    
    
    
    
    
    public void OnClickedExitButton()
    {
        Application.Quit();
    }
}
