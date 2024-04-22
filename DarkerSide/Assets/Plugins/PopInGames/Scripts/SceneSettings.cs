using UnityEngine;

namespace PopInGames
{
    public class SceneSettings : ScriptableObject
    {
        [SerializeField]
        private bool enabled;
        
        [SerializeField]
        private int levelId;

        public bool Enabled
        {
            get => this.enabled;
            set => this.enabled = value;
        }

        public int LevelId
        {
            get => this.levelId;
            set => this.levelId = value;
        }
    }
}