using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PopInGames.Editors
{
    [InitializeOnLoad]
    public static class Plugin
    {
        private static ProjectSettings _projectSettings;

        private static Dictionary<Scene, SceneSettings> sceneSettings = new Dictionary<Scene, SceneSettings>();

        public static string PluginPath
        {
            get
            {
                var scriptPath = AssetDatabase.FindAssets("t:script").Select(AssetDatabase.GUIDToAssetPath).FirstOrDefault(p => p.EndsWith("PopInGames/Scripts/AdManager.cs"));
                if (scriptPath != null)
                {
                    return scriptPath.Substring(0, scriptPath.Length - "/Scripts/AdManager.cs".Length);
                }
                return "Assets/Plugins/PopInGames";
            }
        }

        public static ApiClient Client { get; } = new ApiClient(ProjectSettings, ActiveSceneSettings);

        public static Textures Textures { get; } = new Textures();

        public static ProjectSettings ProjectSettings
        {
            get
            {
                if (_projectSettings == null)
                {
                    _projectSettings = LoadOrCreateProjectSettings();
                }
                return _projectSettings;
            }
            set => _projectSettings = value;
        }

        public static SceneSettings ActiveSceneSettings
        {
            get
            {
                var activeScene = EditorSceneManager.GetActiveScene();
                if (!sceneSettings.ContainsKey(activeScene))
                {
                    sceneSettings[activeScene] = LoadOrCreateSceneSettings(activeScene);
                }
                return sceneSettings[activeScene];
            }
        }

        static Plugin()
        {
            OnInit();
        }

        static void OnInit()
        {
            // Load settings
            ProjectSettings = LoadOrCreateProjectSettings();

            // Load textures
            LoadTextures();

            // Register events
            EditorSceneManager.sceneOpened += OnSceneOpened;
            EditorSceneManager.sceneSaved += OnSceneSaved;
        }
        
        static void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            sceneSettings[scene] = LoadOrCreateSceneSettings(scene);
            LoadTextures();
        }

        static void OnSceneSaved(Scene scene)
        {
            UpdateAdInjector();
        }

        public static void UpdateAdInjector()
        {
            return;
            if (!ActiveSceneSettings.Enabled)
            {
                return;
            }
            
            var adInjector = GetOrCreateAdInjector();
            adInjector.projectSettings = ProjectSettings;
            adInjector.sceneSettings = ActiveSceneSettings;
        }

        static void LoadTextures()
        {
            Textures.Load(PluginPath);
        }

        /*public void ShowAvailableTags()
        {
            this.StartCoroutine(this.client.RetrieveToken(token =>
            {
                this.client.apiToken = token;

                this.StartCoroutine(this.client.RetrieveBanner(0, 0, bannerResponse => { Debug.Log("Available ad tags: " + String.Join(", ", bannerResponse.data.Select(bannerData => bannerData.name))); }));
            }));
        }*/

        public static ProjectSettings LoadOrCreateProjectSettings()
        {
            var settingsPath = PluginPath + "/Resources/Settings.asset";
            if (File.Exists(settingsPath))
            {
                var loadedSettings = AssetDatabase.LoadAssetAtPath<ProjectSettings>(settingsPath);
                if (loadedSettings != null)
                {
                    return loadedSettings;
                }
            }

            var newSettings = ScriptableObject.CreateInstance<ProjectSettings>();
            AssetDatabase.CreateAsset(newSettings, settingsPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return newSettings;
        }

        public static SceneSettings LoadOrCreateSceneSettings(Scene scene)
        {
            if (scene.path.Length == 0)
            {
                return null;
            }

            var activeSceneFolder = Path.GetDirectoryName(scene.path) + "/" + scene.name;
            var activeSceneSettingsPath = PluginPath + "/Resources/AdSettings_"+scene.name+".asset";

            if (File.Exists(activeSceneSettingsPath))
            {
                var loadingSceneSettings = AssetDatabase.LoadAssetAtPath(activeSceneSettingsPath, typeof(SceneSettings));
                if (loadingSceneSettings != null && loadingSceneSettings is SceneSettings)
                {
                    return loadingSceneSettings as SceneSettings;
                }
                else
                {
                    Debug.LogError("Error loading advertising settings for scene " + scene.name);
                }
            }

            if (!Directory.Exists(activeSceneFolder))
            {
                AssetDatabase.CreateFolder(Path.GetDirectoryName(scene.path), scene.name);
            }
            var newSceneSettings = ScriptableObject.CreateInstance<SceneSettings>();
            AssetDatabase.CreateAsset(newSceneSettings, activeSceneSettingsPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return newSceneSettings;
        }

        public static AdManager GetOrCreateAdInjector()
        {
            var adManager = Object.FindObjectOfType<AdManager>();

            if (adManager == null)
            {
                var adManagerObj = new GameObject("Ad Manager");
                adManager = adManagerObj.AddComponent<AdManager>();
            }

            return adManager;
        }
    }
}