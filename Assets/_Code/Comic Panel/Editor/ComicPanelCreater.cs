using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Xplosiveginger.Tools
{
    public static class ComicPanelCreater
    {
        [MenuItem("GameObject/UI/Add Comic Panel", false)]
        public static void CreateComicPanel(MenuCommand menuCommand)
        {
            GameObject mainPanel = menuCommand.context as GameObject;
            GameObject panel = new GameObject("New Comic Panel", typeof(RectTransform), typeof(Image), typeof(ComicPanel));

            GameObjectUtility.SetParentAndAlign(panel, mainPanel);

            ComicPagePanel comicPage = mainPanel.GetComponent<ComicPagePanel>();

            if (comicPage)
            {
                PanelProperties newPanel = new PanelProperties();
                newPanel.panel = panel.GetComponent<RectTransform>();


                comicPage.panelProperties.Add(newPanel);
            }

            Undo.RegisterCreatedObjectUndo(panel, "Add New Comic Panel");
            Selection.activeObject = panel;
        }

        [MenuItem("GameObject/UI/Add Comic Panel", true)]
        static bool ValidateCreateComicPanel()
        {
            if (Selection.activeGameObject == null) return false;

            GameObject go = Selection.activeGameObject;
            ComicPagePanel comicPage = go.GetComponent<ComicPagePanel>();

            return comicPage != null;
        }

        [MenuItem("GameObject/UI/Add Comic Page")]
        private static void CreateComicPage(MenuCommand menuCommand)
        {
            GameObject parent = menuCommand.context as GameObject;

            Canvas canvas = parent != null
                ? parent.GetComponentInParent<Canvas>()
                : Object.FindFirstObjectByType<Canvas>();

            if (canvas == null)
            {
                GameObject canvasGO = new GameObject(
                    "Canvas",
                    typeof(RectTransform),
                    typeof(Canvas),
                    typeof(CanvasScaler),
                    typeof(GraphicRaycaster)
                );

                canvas = canvasGO.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;

                Undo.RegisterCreatedObjectUndo(canvasGO, "Create Canvas");
            }

            GameObject panelGO = new GameObject(
                "Comic Page",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image),
                typeof(ComicPagePanel)
            );

            EventSystem eventSys = Object.FindFirstObjectByType<EventSystem>();

            if(eventSys == null)
            {
                GameObject eventGo = new GameObject(
                    "Event System",
                    typeof(EventSystem),
                    typeof(StandaloneInputModule)
                );

                Undo.RegisterCreatedObjectUndo(eventGo, "Create Event System");
            }
            

            Undo.RegisterCreatedObjectUndo(panelGO, "Create Comic Page");

            RectTransform rect = panelGO.GetComponent<RectTransform>();

            rect.SetParent(canvas.transform, false);

            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.anchoredPosition = Vector2.zero;

            Selection.activeGameObject = panelGO;
        }
    } 
}