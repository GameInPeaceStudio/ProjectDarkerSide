using UnityEditor;
using UnityEngine;

namespace PopInGames.Editors
{
    public class Textures
    {
        public Texture2D White { get; private set; }

        public Texture2D Logo { get; private set; }

        public void Load(string pluginPath)
        {
            this.White = new Texture2D(1, 1); 
            this.White.SetPixel(0, 0, new Color(1, 1, 1, 1));
            this.White.Apply();

            this.Logo = AssetDatabase.LoadAssetAtPath<Texture2D>(pluginPath+"/Logo.png");
            if (this.Logo == null)
            {
                this.Logo = new Texture2D(1, 1);
                this.Logo.SetPixel(0, 0, new Color(1, 1, 1, 1));
                this.Logo.Apply();
            }
        }
    }
}