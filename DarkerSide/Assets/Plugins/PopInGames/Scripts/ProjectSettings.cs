using UnityEngine;

namespace PopInGames
{
    public class ProjectSettings : ScriptableObject
    {
        private static ProjectSettings settings;
       
        public string ApiUrl = "https://popingame.com/api";

        [SerializeField]
        private string clientId;

        [SerializeField]
        private string clientSecret;
        
        [SerializeField]
        private string gameId = "";

        [SerializeField]
        private int blockingLayers = 1;

        [SerializeField]
        private bool refreshFlag;
        
        [SerializeField]
        private float refreshTime;

        public float RefreshTime
        {
            get
            {
                return refreshTime;
            }
            set
            {
                refreshTime = value;
            }
        }
        public bool RefreshFlag {
            get
            {
                return refreshFlag;
            }
            set
            {
                refreshFlag = value;
            }
        }

        public string ClientId
        {
            get => this.clientId;
            set => this.clientId = value;
        }

        public string ClientSecret
        {
            get => this.clientSecret;
            set => this.clientSecret = value;
        }

        public string GameId
        {
            get => this.gameId;
            set => this.gameId = value;
        }

        public int BlockingLayers
        {
            get => this.blockingLayers;
            set => this.blockingLayers = value;
        }
        
        public LayerMask BlockingLayersConverted
        {
            get
            {
                int layerMask = 0;

                int physicsLayer = 0;
                int configLayer = 0;
                while (physicsLayer < 32)
                {
                    string layerName = LayerMask.LayerToName(physicsLayer);
                    if (layerName.Length > 0 && layerName != "AdMesh" && layerName != "AdChecker")
                    {
                        if (this.BlockingLayers == (this.BlockingLayers | 1 << configLayer))
                        {
                            layerMask |= 1 << physicsLayer;
                        }
                        configLayer++;
                    }
                    physicsLayer++;
                }

                // Add the layer AdMesh because it is mandatory
                layerMask |= 1 << LayerMask.NameToLayer("AdMesh");

                return layerMask;
            }
        }
    }
}