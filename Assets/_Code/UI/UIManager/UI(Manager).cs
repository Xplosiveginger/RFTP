using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMng : MonoBehaviour
{
    public GameObject pausePanel;

    private bool isActive = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isActive = !isActive;
            pausePanel.SetActive(isActive);

            // pauses the game when the pause menu is acitve and resumes it when deactive 
            Time.timeScale = isActive ? 0f : 1f;
        }
    }
}