using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

namespace KrisDevelopment.KrisFavoriteAssets
{
    public class FavoriteAssets : EditorWindow
    {
        [System.Serializable]
        public class DataWrapper
        {
            public List<AssetData> assets = new List<AssetData>();
        }

        [System.Serializable]
        public class AssetData
        {
            public string guid;
            public string path;
            public string name;
            public string type;
        }

        /// <summary>
        /// Hook for the pro version.
        /// </summary>
        public interface IDataDrawHook
        {
            public void DrawToolbar(FavoriteAssets window);
            public void DrawData(FavoriteAssets window);
        }

        private static string GetPrefix() { return Application.productName + "_KFA_"; }
        
		[SerializeField]
        DataWrapper _assetsData = null;
        public DataWrapper assetsData
        {
            get
            {
                if(_assetsData == null){
                    LoadData();
                }
                
                return _assetsData;
            }
        }

		private Vector2 scrollView = Vector2.zero;

        private List<IDataDrawHook> dataDrawHooks = null;


        [MenuItem("Window/Kris Development/Favorite Assets")]
        public static void ShowWindow ()
        {
            GetWindow<FavoriteAssets>("★ Fav. Assets");
        }

        void CheckInit ()
        {
            // Initialize the data draw hooks list if it's null. This allows the pro version to inject its own hooks without referencing the pro assembly directly.
            if(dataDrawHooks == null){
                dataDrawHooks = new List<IDataDrawHook>();
                // use the activator to spawn instances of the IDataDrawHook interface, so we don't need to reference the pro assembly directly
                foreach(var type in System.AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(IDataDrawHook).IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract)){
                    IDataDrawHook instance = (IDataDrawHook)System.Activator.CreateInstance(type);
                    dataDrawHooks.Add(instance);  
                }
            }
        }

        public void OnGUI () 
        {
            CheckInit();

            GUILayout.BeginVertical(EditorStyles.helpBox);
            if(GUILayout.Button("Pin Selected Assets", EditorStyles.miniButton)){
                foreach(string assetGUID in Selection.assetGUIDs){
                    AssetData assetData = new AssetData();
                    assetData.guid  = assetGUID;
                    assetData.path = AssetDatabase.GUIDToAssetPath(assetGUID);
                    Object asset = AssetDatabase.LoadAssetAtPath<Object>(assetData.path);
                    assetData.name = asset.name;
                    assetData.type = asset.GetType().ToString();
                    _assetsData.assets.Add(assetData);
                }
                SaveData();
            }
            GUILayout.EndVertical();

            if(dataDrawHooks.Count > 0){

                foreach(IDataDrawHook hook in dataDrawHooks){
                    GUILayout.BeginHorizontal(EditorStyles.toolbar);
                    {
                        hook.DrawToolbar(this);
                    }
                    GUILayout.EndHorizontal();
                    
                }
            }
            else
            {
                
                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                {
                    GUILayout.Label("Pinned Assets:");
                    if(GUILayout.Button("▼ Sort Assets", EditorStyles.toolbarButton)){
                        _assetsData.assets.Sort(AssetDataComparer);
                    }

                }
                GUILayout.EndHorizontal();

            }

            scrollView = GUILayout.BeginScrollView(scrollView);
            if(dataDrawHooks.Count == 0)
            {       
                foreach(AssetData assetData in assetsData.assets) {
                    GUILayout.BeginHorizontal();

                    if(GUILayout.Button(new GUIContent("Open", "Open file with default app"), GUILayout.ExpandWidth(false))){
                        if(!Path.GetExtension(assetData.path).Equals(".unity")){
                            EditorUtility.OpenWithDefaultApp(assetData.path);
                        }else{
                            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(assetData.path, UnityEditor.SceneManagement.OpenSceneMode.Single);
                        }
                    }

                    if(GUILayout.Button(new GUIContent("Ping", "Highlight asset on Project panel"), GUILayout.ExpandWidth(false))){
                        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(assetData.path));
                    }

                    if(GUILayout.Button(new GUIContent(" " + assetData.name, AssetDatabase.GetCachedIcon(assetData.path)), GUILayout.Height(18))){
                        var asset = AssetDatabase.LoadAssetAtPath<Object>(assetData.path);
                        EditorGUIUtility.PingObject(asset);
                        Selection.activeObject = asset;
                    }

                    if(GUILayout.Button(new GUIContent("X", "Un-pin"), GUILayout.ExpandWidth(false))){
                        RemovePin(assetData);
                        break;
                    }

                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                dataDrawHooks.ForEach(hook => {
                    hook.DrawData(this);
                });
            }
            GUILayout.EndScrollView();

            // If there are no data draw hooks, show the try pro button at the bottom of the window.
            if(dataDrawHooks.Count == 0 && GUILayout.Button("Try PRO!", EditorStyles.linkLabel)){
                Application.OpenURL("https://assetstore.unity.com/packages/tools/utilities/favorite-assets-pro-376086");
            }
        }

        public void SaveData ()
        {
            string key = GetPrefix() + "pinned";
            string json = JsonUtility.ToJson(assetsData);
            EditorPrefs.SetString(key, json);
        }

        private void LoadData ()
        {
            _assetsData = new DataWrapper();

            string key = GetPrefix() + "pinned";
            if(EditorPrefs.HasKey(key)){
                string json = EditorPrefs.GetString(key);
                _assetsData = JsonUtility.FromJson<DataWrapper>(json);
            }
        }

        public void RemovePin (AssetData assetData)
        {
            _assetsData.assets.Remove(assetData);
            SaveData();
        }

        public int AssetDataComparer (AssetData left, AssetData right)
        {
            return left.type.CompareTo(right.type);
        }
    }
}
