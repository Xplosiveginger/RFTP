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

    // Player
    private SerializedProperty affectedPlayerStat;
    private SerializedProperty playerStatModifier;
    private SerializedProperty playerStatIsPercentage;
    private SerializedProperty itemSO;
    private SerializedProperty item;

    // Enemy
    private SerializedProperty affectedEnemyStat;
    private SerializedProperty enemyStatModifier;

    // Weapon
    private SerializedProperty weaponName;
    private SerializedProperty weaponToAdd;

    private SerializedProperty affectedWeaponStat;
    private SerializedProperty weaponStatModifier;
    private SerializedProperty weaponStatIsPercentage;

    private SerializedProperty time;
    
    private SerializedProperty extraHealth;
    private SerializedProperty extraMoney;


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


        // =====================================================
        // PLAYER
        // =====================================================

        affectedPlayerStat =
            serializedObject.FindProperty("affectedPlayerStat");

        playerStatModifier =
            serializedObject.FindProperty("playerStatModifier");

        playerStatIsPercentage =
            serializedObject.FindProperty("playerStatIsPercentage");

        itemSO =
            serializedObject.FindProperty("itemSO");

        item =
            serializedObject.FindProperty("item");


        // =====================================================
        // ENEMY
        // =====================================================

        affectedEnemyStat =
            serializedObject.FindProperty("affectedEnemyStat");

        enemyStatModifier =
            serializedObject.FindProperty("enemyStatModifier");


        // =====================================================
        // WEAPON
        // =====================================================

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


        // =====================================================
        // BUFF / DEBUFF
        // =====================================================

        isBuffDebuff =
            serializedObject.FindProperty("isBuffDebuff");

        time =
            serializedObject.FindProperty("time");

        extraHealth = serializedObject.FindProperty("healthToAdd");
        extraMoney = serializedObject.FindProperty("moneyToAdd");
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

                EditorGUILayout.PropertyField(
                    playerStatIsPercentage
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

            case ECardType.ExtraCard_Health:

                EditorGUILayout.PropertyField(
                    extraHealth
                );

                break;

            case ECardType.ExtraCard_Money:

                EditorGUILayout.PropertyField(
                    extraMoney
                );
                break;


        }
    }
}