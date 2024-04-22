using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PopInGames.Editors
{
    public class SettingsWindow : EditorWindow
    {
        private const string windowTitle = "Advertising Settings";
        
        private ProjectSettings settings;

        public static void OpenWindow()
        {
            var window = (SettingsWindow) GetWindow(typeof(SettingsWindow));
            window.titleContent.text = windowTitle;
            window.minSize = new Vector2(430, 195);
            window.Show();
            window.OnOpen();
        }

        private void OnOpen()
        {
            this.settings = Plugin.ProjectSettings;
        }

        void OnInspectorUpdate()
        {
            EditorUtility.SetDirty(Plugin.ProjectSettings);
        }

        private void OnGUI()
        {
            this.DrawLayouts();
        }

        private void DrawLayouts()
        {
            GUILayout.BeginArea(new Rect(15, 15, this.position.width - 30, this.position.height - 30));
            
            //EditorGUILayout.LabelField("Backend URL", EditorStyles.boldLabel);
            //this.settings.ApiUrl = EditorGUILayout.TextField(this.settings.ApiUrl);

            //EditorGUILayout.Space(10);
            
            EditorGUILayout.LabelField("Credentials", EditorStyles.boldLabel);
            
            this.settings.ClientId = EditorGUILayout.TextField("Client ID", this.settings.ClientId);
            this.settings.ClientSecret = EditorGUILayout.TextField("Client Secret", this.settings.ClientSecret);
            
            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Project", EditorStyles.boldLabel);
            this.settings.GameId = EditorGUILayout.TextField("Game ID", this.settings.GameId);

            this.settings.RefreshFlag = EditorGUILayout.Toggle("Refresh", this.settings.RefreshFlag);
            if (this.settings.RefreshFlag)
            {
                EditorGUI.indentLevel++;
                this.settings.RefreshTime =Mathf.Clamp(EditorGUILayout.FloatField("Refresh Time", this.settings.RefreshTime),10,60);
                EditorGUI.indentLevel--;
            }

            var layerNames = new List<string>();
            for (int layer = 0; layer < 32; layer++)
            {
                string layerName = LayerMask.LayerToName(layer);
                if (layerName.Length > 0 && layerName != "AdMesh" && layerName != "AdChecker")
                {
                    layerNames.Add(layerName);
                }
            }
            this.settings.BlockingLayers = EditorGUILayout.MaskField("Blocking Layers", this.settings.BlockingLayers, layerNames.ToArray());

            GUILayout.EndArea();
        }
    }
}