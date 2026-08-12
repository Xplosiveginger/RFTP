using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager instance;
    public GameObject startPanel;
    public GameObject settingsPanel;
    public GameObject shopPanel;
    
    [Header("Setting Sub Menus")] 
    public GameObject displaySettingPanel;
    public GameObject gameSettingsPanel;
    public GameObject audioSettingsPanel;
    public GameObject graphicsSettingsPanel;

    [Header("Shop")] 
    public Button shopBuyButton;
    public Button shopRefundButton;
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip buttonClick;
    public AudioClip buyButtonClick;
    public AudioClip refundButtonClick;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        startPanel.SetActive(true);
        shopPanel.SetActive(false);
        settingsPanel.SetActive(false);
        
        shopBuyButton.onClick.AddListener(OnClickedShopBuyButton);
        shopRefundButton.onClick.AddListener(OnClickedShopRefundButton);
    }

    public void OnClickPlaySound()
    {
        audioSource.PlayOneShot(buttonClick);
    }
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    public void OnClickedSettingButton()
    {
        startPanel.SetActive(false);
        settingsPanel.SetActive(true);
        PlaySound(buttonClick);
    }

    public void OnClickedShopButton()
    {
        startPanel.SetActive(false);
        shopPanel.SetActive(true);
        PlaySound(buttonClick);
    }
    public void OnClickedShopBuyButton()
    {
        PlaySound(buyButtonClick);
    }
    public void OnClickedShopRefundButton()
    {
        PlaySound(refundButtonClick);
    }

    public void OnClickedShopCloseButton()
    {
        startPanel.SetActive(true);
        shopPanel.SetActive(false);
        PlaySound(buttonClick);
    }
    public void OnClickedSettingExitButton()
    {
        settingsPanel.SetActive(false);
        startPanel.SetActive(true);
        PlaySound(buttonClick);
    }

    public void OnClickedDisplaySettingButton()
    {
        displaySettingPanel.SetActive(true);
        gameSettingsPanel.SetActive(false);
        audioSettingsPanel.SetActive(false);
        graphicsSettingsPanel.SetActive(false);
        PlaySound(buttonClick);

    }
    public void OnClickedGameSettingButton()
    {
        displaySettingPanel.SetActive(false);
        gameSettingsPanel.SetActive(true);
        audioSettingsPanel.SetActive(false);
        graphicsSettingsPanel.SetActive(false);
        PlaySound(buttonClick);

    }
    public void OnClickedAudioSettingButton()
    {
        displaySettingPanel.SetActive(false);
        gameSettingsPanel.SetActive(false);
        audioSettingsPanel.SetActive(true);
        graphicsSettingsPanel.SetActive(false);
        PlaySound(buttonClick);

    }
    public void OnClickedGraphicsSettingButton()
    {
        displaySettingPanel.SetActive(false);
        gameSettingsPanel.SetActive(false);
        audioSettingsPanel.SetActive(false);
        graphicsSettingsPanel.SetActive(true);
        PlaySound(buttonClick);

    }
    
    
    
    
    
    public void OnClickedExitButton()
    {
        Application.Quit();
    }
}
