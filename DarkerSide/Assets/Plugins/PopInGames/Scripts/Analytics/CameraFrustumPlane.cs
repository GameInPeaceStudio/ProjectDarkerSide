using System;
using UnityEngine;

namespace PopInGames
{
    public class CameraFrustumPlane: MonoBehaviour
    {
        private CameraTracker cameraTracker;

        public void Init(CameraTracker cameraTracker)
        {
            this.cameraTracker = cameraTracker;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            var adMesh = other.GetComponent<AdMesh>();
            if (adMesh != null && adMesh.IsRendered)
            {
                if (!this.cameraTracker.CrossingAdMeshes.Contains(adMesh))
                {
                    //Debug.Log("Collision with " + other.gameObject.name);
                }
                
                this.cameraTracker.CrossingAdMeshes.Add(adMesh);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var adMesh = other.GetComponent<AdMesh>();
            if (adMesh != null)
            {
                //Debug.Log("Trigger exit: " + other.gameObject.name);
                this.cameraTracker.CrossingAdMeshes.Remove(adMesh);
            }
        }
    }
}