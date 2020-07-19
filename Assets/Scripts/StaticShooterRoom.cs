using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace Valve.VR.Extras
{
    public class StaticShooterRoom : MonoBehaviour
    {
        public GameObject TeleportPoint;
        public Material highLightMaterial;
        private bool showAssets = true;
        private static List<GameObject> _assetsToScale = new List<GameObject>(); // Prefabs of the assets to spawn
        [SerializeField]
        private List<GameObject> scaledAssets; // The assets after instantiation
        private List<GameObject> generatedPlanes;
        private Dictionary<PlaneType, List<GameObject>> planeDictionary = new Dictionary<PlaneType, List<GameObject>>();
        private SaveLoadData saveLoadData;

        public GameObject Level;
        // private float floorLevel;

        [SerializeField]
        private SteamVR_Action_Boolean orientateButton = SteamVR_Input.GetBooleanAction("CalibrateButton");
        [SerializeField]
        public SteamVR_Behaviour_Pose pose;

        void Awake()
        {
            if (pose == null)
                pose = GetComponent<SteamVR_Behaviour_Pose>();
            if (pose == null)
                Debug.Log("No SteamVR_Behaviour_Pose component found on this object", this);

            // mapButton.AddOnStateDownListener(MapVertex, pose.inputSource);
            // orientateButton.AddOnStateDownListener(OrientateLevelButtonHandler, pose.inputSource);

            saveLoadData = GetComponent<SaveLoadData>();
            // floorLevel = Level.transform.position.y;
            generatedPlanes = new List<GameObject>();
            scaledAssets = new List<GameObject>();
            Load();
            GeneratePivotPoints();
            GenerateTeleportPoints();
        }

        void Start()
        {
            AssetController assetController = scaledAssets[1].GetComponent<AssetController>();
            RotateLevel(assetController.AssociatedPlane, assetController.AssociatedPivotPoint);

        }

        public void ToggleShowPlanes()
        {
            foreach (GameObject go in generatedPlanes) go.SetActive(!go.activeSelf);
        }

        public void OrientateLevelHandler(GameObject teleportPoint)
        {
            AssetController assetController = teleportPoint.transform.parent.GetComponent<AssetController>();
            GameObject plane = assetController.AssociatedPlane;
            GameObject pivot = assetController.AssociatedPivotPoint;
            RotateLevel(plane, pivot); // TODO: Only using one object
        }

        // Instantiates Steam VR teleport points that the user will use in order to move forward in the level.
        // TODO: Needs to be placed on the floor properly. This can be done once we have done the code for setting the proper floor height.
        private void GenerateTeleportPoints()
        {
            foreach (var asset in scaledAssets)
            {
                GameObject telepoint = Instantiate(TeleportPoint, asset.transform.position, asset.transform.rotation);
                telepoint.transform.position -= asset.transform.forward;
                telepoint.transform.parent = asset.transform;
            }

        }

        public static void RegisterAssetToSpawn(GameObject asset)
        {
            _assetsToScale.Add(asset);
        }

        // Creates a copy of the rotation/position of all the assets. This is necessary as the entire level gets rotated/moved so we need
        // to store where the objects were originally before any transformation.
        private void GeneratePivotPoints()
        {
            foreach (var asset in scaledAssets)
            {
                GameObject pivotPoint = new GameObject("Pivot point");
                pivotPoint.transform.position = asset.transform.position;
                pivotPoint.transform.rotation = asset.transform.rotation;
                asset.GetComponent<AssetController>().AssociatedPivotPoint = pivotPoint;
            }
        }
        private void RotateLevel(GameObject planeToMoveTo, GameObject pivotPoint)
        {
            OrientateLevel.Orientate(planeToMoveTo, pivotPoint, Level);
        }

        private void ScaleAllAssets()
        {
            // TODO: This works on the assumption that there is an equal number of planes to assets.
            // foreach (var plane in generatedPlanes)
            foreach (var asset in _assetsToScale)
            {
                // As plane get's passed as reference and modified in AssetScaler it's important to make a visual copy of the original plane
                AssetController assetController = asset.GetComponent<AssetController>();
                assetController.DecideBestPlane(planeDictionary[assetController.planeType]);
                GameObject planeCopy = Instantiate(assetController.AssociatedPlane);
                Mesh planeMesh = planeCopy.GetComponent<MeshFilter>().mesh;
                GameObject assetScaled = AssetScaler.ScaleAsset(planeCopy, planeMesh.normals[0], asset, false);
                scaledAssets.Add(assetScaled);
            }
        }

        private void GeneratePlaneDictionary()
        {
            foreach (var plane in generatedPlanes)
            {
                CalibrationType assetController = plane.GetComponent<CalibrationType>();
                if (!planeDictionary.ContainsKey(assetController.planeType))
                {
                    planeDictionary[assetController.planeType] = new List<GameObject>();
                }
                planeDictionary[assetController.planeType].Add(plane);

            }
        }

        private void ResetScene()
        {
            foreach (var go in generatedPlanes) Destroy(go);
            foreach (var go in scaledAssets) Destroy(go);
            generatedPlanes.Clear();
            scaledAssets.Clear();
        }

        public void Load()
        {
            ResetScene();
            saveLoadData.LoadPlanesFromFile(ref generatedPlanes);
            GeneratePlaneDictionary();
            ScaleAllAssets();
        }


    }
}
