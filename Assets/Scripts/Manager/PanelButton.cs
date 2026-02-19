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
}
