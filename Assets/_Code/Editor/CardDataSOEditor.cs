using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CardDataSO))]
public class CardDataSOEditor : Editor
{
    SerializedProperty cardType;
    SerializedProperty isBuffDebuff;

    private void OnEnable()
    {
        cardType = serializedObject.FindProperty("cardType");
        isBuffDebuff = serializedObject.FindProperty("isBuffDebuff");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        CardDataSO cardData = (CardDataSO)target;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("cardPriority"));
        EditorGUILayout.PropertyField(cardType);

        EditorGUI.indentLevel++;
        EditorGUILayout.BeginVertical("box");
        ShowDifferentProperties();
        EditorGUILayout.EndVertical();
        EditorGUI.indentLevel--;

        EditorGUILayout.PropertyField(isBuffDebuff);
        if(!isBuffDebuff.boolValue)
        {
            GUI.enabled = false;
        }
        EditorGUILayout.PropertyField(serializedObject.FindProperty("time"));
        GUI.enabled = true;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("levelImages"));

        serializedObject.ApplyModifiedProperties();
    }

    private void ShowDifferentProperties()
    {
        switch ((ECardType)cardType.enumValueIndex)
        {
            case ECardType.AffectsPlayer:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("affectedPlayerStat"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("playerStatModifier"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("item"));
                break;
            case ECardType.AddsWeapon:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("weaponToAdd"));
                break;
            case ECardType.AffectsEnemy:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("affectedEnemyStat"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("enemyStatModifier"));
                break;
            case ECardType.AffectsWeaponLevel:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("weaponName"));
                break;
            case ECardType.AffectsSpecificWeaponStat:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("weaponName"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("affectedWeaponStat"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("weaponStatModifier"));
                break;
            case ECardType.AffectsAllWeaponsStat:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("affectedWeaponStat"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("weaponStatModifier"));
                break;
        }
    }
}
