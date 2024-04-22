using PopInGames;
using UnityEditor;
using UnityEngine;

namespace PopInGames.Editors
{
    [CustomEditor(typeof(AdMesh))]
    public class AdMeshEditor : Editor
    {
        AdType newValue = AdType.None;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            AdMesh targ = target as AdMesh;
            if (newValue != targ.Type)
            {
                Vector3 scale = targ.transform.localScale;
                scale.x = scale.y * Conversions.AdTypeAspectRatio(targ.Type);
                targ.transform.localScale = scale;
            }
            newValue = targ.Type;
        }
    }
}
