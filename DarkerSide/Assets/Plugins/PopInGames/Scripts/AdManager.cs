using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace PopInGames
{
    public class AdManager : MonoBehaviour
    {
        private const bool Debugging = false;

        [HideInInspector]
        public ProjectSettings projectSettings;

        [HideInInspector]
        public SceneSettings sceneSettings;

        private ApiClient client;
        private AnalyticsRecorder analyticsRecorder;
        private CameraTracker cameraTracker;

        private float callCameraTrackerTimer = 0;
        private float saveAnalyticsTimer = 0;
        private float refreshTimer = 0;

        class AdRequestData
        {
            public string game_id;
            public List<AreaCodeData> area_codes;
        }
        class AreaCodeData
        {
            public string code;
            public int quantity;
        }

        private void Start()
        {
            projectSettings = Resources.Load<ProjectSettings>("Settings");
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            sceneSettings = Resources.Load<SceneSettings>("AdSettings_"+ sceneName);
            this.client = new ApiClient(this.projectSettings, this.sceneSettings);
            this.RetrieveAndInjectAds(this.client);
            this.analyticsRecorder = new AnalyticsRecorder();
            this.cameraTracker = CameraTracker.CreateInstance(this.analyticsRecorder);
            StartCoroutine(client.GetIPAddress(null, null));
        }

        private Dictionary<string,List<AdMesh>> GetAdMeshesByAreaCode()
        {
            List<AdMesh> adMeshes = Utility.FindComponentsInScene<AdMesh>();
            Dictionary<string, List<AdMesh>> adMeshTypes = new Dictionary<string, List<AdMesh>>();
            for (int i = 0; i < adMeshes.Count; i++)
            {
                string key = Conversions.AdTypeToCode(adMeshes[i].Type);
                if (adMeshTypes.ContainsKey(key))
                {
                    adMeshTypes[key].Add(adMeshes[i]);
                }
                else
                {
                    adMeshTypes.Add(key, new List<AdMesh>() { adMeshes[i] });
                }
            }
            return adMeshTypes;
        }

        private string CreateJsonRequest(Dictionary<string, List<AdMesh>> adMeshTypes)
        {
            AdRequestData adRequestData = new AdRequestData();
            adRequestData.game_id = this.projectSettings.GameId;
            List<AreaCodeData> areaCodeDatas = new List<AreaCodeData>();
            foreach (var i in adMeshTypes)
            {
                areaCodeDatas.Add(new AreaCodeData() { code = i.Key, quantity = i.Value.Count });
            }
            adRequestData.area_codes = areaCodeDatas;
            return JsonConvert.SerializeObject(adRequestData);
        }

        private Dictionary<string, List<ApiClient.BannerResponse.Data>> GetBannerDataByAreaCode(List<ApiClient.BannerResponse.Data> data)
        {
            Dictionary<string, List<ApiClient.BannerResponse.Data>> adDataTypes = new Dictionary<string, List<ApiClient.BannerResponse.Data>>();
            for (int i = 0; i < data.Count; i++)
            {
                if (adDataTypes.ContainsKey(data[i].area_code))
                {
                    adDataTypes[data[i].area_code].Add(data[i]);
                }
                else
                {
                    adDataTypes.Add(data[i].area_code, new List<ApiClient.BannerResponse.Data>() { data[i] });
                }
            }
            return adDataTypes;
        }
        public void RetrieveAndInjectAds(ApiClient client)
        {
            try
            {
                this.StartCoroutine(client.RetrieveToken_Coroutine(token =>
                {
                    client.ApiToken = token;

                    Dictionary<string, List<AdMesh>> adMeshTypes = GetAdMeshesByAreaCode();
                    string jsonRequest = CreateJsonRequest(adMeshTypes);

                    this.StartCoroutine(client.RetrieveBanner_Coroutine(jsonRequest, bannerResponse =>
                    {
                        Dictionary<string, List<ApiClient.BannerResponse.Data>> adDataTypes = GetBannerDataByAreaCode(bannerResponse.data);

                        //Assign ads to meshes, Recycle ads if more meshes than ads;
                        foreach (var adMeshList in adMeshTypes)
                        {
                            int index = 0;
                            List<ApiClient.BannerResponse.Data> adDataList = null;
                            if (adDataTypes.TryGetValue(adMeshList.Key, out adDataList))
                            {
                                for (int i = 0; i < adMeshList.Value.Count; i++)
                                {
                                    var adData = adDataList[index];
                                    this.StartCoroutine(client.DownloadTexture_Coroutine(adData, adMeshList.Value[i], (texture, adMesh,adData) =>
                                    {
                                        float aspect = texture.height/texture.width;
                                        float scaleHeight = adMesh.transform.localScale.x * aspect;
                                        adMesh.BannerData = adData;
                                        adMesh.ApplyTexture(texture, "Ad " + Conversions.AdTypeToCode(adMesh.Type));
                                    }));

                                    index++;
                                    index = index % adDataList.Count;
                                }
                            }
                            else
                            {
                                Debug.LogError("No ads for ad type " + adMeshList.Key);
                            }
                        }
                        ResetRefreshTimer();
                    }, ResetRefreshTimer));
                }, ResetRefreshTimer));
            }
            catch (InvalidCredentialsException)
            {
                ResetRefreshTimer();
                Debug.LogError("The credentials seem to be incorrect. Please check the client ID and client secret in the project settings.");
            }
        }

        private void ResetRefreshTimer()
        {
            this.refreshTimer = 0;
        }

        private void Update()
        {
            this.callCameraTrackerTimer += Time.deltaTime;
            if (this.callCameraTrackerTimer > 0.25f)
            {
                var raycastMask = this.projectSettings.BlockingLayersConverted;
                this.cameraTracker.Update(this.callCameraTrackerTimer, raycastMask);

                if (Debugging)
                {
                    foreach (var adMesh in this.cameraTracker.AllAdMeshes)
                    {
                        if (this.cameraTracker.MostlySeenAds.Contains(adMesh))
                        {
                            adMesh.ApplyColor(Color.green, "CompletelySeen");
                        }
                        else if (this.cameraTracker.PartiallySeenAds.Contains(adMesh))
                        {
                            adMesh.ApplyColor(Color.yellow, "Seen");
                        }
                        else
                        {
                            adMesh.ApplyColor(Color.red, "NotSeen");
                        }
                    }
                }
                
                this.callCameraTrackerTimer = 0;
            }

            this.saveAnalyticsTimer += Time.deltaTime;
            if (this.saveAnalyticsTimer > 10 && !this.analyticsRecorder.IsEmpty)
            {
                this.StartCoroutine(this.analyticsRecorder.SaveAndReset_Coroutine(this.client));
                this.saveAnalyticsTimer = 0;
            }

            if (projectSettings.RefreshFlag)
            {
                this.refreshTimer += Time.deltaTime;
                if (this.refreshTimer >= projectSettings.RefreshTime)
                {
                    this.refreshTimer = -100000;
                    //The timer is reset when ads are loaded and applied.
                    //The upper value of -100000 is so that this function doesnt keep calling constantly while loading ads
                    RetrieveAndInjectAds(this.client);
                }
            }
        }

        private void OnGUI()
        {
            if (Debugging)
            {
                GUI.skin.label.fontSize = 40;
                GUI.skin.label.fontStyle = FontStyle.Bold;
                var rect = new Rect(20, 20, 400, 130);
                var text = "Partially seen: " + this.cameraTracker.PartiallySeenAds.Count + "\n" +
                           "Mostly seen: " + this.cameraTracker.MostlySeenAds.Count;
                this.DrawLabelWithOutline(rect, text, 2, Color.white, Color.black);
            }
        }

        private void DrawLabelWithOutline(Rect rect, string text, int strength, Color color, Color borderColor)
        {
            GUI.color = borderColor;
            int i;
            for (i = -strength; i <= strength; i++)
            {
                GUI.Label(new Rect(rect.x - strength, rect.y + i, rect.width, rect.height), text);
                GUI.Label(new Rect(rect.x + strength, rect.y + i, rect.width, rect.height), text);
            }
            for (i = -strength + 1; i <= strength - 1; i++)
            {
                GUI.Label(new Rect(rect.x + i, rect.y - strength, rect.width, rect.height), text);
                GUI.Label(new Rect(rect.x + i, rect.y + strength, rect.width, rect.height), text);
            }
            GUI.color = color;
            GUI.Label(new Rect(rect.x, rect.y, rect.width, rect.height), text);
        }
    }
}