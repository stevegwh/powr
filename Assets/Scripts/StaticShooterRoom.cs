using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace Valve.VR.Extras
{
    public class StaticShooterRoom : MonoBehaviour
    {


        public static Transition activeTransition;
        private static bool gamePaused;
        public static PostProcessVolume postProcessingVol;
        public GameObject TeleportPoint;
        public GameObject TransitionPoint;
        public Material highLightMaterial;
        private bool showAssets = true;
        private static List<GameObject> _assetsToScale = new List<GameObject>(); // Prefabs of the assets to spawn
        // [SerializeField]
        private static List<GameObject> scaledAssets; // The assets after instantiation
        private static List<GameObject> generatedPlanes;
        private Dictionary<PlaneType, List<GameObject>> planeDictionary = new Dictionary<PlaneType, List<GameObject>>();
        private SaveLoadData saveLoadData;

        public static GameObject Level;
        // private static float floorLevel;
        public static GameObject FloorMarker;

        [SerializeField]
        private SteamVR_Action_Boolean orientateButton = SteamVR_Input.GetBooleanAction("CalibrateButton");
        [SerializeField]
        public SteamVR_Behaviour_Pose pose;

        void Awake()
        {
            postProcessingVol = GameObject.Find("VRCamera").GetComponent<PostProcessVolume>(); // TODO: Find a better way
            postProcessingVol.enabled = false;
            Level = GameObject.Find("Level");
            FloorMarker = GameObject.Find("FloorMarker");
            if (pose == null)
                pose = GetComponent<SteamVR_Behaviour_Pose>();
            if (pose == null)
                Debug.Log("No SteamVR_Behaviour_Pose component found on this object", this);

            // mapButton.AddOnStateDownListener(MapVertex, pose.inputSource);
            orientateButton.AddOnStateDownListener(PauseGame, pose.inputSource);

            saveLoadData = GetComponent<SaveLoadData>();
            generatedPlanes = new List<GameObject>();
            scaledAssets = new List<GameObject>();
            Load();
        }

        private void PauseGame(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            GetComponent<PauseGame>().GamePause();
        }

        void Start()
        {
            AssetController assetController = scaledAssets[1].GetComponent<AssetController>();
            RotateLevel(assetController.AssociatedPlane, assetController.AssociatedPivotPoint);
            // SetFloorLevel();
        }

        public static void ToggleShowPlanes()
        {
            foreach (GameObject go in generatedPlanes)
            {
                MeshRenderer renderer = go.GetComponent<MeshRenderer>();
                renderer.enabled = !renderer.enabled;
            }
        }
        public static void PauseGame(bool toggle)
        {
            Time.timeScale = Convert.ToInt32(toggle);
            gamePaused = toggle;
        }
        public static void OrientateLevelHandler(GameObject teleportPoint)
        {
            AssetController assetController = teleportPoint.transform.parent.GetComponent<AssetController>();
            GameObject plane = assetController.AssociatedPlane;
            GameObject pivot = assetController.AssociatedPivotPoint;
            GameObject transitionPoint = plane.transform.GetChild(0).gameObject;
            transitionPoint.SetActive(true);
            activeTransition = new Transition(plane, pivot, transitionPoint, postProcessingVol, Level);
        }

        // Instantiates Steam VR teleport points that the user will use in order to move forward in the level.
        // TODO: Needs to be placed on the floor properly. This can be done once we have done the code for setting the proper floor height.
        private void GenerateTeleportPoints()
        {
            foreach (var asset in scaledAssets)
            {
                GameObject telepoint = Instantiate(TeleportPoint);
                telepoint.transform.parent = asset.transform;
                telepoint.transform.position = new Vector3(asset.transform.position.x, 0, asset.transform.position.z);
                telepoint.transform.position -= asset.transform.forward;
            }

        }
        // Generates the markers for where the player has to stand before the game will teleport to the next point
        private void GenerateTransitionPoints()
        {
            foreach (var plane in generatedPlanes)
            {
                GameObject transitionPoint = Instantiate(TransitionPoint, plane.transform.position, Quaternion.identity);
                transitionPoint.transform.parent = plane.transform;
                Vector3 planeNormal = plane.GetComponent<MeshFilter>().mesh.normals[0];
                transitionPoint.transform.rotation = Quaternion.FromToRotation(transitionPoint.transform.forward, planeNormal);
                transitionPoint.transform.position += planeNormal/2;
                transitionPoint.transform.position = new Vector3(transitionPoint.transform.position.x, 0.02f, transitionPoint.transform.position.z);
                transitionPoint.SetActive(false);
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
        public static void RotateLevel(GameObject planeToMoveTo, GameObject pivotPoint)
        {
            OrientateLevel.Orientate(planeToMoveTo, pivotPoint, Level);
        }

        private void ScaleAllAssets()
        {
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

        private static void SetFloorLevel()
        {
            float floorLevel = 0.172f;
            Dictionary<GameObject, Transform> assetParentDictionary = new Dictionary<GameObject, Transform>();
            // Deparent all assets from the Level
            foreach (var asset in scaledAssets)
            {
                assetParentDictionary[asset] = asset.transform.parent;
                asset.transform.parent = null;
            }

            Vector3 previousPos = FloorMarker.transform.position;
            Level.transform.parent = FloorMarker.transform;
            FloorMarker.transform.position = new Vector3(0, FloorMarker.transform.position.y + floorLevel, 0);
            Level.transform.parent = null;
            FloorMarker.transform.position = previousPos;
            foreach (var asset in scaledAssets)
            {
                asset.transform.parent = assetParentDictionary[asset];
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
            // Level.transform.position = new Vector3(0, floorLevel, 0);
            GenerateTransitionPoints();
            GeneratePlaneDictionary();
            ScaleAllAssets();
            GeneratePivotPoints();
            GenerateTeleportPoints();
            ToggleShowPlanes();
        }


    }
}
