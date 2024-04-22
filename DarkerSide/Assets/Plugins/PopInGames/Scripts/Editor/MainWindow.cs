using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PopInGames.Editors
{
    public class MainWindow : EditorWindow
    {
        private const string windowTitle = "Advertising";

        private Rect headerSection;
        private Rect mainSection;

        private readonly List<AdMesh> foundAdMeshes = new List<AdMesh>();
        private double nextScanTime = 0;

        private Vector2 scrollPosition = Vector2.zero;

        [MenuItem("Window/Pop in Games/Advertising")]
        private static void OpenWindow()
        {
            var window = (MainWindow)GetWindow(typeof(MainWindow));
            window.titleContent.text = windowTitle;
            window.minSize = new Vector2(300, 300);
            window.Show();
            window.OnOpen();
        }

        private void OnOpen()
        {
            this.ScanAdMeshes();
        }

        private void OnEnable()
        {
            this.ScanAdMeshes();
        }

        private void Update()
        {
            if (EditorApplication.timeSinceStartup > this.nextScanTime)
            {
                this.nextScanTime = EditorApplication.timeSinceStartup + 1;

                this.ScanAdMeshes();
            }
        }

        void OnInspectorUpdate()
        {
            EditorUtility.SetDirty(Plugin.ActiveSceneSettings);
            Repaint();
        }

        private void OnGUI()
        {
            this.DrawLayouts();
        }

        private void DrawLayouts()
        {
            this.headerSection.x = 0;
            this.headerSection.y = 0;
            this.headerSection.width = this.position.width;
            this.headerSection.height = 50;

            this.mainSection.x = 15;
            this.mainSection.y = 50 + 15;
            this.mainSection.width = this.position.width - 30;
            this.mainSection.height = this.position.height - 50 - 30;

            this.DrawHeader();
            this.DrawMainSection();
        }

        private void DrawHeader()
        {
            var textures = Plugin.Textures;

            // Background
            if (textures.White != null)
            {
                GUI.DrawTexture(this.headerSection, textures.White);
            }

            // Logo
            GUI.DrawTexture(new Rect(this.headerSection.x, this.headerSection.y, this.headerSection.height * (textures.Logo.width * 1f / textures.Logo.height), this.headerSection.height), textures.Logo);

            // Settings button
            var settingsButtonRect = new Rect(this.headerSection.width - 10 - 120, 10, 120, 20);
            if (GUI.Button(settingsButtonRect, "Project Settings"))
            {
                SettingsWindow.OpenWindow();
            }
        }

        private void DrawMainSection()
        {
            GUILayout.BeginArea(this.mainSection);

            if (Plugin.ActiveSceneSettings != null)
            {
                EditorGUILayout.LabelField("Scene Settings", EditorStyles.boldLabel);
                bool wasEnabled = Plugin.ActiveSceneSettings.Enabled;
                Plugin.ActiveSceneSettings.Enabled = EditorGUILayout.Toggle("Show Ads", Plugin.ActiveSceneSettings.Enabled);
                if (!wasEnabled && Plugin.ActiveSceneSettings.Enabled)
                {
                    Plugin.UpdateAdInjector();
                }

                if (Plugin.ActiveSceneSettings.Enabled)
                {
                    Plugin.ActiveSceneSettings.LevelId = EditorGUILayout.IntField("Level ID", Plugin.ActiveSceneSettings.LevelId);
                    GUILayout.Space(15);

                    EditorGUILayout.LabelField("Ad Meshes", EditorStyles.boldLabel);
                    if (this.foundAdMeshes.Count > 0)
                    {
                        EditorGUILayout.BeginVertical();
                        EditorGUILayout.BeginScrollView(this.scrollPosition);
                        foreach (AdMesh adMesh in this.foundAdMeshes)
                        {
                            if (adMesh != null && adMesh.gameObject != null)
                            {
                                string label = adMesh.Type.ToString();
                                EditorGUILayout.ObjectField(label, adMesh.gameObject, typeof(AdMesh), true);
                            }
                        }
                        EditorGUILayout.EndScrollView();
                        EditorGUILayout.EndVertical();
                    }
                    else
                    {
                        EditorGUILayout.LabelField("No ad meshes in scene");
                    }
                    GUILayout.Space(15);

                    // TODO: if (GUILayout.Button("Show Available Tags"))

                    GUI.enabled = this.foundAdMeshes.Count > 0;
                    if (GUILayout.Button("Preview Advertisting"))
                    {
                        this.PreviewAdvertising(this.foundAdMeshes.ToArray());
                    }
                    GUI.enabled = true;
                }
            }
            else
            {
                GUILayout.Label("Please save the scene first.");
            }

            GUILayout.EndArea();
        }

        private void ScanAdMeshes()
        {
            this.foundAdMeshes.Clear();
            List<AdMesh> adMeshes = Utility.FindComponentsInScene<AdMesh>();
            foreach (AdMesh adMesh in adMeshes)
            {
                this.foundAdMeshes.Add(adMesh);
            }
        }

        private void PreviewAdvertising(AdMesh[] adMeshes)
        {
            try
            {
                Plugin.UpdateAdInjector();
                var adInjector = Plugin.GetOrCreateAdInjector();
                adInjector.RetrieveAndInjectAds(Plugin.Client);

            }
            catch (InvalidCredentialsException)
            {
                EditorUtility.DisplayDialog("Error", "The credentials seem to be incorrect. Please check the client ID and client secret in the project settings.", "OK");
            }
        }
    }
}