using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace PopInGames
{
    public class ApiClient
    {
        private ProjectSettings projectSettings;
        private SceneSettings sceneSettings;

        private string apiToken;
        private Dictionary<string, DownloadTextureData> _textureCache;
        public string IpAddress;

        class DownloadTextureData
        {
            public Texture2D Texture;
            public bool IsDone;
            public bool HasError;
        }

        public ApiClient(ProjectSettings projectSettings, SceneSettings sceneSettings)
        {
            this.projectSettings = projectSettings;
            this.sceneSettings = sceneSettings;
        }

        public string ApiToken
        {
            get => this.apiToken;
            set => this.apiToken = value;
        }

        public IEnumerator RetrieveToken_Coroutine(Action<string> callback,Action onError=null)
        {
            if (this.projectSettings.ApiUrl.Length == 0)
            {
                Debug.LogError("There is no API url set. Please set it in Window -> Pop in Games -> Advertising -> Project Settings");
                onError?.Invoke();
                yield break;
            }

            if (this.projectSettings.ClientId.Length == 0)
            {
                Debug.LogError("There is no client ID set. Please set it in Window -> Pop in Games -> Advertising -> Project Settings");
                onError?.Invoke();
                yield break;
            }

            if (this.projectSettings.ClientSecret.Length == 0)
            {
                Debug.LogError("There is no client secret set. Please set it in Window -> Pop in Games -> Advertising -> Project Settings");
                onError?.Invoke();
                yield break;
            }

            using (var request = new UnityWebRequest(this.projectSettings.ApiUrl + "/token", "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(new TokenRequest(this.projectSettings.ClientId, this.projectSettings.ClientSecret)));
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SendWebRequest();

                while (!request.isDone)
                {
                    yield return null;
                }
                if (!string.IsNullOrEmpty(request.error))
                {
                    Debug.LogError("Network error: " + request.error);
                    onError?.Invoke();
                }
                else
                {
                    byte[] result = request.downloadHandler.data;
                    string responseJson = Encoding.Default.GetString(result);
                    TokenResponse info = JsonUtility.FromJson<TokenResponse>(responseJson);
                    Debug.Log(info.status_message);
                    if (info != null)
                    {
                        string token = info.data.token;
                        callback(token);
                    }
                    else
                    {
                        onError?.Invoke();
                        throw new InvalidCredentialsException();
                    }
                }
            }
        }

        public IEnumerator RetrieveBanner_Coroutine(string jsonRequest, Action<BannerResponse> callback,Action onError=null)
        {
            if (this.ApiToken == null)
            {
                onError?.Invoke();
                yield break;
            }

            if (this.projectSettings.GameId.Length == 0)
            {
                Debug.LogError("There is no game ID set. Please set it in Window -> Pop in Games -> Advertising -> Project Settings");
                onError?.Invoke();
                yield break;
            }
            
            UnityWebRequest www = UnityWebRequest.Get(this.projectSettings.ApiUrl+"/getbanner");
            www.SetRequestHeader("Postman-Token", "410feb74-d07f-4c1f-9fc5-34f965575e20");
            www.SetRequestHeader("cache-control", "no-cache");
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Accept", "application/json");
            www.SetRequestHeader("Authorization", "Bearer " + this.ApiToken);
            www.SetRequestHeader("Content-Type", "application/json");
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonRequest));
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();
            while (!www.isDone)
            {
                yield return null;
            }
            if (!string.IsNullOrEmpty(www.error))
            {
                onError?.Invoke();
                Debug.LogError("Network error: " + www.error);
            }
            else
            {
                byte[] result = www.downloadHandler.data;
                string responseJSON = Encoding.Default.GetString(result);

                BannerResponse bannerResponse = JsonUtility.FromJson<BannerResponse>(responseJSON);
                if (_textureCache != null)
                {
                    _textureCache.Clear();
                }
                callback(bannerResponse);
            }
        }

        public IEnumerator DownloadTexture_Coroutine(BannerResponse.Data bannerData,
            AdMesh adMesh, Action<Texture2D,AdMesh, BannerResponse.Data> callback)
        {
            if (_textureCache == null)
            {
                _textureCache = new Dictionary<string, DownloadTextureData>();
            }
            DownloadTextureData textureData;
            if (_textureCache.TryGetValue(bannerData.banner_url, out textureData))
            {
                while (!textureData.IsDone)
                {
                    yield return null;
                }
                if (!textureData.HasError)
                {
                    callback(textureData.Texture, adMesh,bannerData);
                }
            }
            else
            {
                using (var request = UnityWebRequestTexture.GetTexture(bannerData.banner_url))
                {
                    _textureCache.Add(bannerData.banner_url, new DownloadTextureData()
                    {
                        Texture = null,
                        IsDone = false,
                        HasError=false
                    });
                    request.SendWebRequest();
                    while (!request.isDone)
                    {
                        yield return null;
                    }

                    // Get downloaded asset bundle
                    var texture = DownloadHandlerTexture.GetContent(request);
                    _textureCache[bannerData.banner_url].Texture = texture;
                    _textureCache[bannerData.banner_url].IsDone = true;
                    _textureCache[bannerData.banner_url].HasError = request.result==UnityWebRequest.Result.ProtocolError||!string.IsNullOrEmpty(request.error);
                    callback(texture, adMesh, bannerData);
                }
            }
        }


        public IEnumerator GetIPAddress(Action<string> onFinish,Action onError)
        {
            if (!string.IsNullOrEmpty(IpAddress))
            {
                Debug.Log(IpAddress);
                yield break;
            }
            else
            {
                IpAddress = "";
            }
            UnityWebRequest www = UnityWebRequest.Get("http://checkip.dyndns.org");
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                onError?.Invoke();
            }
            else
            {
                string result = www.downloadHandler.text;

                // This results in a string similar to this: <html><head><title>Current IP Check</title></head><body>Current IP Address: 123.123.123.123</body></html>
                // where 123.123.123.123 is your external IP Address.
                //  Debug.Log("" + result);

                string[] a = result.Split(':'); // Split into two substrings -> one before : and one after. 
                string a2 = a[1].Substring(1);  // Get the substring after the :
                string[] a3 = a2.Split('<');    // Now split to the first HTML tag after the IP address.
                string a4 = a3[0];              // Get the substring before the tag.

                IpAddress = a4;
                Debug.Log(IpAddress);
                onFinish?.Invoke(a4);
            }
        }

        public string GetLocalIPAddress()
        {
            //var host = Dns.GetHostEntry(Dns.GetHostName());
            //foreach (var ip in host.AddressList)
            //{
            //    if (ip.AddressFamily == AddressFamily.InterNetwork)
            //    {
            //        return ip.ToString();
            //    }
            //}
            ////"No network adapters with an IPv4 address in the system!"
            //throw new System.Exception("NoIPv4InSys");

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            //do what you want with the IP here... add it to a list, just get the first and break out. Whatever.
                            Debug.Log(ip.Address.ToString());
                        }
                    }
                }
            }
            return "";
        }

        public IEnumerator StoreAnalytics_Coroutine(AnalyticsReport report)
        {
            var reports = new AnalyticsReports();
            reports.Add(report);
            var reportsJson = JsonUtility.ToJson(report);
            using (var request = UnityWebRequest.Post(this.projectSettings.ApiUrl + "/store-analytics" +
                                                      "?game_id=" + this.projectSettings.GameId +
                                                      "&level_id=" + this.sceneSettings.LevelId +
                                                      "&platform_id=" + this.GetPlatformId() +
                                                      "&ip_address="+ IpAddress +
                                                      "&json_data=" + reportsJson, ""))
            {
                request.SetRequestHeader("Accept", "application/json");
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", "Bearer " + this.ApiToken);
                request.SendWebRequest();
                while (!request.isDone)
                {
                    yield return null;
                }
                if (!string.IsNullOrEmpty(request.error))
                {
                    Debug.LogWarning("Analytics upload failed: " + request.error);
                }
                else
                {
                    if (request.isHttpError || request.isNetworkError)
                    {
                        Debug.LogWarning("Analytics upload failed: " + request.error);
                        yield break;
                    }

                    var responseJson = request.downloadHandler.text;
                    var response = JsonUtility.FromJson<StoreAnalyticsResponse>(responseJson);

                    if (response.status == false)
                    {
                        string errorMessage = "Analytics upload failed: " + response.status_code + " " + response.status_message + " " + responseJson;
                        Debug.LogWarning(errorMessage);
                        yield break;
                    }
                }
                //Debug.Log("Uploaded report.\nRequest: " + reportsJson + "\nResponse: " + responseJson);
            }
        }

        private int GetPlatformId()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor: return 1;
                case RuntimePlatform.WindowsPlayer: return 1;
                case RuntimePlatform.Android: return 2;
                case RuntimePlatform.IPhonePlayer: return 3;
                case RuntimePlatform.WebGLPlayer: return 4;
                case RuntimePlatform.OSXEditor: return 5;
                case RuntimePlatform.OSXPlayer: return 5;
                case RuntimePlatform.LinuxEditor: return 6;
                case RuntimePlatform.LinuxPlayer: return 6;
                case RuntimePlatform.Stadia: return 7;
                case RuntimePlatform.Switch: return 8;
                case RuntimePlatform.PS4: return 9;
                case RuntimePlatform.PS5: return 10;
                case RuntimePlatform.XboxOne: return 11;
                default: return 0;
            }
        }

        [Serializable]
        public class TokenRequest
        {
            public string client_id;
            public string client_secret;

            public TokenRequest(string clientId, string clientSecret)
            {
                this.client_id = clientId;
                this.client_secret = clientSecret;
            }
        }

        [Serializable]
        public class TokenResponse
        {
            public bool status;
            public int status_code;
            public string status_message;
            public Data data;

            [Serializable]
            public class Data
            {
                public int id;
                public int user_id;
                public string application_name;
                public string created_at;
                public string updated_at;
                public string token;
                public string token_expires;
            }
        }

        [Serializable]
        public class BannerResponse
        {
            public bool status;
            public int status_code;
            public string status_message;
            public List<Data> data;

            public Data GetDataByAreaCode(string area_code)
            {
                return this.data.FirstOrDefault(x => x.area_code == area_code);
            }

            [Serializable]
            public class Data
            {
                public string banner_id;
                public string campaign_id;
                public string banner_url;
                public string link;
                public string width;
                public string height;
                public string area_code;
            }
        }

        [Serializable]
        public class AnalyticsReports : List<AnalyticsReport>
        {
        }

        [Serializable]
        public class StoreAnalyticsResponse
        {
            public bool status;
            public int status_code;
            public string status_message;
        }
    }
}