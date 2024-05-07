using UnityEngine;
using UnityEngine.Rendering;
using static PopInGames.ApiClient;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace PopInGames
{
    [RequireComponent(typeof(MeshRenderer))]
    public class AdMesh : MonoBehaviour
    {
        
        [HideInInspector] public BannerResponse.Data BannerData;
        [SerializeField]
        private AdType type = AdType.LongHorizontal;
        private float lastRenderTime;
        public AdType Type => this.type;
        public bool IsRendered => this.lastRenderTime > Time.timeSinceLevelLoad - 0.1f;
        private bool _adLoaded;
        private void Awake()
        {
            if (!_adLoaded)
            {
                Renderer meshRenderer = this.GetComponent<MeshRenderer>();
                meshRenderer.sharedMaterial = new Material(Shader.Find("Diffuse"));
                gameObject.SetActive(false);
            }
        }

        public void ApplyTexture(Texture2D texture, string materialName)
        {
            var newMaterial = new Material(this.GetAdShader())
            {
                name = materialName,
                mainTexture = texture
            };

            Renderer meshRenderer = this.GetComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = newMaterial;
            _adLoaded = true;
            gameObject.SetActive(true);
        }

        public void ApplyColor(Color color, string materialName)
        {
            Renderer meshRenderer = this.GetComponent<MeshRenderer>();
   
            if (meshRenderer.material != null)
            {
                meshRenderer.material.color = color;
                meshRenderer.material.SetColor("_Color", color);
            }
            else
            {
                var newMaterial = new Material(this.GetAdShader())
                {
                    name = materialName,
                    color = color
                };
                newMaterial.SetColor("_Color", color);
                meshRenderer.material = newMaterial;
            }
        }

        public bool No_Shadows;
        public Shader GetAdShader()
        {
            if (GraphicsSettings.renderPipelineAsset != null)
            {
                var srpType = GraphicsSettings.renderPipelineAsset.GetType().ToString();

                if (srpType.Contains("HDRenderPipelineAsset"))
                {
                    
                    return Shader.Find("HDRP/Lit");
                }
                if (srpType.Contains("UniversalRenderPipelineAsset"))
                {
                    
                    return Shader.Find("Universal Render Pipeline/Lit");
                }
                if (srpType.Contains("LightweightRenderPipelineAsset"))
                {
                    
                    return Shader.Find("Lightweight Render Pipeline/Lit");
                }

            }

            if (No_Shadows==true)
            {
                
                return Shader.Find("Unlit/Texture");
                
            }
            else
            {
                
                return Shader.Find("Standard");
            }

            


        }

        private void OnWillRenderObject()
        {
            this.lastRenderTime = Time.timeSinceLevelLoad;
        }

        private void OnMouseUpAsButton()
        {
            OnAdClick();
        }

        private void OnAdClick()
        {
            if (!string.IsNullOrEmpty(BannerData.link))
            {
                Application.OpenURL(BannerData.link);
            }
        }
    }
}