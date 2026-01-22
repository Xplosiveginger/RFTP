using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenuUI;
    public GameObject shopUI;

    public void OnPlayGameClicked()
    {
        mainMenuUI.SetActive(false);
        shopUI.SetActive(true);
    }
}
