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
    public class SingleObjectShooterRoom : MonoBehaviour
    {
        public static AssetController currentFocalPoint;
        public static Transition activeTransition;
        private static AudioSource _audioSource;
        private static bool gamePaused;
        public static PostProcessVolume postProcessingVol;
        public GameObject TransitionPoint;
        public GameObject vrAnchorPoint;
        public static GameObject Level;
        // private static float floorLevel;
        public static GameObject FloorMarker;
        private SaveLoadData saveLoadData;
        private bool showAssets = true;

        public List<GameObject> _assetsToScale = new List<GameObject>(); // Prefabs of the assets to spawn
        // [SerializeField]
        private static List<GameObject> scaledAssets; // The assets after instantiation
        private static List<GameObject> generatedPlanes;
        private static List<GameObject> teleportPoints;
        private static int teleportPointIndex = 1;
        private Dictionary<PlaneType, List<GameObject>> planeDictionary = new Dictionary<PlaneType, List<GameObject>>();


        [SerializeField]
        private SteamVR_Action_Boolean orientateButton = SteamVR_Input.GetBooleanAction("CalibrateButton");
        [SerializeField]
        public SteamVR_Behaviour_Pose pose;

        void Awake()
        {
            vrAnchorPoint = new GameObject();
            _audioSource = GetComponent<AudioSource>();
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
            currentFocalPoint = scaledAssets[0].GetComponent<AssetController>();
            RotateLevel(currentFocalPoint.AssociatedPlane, currentFocalPoint.AssociatedPivotPoint);
            currentFocalPoint.StartEnemyWave();
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
        // Sets all the necessary variables so that the level can transition to the next focal point
        public static void PrimeLevelForTransition(GameObject teleportPoint)
        {
            currentFocalPoint = teleportPoint.transform.parent.GetComponent<AssetController>();
            GameObject plane = currentFocalPoint.AssociatedPlane;
            GameObject pivot = currentFocalPoint.AssociatedPivotPoint;
            GameObject transitionPoint = plane.transform.GetChild(0).gameObject;
            transitionPoint.SetActive(true);
            activeTransition = new Transition(plane, pivot, transitionPoint, postProcessingVol, Level);
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
            // _assetsToScale.Add(asset);
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
            _audioSource.Play();
            OrientateLevel.Orientate(planeToMoveTo, pivotPoint, Level);
            // AlignFloor();
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

        // Instantiates a dictionary that will be used to decide the most appropriate plane for each asset to use as a reference to scale to.
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

        public static void EnableNextTeleportPoint()
        {
            teleportPoints[teleportPointIndex].gameObject.SetActive(true);
        }

        public static void OnTeleportFinish()
        {
            teleportPoints[teleportPointIndex].gameObject.SetActive(false);
            teleportPointIndex++;
        }

        public void Load()
        {
            ResetScene();
            saveLoadData.LoadPlanesFromFile(ref generatedPlanes, ref vrAnchorPoint);
            // Level.transform.position = new Vector3(0, floorLevel, 0);
            GenerateTransitionPoints();
            GeneratePlaneDictionary();
            ScaleAllAssets();
            GeneratePivotPoints();
            ToggleShowPlanes();
        }

    }
}
