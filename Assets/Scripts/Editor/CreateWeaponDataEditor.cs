using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreateWeaponDataEditor : EditorWindow
{
    static string statfolderPath = "Assets/Scripts/ScriptableObject/StatSO/Player Stats/WeaponStats";
    static string weaponfolderPath = "Assets/Scripts/ScriptableObject/WeaponSO/Refactored";
    [SerializeField] public WeaponData weaponData = new WeaponData();
    SerializedObject serializedObjectRef;

    int selectedStatIndex = -1;

    private void OnEnable()
    {
        serializedObjectRef = new SerializedObject(this);
    }

    [MenuItem("RFTP/Tools/Create Weapon Data")]
    public static void CreateWeaponDataWindow()
    {
        CreateWeaponDataEditor window = GetWindow<CreateWeaponDataEditor>("Create Weapon");
        window.Show();
    }

    private void OnGUI()
    {
        serializedObjectRef.Update();

        GUILayout.Label("Weapon Data Creator", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(serializedObjectRef.FindProperty("weaponData.weaponName"));
        //EditorGUILayout.PropertyField(serializedObjectRef.FindProperty("weaponData.statList"));

        GUIStyle buttonStyle2 = GUI.skin.button; //EditorStyles.selectionRect;
        
        GUILayout.BeginVertical(EditorStyles.textArea);
        GUILayout.Space(5);

        int cols = 3;

        // DYNAMIC GRID - creates button for each stat in list
        for (int i = 0; i < weaponData.statList.Count; i += cols)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            for (int j = 0; j < cols && (i + j) < weaponData.statList.Count; j++)
            {
                int index = i + j;
                GUIStyle buttonStyle = (selectedStatIndex == index)
                                ? EditorStyles.selectionRect
                                : GUI.skin.button;

                GUILayout.Space(5);
                if (GUILayout.Button("Stat " + (index + 1),
                    buttonStyle,
                    GUILayout.Width(40), GUILayout.Height(40)))
                {
                    selectedStatIndex = (selectedStatIndex == index) ? -1 : index;
                }
            }
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
        }
        /*//GUILayout.BeginHorizontal();
        //GUILayout.Space(5);
        //if (GUILayout.Button("Stat", buttonStyle, GUILayout.Width(40), GUILayout.Height(40)))
        //{
        //    //ShowStatProperties();
        //}
        //if(GUILayout.Button("Stat", buttonStyle, GUILayout.Width(40), GUILayout.Height(40)))
        //{

        //}
        //GUILayout.Space(5);
        //GUILayout.EndHorizontal();*/

        GUILayout.Space(5);
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Add Stat"))
        {
            AddStat(weaponData.statList);
        }

        if(GUILayout.Button("Remove Stat"))
        {
            RemoveStat(weaponData.statList, selectedStatIndex);
        }
        GUILayout.EndHorizontal();
        
        if (selectedStatIndex >= 0 && weaponData.statList.Count > selectedStatIndex)
        {
            GUILayout.Space(5);
            Stat selectedStat = weaponData.statList[selectedStatIndex];

            ShowStatProperties(selectedStat);
        }

        EditorGUILayout.Space(50);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        if(GUILayout.Button("Create Weapon Data"))
        {
            CreateWeaponStatData(weaponData.weaponName);
            CreateWeaponData(weaponData.weaponName);
        }

        serializedObjectRef.ApplyModifiedProperties();
    }

    void ShowStatProperties(Stat stat)
    {
        stat.statName = (EStatType)EditorGUILayout.EnumPopup("Stat Name", stat.statName);
        stat.baseValue = EditorGUILayout.FloatField("Base Value", stat.baseValue);
        EditorUtility.SetDirty(stat);
    }

    static void CreateWeaponStatData(string weaponName)
    {
        AssetDatabase.CreateFolder(statfolderPath, $"{weaponName} Stat Data");
    }

    static void CreateWeaponData(string weaponName)
    {
        AssetDatabase.CreateFolder(weaponfolderPath, $"{weaponName} Data");
    }

    static void AddStat(List<Stat> statList)
    {
        Stat newStat = new Stat();
        statList.Add(newStat);
    }

    static void RemoveStat(List<Stat> statList, int index)
    {
        if (index < 0 || index > statList.Count) return;

        Stat stat = statList[index];
        statList.Remove(stat);
    }

    [System.Serializable]
    public class WeaponData
    {
        public string weaponName;
        public List<Stat> statList = new List<Stat>();
        public GameObject prefab;
    }
}