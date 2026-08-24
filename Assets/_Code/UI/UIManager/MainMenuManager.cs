using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager instance;

    [Header("Panels")]
    public GameObject startPanel;
    public GameObject settingsPanel;
    public GameObject shopPanel;

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
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        startPanel.SetActive(true);
        settingsPanel.SetActive(false);
        shopPanel.SetActive(false);

        shopBuyButton.onClick.AddListener(OnClickedShopBuyButton);
        shopRefundButton.onClick.AddListener(OnClickedShopRefundButton);
    }

    public void OnClickPlaySound()
    {
        PlaySound(buttonClick);
    }

    public void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    // -------------------------
    // SETTINGS
    // -------------------------

    public void OnClickedSettingButton()
    {
        startPanel.SetActive(false);
        settingsPanel.SetActive(true);

        PlaySound(buttonClick);
    }

    public void OnClickedSettingExitButton()
    {
        settingsPanel.SetActive(false);
        startPanel.SetActive(true);

        PlaySound(buttonClick);
    }

    // -------------------------
    // SHOP
    // -------------------------

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
        shopPanel.SetActive(false);
        startPanel.SetActive(true);

        PlaySound(buttonClick);
    }

    // -------------------------
    // EXIT GAME
    // -------------------------

    public void OnClickedExitButton()
    {
        Application.Quit();
    }
}