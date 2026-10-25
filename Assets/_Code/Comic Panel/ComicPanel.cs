#if UNITY_EDITOR
using UnityEngine;

[ExecuteInEditMode]
public class ComicPanel : MonoBehaviour
{
    #region Variables
    private RectTransform panel;
    ComicPagePanel comicPage;
    #endregion

    #region Built-In Methods
    private void Start()
    {
        if (Application.isPlaying) return;

        comicPage = GetComponentInParent<ComicPagePanel>();
        panel = GetComponent<RectTransform>();

        PanelProperties panelToAdd = new PanelProperties();
        panelToAdd.panel = panel;

        foreach (PanelProperties panelR in comicPage.panelProperties)
        {
            if (panelR.panel == panel)
            {
                return;
            }
        }

        if (panelToAdd.panel != null && !comicPage.panelProperties.Contains(panelToAdd))
            comicPage.panelProperties.Add(panelToAdd);
    }

    private void OnDestroy()
    {
        if (Application.isPlaying) return;

        PanelProperties panelToRemove = new PanelProperties();

        foreach (PanelProperties panelProp in comicPage.panelProperties)
        {
            if (panel == panelProp.panel)
            {
                panelToRemove = panelProp;
            }
        }

        if (panelToRemove.panel != null)
            comicPage.panelProperties.Remove(panelToRemove);
    } 
    #endregion
}
#endif