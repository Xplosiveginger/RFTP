using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CardDataSO))]
public class CardDataSOEditor : Editor
{
    private SerializedProperty cardType;
    private SerializedProperty isBuffDebuff;

    private SerializedProperty icon;
    private SerializedProperty cardName;
    private SerializedProperty description;
    private SerializedProperty cardPriority;
    private SerializedProperty levelImages;

    private SerializedProperty affectedPlayerStat;
    private SerializedProperty playerStatModifier;
    private SerializedProperty itemSO;
    private SerializedProperty item;

    private SerializedProperty affectedEnemyStat;
    private SerializedProperty enemyStatModifier;

    private SerializedProperty weaponName;
    private SerializedProperty weaponToAdd;

    private SerializedProperty affectedWeaponStat;
    private SerializedProperty weaponStatModifier;
    private SerializedProperty weaponStatIsPercentage;

    private SerializedProperty time;


    private void OnEnable()
    {
        icon = serializedObject.FindProperty("Icon");
        cardName = serializedObject.FindProperty("Name");
        description = serializedObject.FindProperty("Description");

        cardPriority =
            serializedObject.FindProperty("cardPriority");

        cardType =
            serializedObject.FindProperty("cardType");

        levelImages =
            serializedObject.FindProperty("levelImages");


        // Player
        affectedPlayerStat =
            serializedObject.FindProperty("affectedPlayerStat");

        playerStatModifier =
            serializedObject.FindProperty("playerStatModifier");

        itemSO =
            serializedObject.FindProperty("itemSO");

        item =
            serializedObject.FindProperty("item");


        // Enemy
        affectedEnemyStat =
            serializedObject.FindProperty("affectedEnemyStat");

        enemyStatModifier =
            serializedObject.FindProperty("enemyStatModifier");


        // Weapon
        weaponName =
            serializedObject.FindProperty("weaponName");

        weaponToAdd =
            serializedObject.FindProperty("weaponToAdd");

        affectedWeaponStat =
            serializedObject.FindProperty("affectedWeaponStat");

        weaponStatModifier =
            serializedObject.FindProperty("weaponStatModifier");

        weaponStatIsPercentage =
            serializedObject.FindProperty("weaponStatIsPercentage");


        // Buff / Debuff
        isBuffDebuff =
            serializedObject.FindProperty("isBuffDebuff");

        time =
            serializedObject.FindProperty("time");
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();


        // =====================================================
        // CARD INFO
        // =====================================================

        EditorGUILayout.LabelField(
            "Card Info",
            EditorStyles.boldLabel
        );

        EditorGUILayout.PropertyField(icon);

        EditorGUILayout.PropertyField(cardName);

        EditorGUILayout.PropertyField(description);


        EditorGUILayout.Space();


        // =====================================================
        // CARD TYPE
        // =====================================================

        EditorGUILayout.LabelField(
            "Card Settings",
            EditorStyles.boldLabel
        );

        EditorGUILayout.PropertyField(
            cardPriority
        );

        EditorGUILayout.PropertyField(
            cardType
        );


        EditorGUILayout.Space();


        // =====================================================
        // TYPE-SPECIFIC SETTINGS
        // =====================================================

        EditorGUILayout.LabelField(
            "Effect Settings",
            EditorStyles.boldLabel
        );

        EditorGUILayout.BeginVertical("box");

        ShowDifferentProperties();

        EditorGUILayout.EndVertical();


        EditorGUILayout.Space();


        // =====================================================
        // BUFF / DEBUFF
        // =====================================================

        EditorGUILayout.PropertyField(
            isBuffDebuff
        );


        EditorGUI.BeginDisabledGroup(
            !isBuffDebuff.boolValue
        );

        EditorGUILayout.PropertyField(
            time
        );

        EditorGUI.EndDisabledGroup();


        EditorGUILayout.Space();


        // =====================================================
        // LEVEL IMAGES
        // =====================================================

        EditorGUILayout.LabelField(
            "Level Images",
            EditorStyles.boldLabel
        );

        EditorGUILayout.PropertyField(
            levelImages
        );


        serializedObject.ApplyModifiedProperties();
    }


    private void ShowDifferentProperties()
    {
        ECardType type =
            (ECardType)cardType.enumValueIndex;


        switch (type)
        {
            // =================================================
            // PLAYER
            // =================================================

            case ECardType.AffectsPlayer:

                EditorGUILayout.PropertyField(
                    affectedPlayerStat
                );

                EditorGUILayout.PropertyField(
                    playerStatModifier
                );

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(
                    itemSO
                );

                EditorGUILayout.PropertyField(
                    item
                );

                break;


            // =================================================
            // ADD WEAPON
            // =================================================

            case ECardType.AddsWeapon:

                EditorGUILayout.PropertyField(
                    weaponToAdd
                );

                break;


            // =================================================
            // ENEMY
            // =================================================

            case ECardType.AffectsEnemy:

                EditorGUILayout.PropertyField(
                    affectedEnemyStat
                );

                EditorGUILayout.PropertyField(
                    enemyStatModifier
                );

                break;


            // =================================================
            // WEAPON LEVEL
            // =================================================

            case ECardType.AffectsWeaponLevel:

                EditorGUILayout.PropertyField(
                    weaponName
                );

                break;


            // =================================================
            // SPECIFIC WEAPON STAT
            // =================================================

            case ECardType.AffectsSpecificWeaponStat:

                EditorGUILayout.PropertyField(
                    weaponName
                );

                EditorGUILayout.PropertyField(
                    affectedWeaponStat
                );

                EditorGUILayout.PropertyField(
                    weaponStatModifier
                );

                EditorGUILayout.PropertyField(
                    weaponStatIsPercentage
                );

                break;


            // =================================================
            // ALL WEAPONS STAT
            // =================================================

            case ECardType.AffectsAllWeaponsStat:

                EditorGUILayout.PropertyField(
                    affectedWeaponStat
                );

                EditorGUILayout.PropertyField(
                    weaponStatModifier
                );

                EditorGUILayout.PropertyField(
                    weaponStatIsPercentage
                );

                break;
        }
    }
}