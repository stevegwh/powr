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
        public AssetController FocalPoint;
        private static AudioSource _audioSource;
        private static bool gamePaused;
        public GameObject TransitionPoint;
        public GameObject vrAnchorPoint;
        public static GameObject Level;
        private SaveLoadData saveLoadData;

        public List<GameObject> _assetsToScale = new List<GameObject>(); // Prefabs of the assets to spawn
        // [SerializeField]
        private static List<GameObject> scaledAssets; // The assets after instantiation
        private static List<GameObject> generatedPlanes;
        private Dictionary<PlaneType, List<GameObject>> planeDictionary = new Dictionary<PlaneType, List<GameObject>>();


        [SerializeField]
        private SteamVR_Action_Boolean orientateButton = SteamVR_Input.GetBooleanAction("CalibrateButton");
        [SerializeField]
        public SteamVR_Behaviour_Pose pose;

        void Awake()
        {
            vrAnchorPoint = new GameObject();
            _audioSource = GetComponent<AudioSource>();
            Level = GameObject.Find("Level");
            if (pose == null)
                pose = GetComponent<SteamVR_Behaviour_Pose>();
            if (pose == null)
                Debug.Log("No SteamVR_Behaviour_Pose component found on this object", this);

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
            RotateLevel(FocalPoint.AssociatedPlane, FocalPoint.AssociatedPivotPoint);
            FocalPoint.StartEnemyWave();
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

        private void ScaleFocalPoint()
        {
            // As plane get's passed as reference and modified in AssetScaler it's important to make a visual copy of the original plane
            FocalPoint.DecideBestPlane(planeDictionary[FocalPoint.planeType]);
            GameObject planeCopy = Instantiate(FocalPoint.AssociatedPlane);
            Mesh planeMesh = planeCopy.GetComponent<MeshFilter>().mesh;
            GameObject assetScaled = AssetScaler.ScaleAsset(planeCopy, planeMesh.normals[0], FocalPoint.gameObject, false);
            scaledAssets.Add(assetScaled);
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

        public void Load()
        {
            ResetScene();
            saveLoadData.LoadPlanesFromFile(ref generatedPlanes, ref vrAnchorPoint);
            GeneratePlaneDictionary();
            ScaleFocalPoint();
            GeneratePivotPoints();
            ToggleShowPlanes();
        }

    }
}
