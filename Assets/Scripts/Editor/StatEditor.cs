using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Stat))]
//[CanEditMultipleObjects]
public class StatEditor : Editor
{
    SerializedProperty statName;
    SerializedProperty baseValue;
    SerializedProperty startValue;
    SerializedProperty startMultiplier;
    SerializedProperty maxValue;

    SerializedProperty customMaxValue;
    SerializedProperty showCurrentValues;

    SerializedProperty currentMultiplier;
    SerializedProperty currentValue;

    private void OnEnable()
    {
        statName = serializedObject.FindProperty("statName");
        baseValue = serializedObject.FindProperty("baseValue");
        startValue = serializedObject.FindProperty("startValue");
        startMultiplier = serializedObject.FindProperty("startMultiplier");
        maxValue = serializedObject.FindProperty("maxValue");
        customMaxValue = serializedObject.FindProperty("customMaxValue");
        showCurrentValues = serializedObject.FindProperty("showCurrentValues");
        currentMultiplier = serializedObject.FindProperty("currentMultiplier");
        currentValue = serializedObject.FindProperty("currentValue");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Stat stat = (Stat)target;

        EditorGUILayout.PropertyField(statName);
        EditorGUILayout.PropertyField(baseValue);
        EditorGUILayout.PropertyField(startValue);
        EditorGUILayout.PropertyField(startMultiplier);
        EditorGUILayout.Space(10);
        if (statName.enumValueIndex == (int)EStatType.Health)
        {
            EditorGUILayout.PropertyField(customMaxValue);
            if (!customMaxValue.boolValue)
            {
                GUI.enabled = false;
            }

            maxValue.floatValue = baseValue.floatValue;
            EditorGUILayout.PropertyField(maxValue);
            GUI.enabled = true;
        }

        EditorGUILayout.Space(20);
        EditorGUILayout.PropertyField(showCurrentValues);

        GUI.enabled = false;
        if (showCurrentValues.boolValue)
        {
            EditorGUILayout.PropertyField(currentValue);
            EditorGUILayout.PropertyField(currentMultiplier);
        }
        GUI.enabled = true;

        serializedObject.ApplyModifiedProperties();
    }
}
