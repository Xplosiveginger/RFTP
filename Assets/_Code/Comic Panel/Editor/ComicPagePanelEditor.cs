using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ComicPagePanel)), CanEditMultipleObjects]
public class ComicPagePanelEditor : Editor
{
    ComicPagePanel panel;

    SerializedObject so;
    SerializedProperty playOnAwake;
    SerializedProperty panelProps;
    SerializedProperty nextPage;
    SerializedProperty pageTransition;
    SerializedProperty delayBeforePageTransition;
    SerializedProperty buttonVisibilityDelay;
    SerializedProperty button;
    SerializedProperty customEvent;

    private void OnEnable()
    {
        panel = (ComicPagePanel)target;

        so = new SerializedObject(panel);

        playOnAwake = so.FindProperty("playOnAwake");
        panelProps = so.FindProperty("panelProperties");
        nextPage = so.FindProperty("nextPage");
        pageTransition = so.FindProperty("pageTransition");
        delayBeforePageTransition = so.FindProperty("delayBeforePageTransition");
        buttonVisibilityDelay = so.FindProperty("buttonVisibilityDelay");
        button = so.FindProperty("button");
        customEvent = so.FindProperty("OnAnimationCompleted");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(playOnAwake);
        EditorGUILayout.PropertyField(panelProps);
        EditorGUILayout.PropertyField(nextPage);

        if(nextPage.objectReferenceValue != null)
        {
            EditorGUILayout.PropertyField(pageTransition);

            if (pageTransition.enumValueIndex == 1)
            {
                EditorGUILayout.PropertyField(delayBeforePageTransition);
            }
            else if (pageTransition.enumValueIndex == 3)
            {
                EditorGUILayout.PropertyField(button);
                EditorGUILayout.PropertyField(buttonVisibilityDelay);
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(customEvent);

        so.ApplyModifiedProperties();
    }
}