using UnityEngine;

public class PanelButton : MonoBehaviour
{
    [Header("Panels")]
    public GameObject panelToDisable;
    public GameObject panelToEnable;

    // Called by Button OnClick
    public void SwitchPanels()
    {
        if (panelToDisable != null)
            panelToDisable.SetActive(false);

        if (panelToEnable != null)
            panelToEnable.SetActive(true);
    }
    public void onExitClicked()
    {
        Debug.Log("Game is quitting...");

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
