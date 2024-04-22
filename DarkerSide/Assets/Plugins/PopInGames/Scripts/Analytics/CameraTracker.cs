using System.Collections.Generic;
using UnityEngine;

namespace PopInGames
{
    public class CameraTracker
    {
        private Camera trackingCamera;
        private Transform checkersParent;

        private AnalyticsRecorder recorder;

        private CameraFrustumPlane[] checkers = new CameraFrustumPlane[5];
        private int checkerLayer;
        private int adMeshLayer;

        public HashSet<AdMesh> AllAdMeshes { get; } = new HashSet<AdMesh>();
        public HashSet<AdMesh> PartiallySeenAds { get; } = new HashSet<AdMesh>();
        public HashSet<AdMesh> MostlySeenAds { get; } = new HashSet<AdMesh>();

        public HashSet<AdMesh> CrossingAdMeshes { get; } = new HashSet<AdMesh>();

        public static CameraTracker CreateInstance(AnalyticsRecorder recorder)
        {
            var checkerLayer = LayerMask.NameToLayer("AdChecker");
            if (checkerLayer == -1)
            {
                Debug.LogError("There is no layer called \"AdChecker\". Please create a layer with that name.");
                return null;
            }

            var adMeshLayer = LayerMask.NameToLayer("AdMesh");
            if (adMeshLayer == -1)
            {
                Debug.LogError("There is no layer called \"AdMesh\". Please create a layer with that name.");
                return null;
            }

            return new CameraTracker(recorder, checkerLayer, adMeshLayer);
        }

        private CameraTracker(AnalyticsRecorder recorder, int checkerLayer, int adMeshLayer)
        {
            this.recorder = recorder;
            this.checkerLayer = checkerLayer;
            this.adMeshLayer = adMeshLayer;

            this.AllAdMeshes.Clear();
            List<AdMesh> adMeshes = Utility.FindComponentsInScene<AdMesh>();
            foreach (AdMesh adMesh in adMeshes)
            {
                this.AllAdMeshes.Add(adMesh);
            }

            var otherLayers = new List<int>();
            for (int layer = 0; layer < 32; layer++)
            {
                if (layer != this.checkerLayer && layer != this.adMeshLayer)
                {
                    otherLayers.Add(layer);
                }
            }

            foreach (var layer in otherLayers)
            {
                Physics.IgnoreLayerCollision(layer, this.checkerLayer);
                Physics.IgnoreLayerCollision(layer, this.adMeshLayer);
            }

            Physics.IgnoreLayerCollision(this.checkerLayer, this.checkerLayer);
            Physics.IgnoreLayerCollision(this.adMeshLayer, this.adMeshLayer);

            this.SetTrackingCamera(Camera.main);
        }

        private void SetTrackingCamera(Camera camera)
        {
            if (this.checkersParent != null)
            {
                UnityEngine.Object.Destroy(this.checkersParent.gameObject);
            }

            this.trackingCamera = camera;

            this.checkersParent = new GameObject("Ad Analytics").transform;
            this.checkersParent.position = this.trackingCamera.transform.position;
            this.checkersParent.parent = this.trackingCamera.transform;

            Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
            for (int i = 0; i < 5; ++i)
            {
                GameObject checkerObject = GameObject.CreatePrimitive(PrimitiveType.Plane);

                // Make mesh invisible
                checkerObject.GetComponent<MeshRenderer>().enabled = false;

                checkerObject.name = "Checker " + (i + 1);

                checkerObject.layer = this.checkerLayer;

                var checkerTransform = checkerObject.transform;
                checkerTransform.position = -frustumPlanes[i].normal * frustumPlanes[i].distance;
                checkerTransform.rotation = Quaternion.FromToRotation(Vector3.up, frustumPlanes[i].normal);
                checkerTransform.parent = this.checkersParent;
                checkerTransform.localScale = new Vector3(2, 0.01f, 2); // y: thickness of collider

                var checkerCollider = checkerObject.GetComponent<MeshCollider>();
                checkerCollider.convex = true;
                checkerCollider.isTrigger = true;

                var checkerRigidbody = checkerObject.AddComponent<Rigidbody>();
                checkerRigidbody.isKinematic = true;

                var checkerComponent = checkerObject.AddComponent<CameraFrustumPlane>();
                checkerComponent.Init(this);

                this.checkers[i] = checkerComponent;
            }
        }

        public void Update(float deltaTime, LayerMask raycastMask)
        {
            this.PartiallySeenAds.Clear();
            this.MostlySeenAds.Clear();
            foreach (var adMesh in this.AllAdMeshes)
            {
                bool isSeen = false;
                bool isSeenComplete = false;

                if (adMesh.IsRendered)
                {
                    RaycastHit hit;
                    var direction = adMesh.transform.position - this.trackingCamera.transform.position;

                    Physics.Raycast(this.trackingCamera.transform.position, direction, out hit, Mathf.Infinity, raycastMask);
                    if (hit.collider != null)
                    {
                        if (hit.collider.GetComponent<AdMesh>() != null)
                        {
                            if (hit.collider.GetComponent<AdMesh>().Equals(adMesh))
                            {
                                isSeen = true;
                                this.PartiallySeenAds.Add(adMesh);
                            }
                        }
                    }

                    Physics.SphereCast(this.trackingCamera.transform.position, 0.5f, direction, out hit, Mathf.Infinity, raycastMask);
                    if (hit.collider != null)
                    {
                        if (hit.collider.GetComponent<AdMesh>() != null)
                        {
                            if (hit.collider.GetComponent<AdMesh>().Equals(adMesh))
                            {
                                if (!this.CrossingAdMeshes.Contains(adMesh))
                                {
                                    isSeenComplete = true;
                                    this.MostlySeenAds.Add(adMesh);
                                }
                            }
                        }
                    }
                }

                if (isSeen)
                {
                    float distance = Vector3.Distance(this.trackingCamera.transform.position, adMesh.transform.position);
                    this.recorder.AddTime(adMesh, deltaTime, isSeenComplete ? deltaTime : 0, distance);
                }
            }
        }
    }
}