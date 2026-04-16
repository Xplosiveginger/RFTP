using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string SceneName;
    [SerializeField] private GameObject Loading;
    public void SceneChange()
    {
        
        SceneManager.LoadScene(SceneName);
    }

    public void OnPlay()
    {
        Loading.gameObject.SetActive(true);
    }
    public void Exit()
    {
        Debug.Log("Exit");
        Application.Quit();
    }
    
}
