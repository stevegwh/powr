﻿using System;
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
    public class TransitionShooterRoom : MonoBehaviour
    {
        public AssetController StartingObject;
        public static AssetController currentFocalPoint;
        public static Transition activeTransition;
        private static AudioSource _audioSource;
        private static bool gamePaused;
        public static GameObject Level;
        public static GameObject Environment;

        public GameObject TeleportPoint;
        public GameObject TransitionPoint;
        public GameObject vrAnchorPoint;

        private SaveLoadData saveLoadData;

        private static List<GameObject> _assetsToScale = new List<GameObject>(); // Prefabs of the assets to spawn
        private static List<GameObject> instantiatedScaledAssets; // The assets after instantiation
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
            Environment = Level.transform.Find("Environment").gameObject;

            if (pose == null)
                pose = GetComponent<SteamVR_Behaviour_Pose>();
            if (pose == null)
                Debug.Log("No SteamVR_Behaviour_Pose component found on this object", this);

            orientateButton.AddOnStateDownListener(PauseGame, pose.inputSource);

            saveLoadData = GetComponent<SaveLoadData>();
            generatedPlanes = new List<GameObject>();
            instantiatedScaledAssets = new List<GameObject>();
            Load();
        }

        private void PauseGame(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            GetComponent<PauseGame>().GamePause();
        }

        void Start()
        {
            currentFocalPoint = StartingObject;
            StartTransition();
            activeTransition.TriggerTransition();
        }

        # region Static functions
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
        public static void EnableNextTeleportPoint()
        {
            currentFocalPoint.AssociatedTeleportPoint.SetActive(true);
        }

        public static void RegisterAssetToSpawn(GameObject asset)
        {
            _assetsToScale.Add(asset);
        }
        public static void RotateLevel(GameObject planeToMoveTo, GameObject pivotPoint)
        {
            _audioSource.Play();
            OrientateLevel.Orientate(planeToMoveTo, pivotPoint, Level);
            // AlignFloor();
        }
        public static void StartTransition()
        {
            activeTransition = new Transition(Level, currentFocalPoint);
        }
        #endregion
        #region Generators
        // Instantiates Steam VR teleport points that the user will use in order to move forward in the level.
        private void GenerateTeleportPoints()
        {
            foreach (var asset in instantiatedScaledAssets)
            {
                GameObject telepoint = Instantiate(TeleportPoint);
                telepoint.transform.parent = asset.transform;
                telepoint.transform.position = asset.transform.position;
                telepoint.transform.position -= asset.transform.forward;
                asset.GetComponent<AssetController>().AssociatedTeleportPoint = telepoint;
                telepoint.SetActive(false);
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
                transitionPoint.transform.position = new Vector3(transitionPoint.transform.position.x, 0.02f, transitionPoint.transform.position.z); //TODO: Fix floor position
                transitionPoint.SetActive(false);
            }
        }

        // Creates a copy of the rotation/position of all the assets. This is necessary as the entire level gets rotated/moved so we need
        // to store where the objects were originally before any transformation.
        private void GeneratePivotPoints()
        {
            foreach (var asset in instantiatedScaledAssets)
            {
                GameObject pivotPoint = new GameObject("Pivot point");
                pivotPoint.transform.position = asset.transform.position;
                pivotPoint.transform.rotation = asset.transform.rotation;
                asset.GetComponent<AssetController>().AssociatedPivotPoint = pivotPoint;
            }
        }

        private void GenerateScaledAssets()
        {
            foreach (var asset in _assetsToScale)
            {
                // As plane get's passed as reference and modified in AssetScaler it's important to make a visual copy of the original plane
                AssetController assetController = asset.GetComponent<AssetController>();
                assetController.DecideBestPlane(planeDictionary[assetController.planeType]);
                GameObject planeCopy = Instantiate(assetController.AssociatedPlane);
                Mesh planeMesh = planeCopy.GetComponent<MeshFilter>().mesh;
                GameObject assetScaled = AssetScaler.ScaleAsset(planeCopy, planeMesh.normals[0], asset, false);
                assetController.PostScale();
                instantiatedScaledAssets.Add(assetScaled);
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
        #endregion

        private void ResetScene()
        {
            foreach (var go in generatedPlanes) Destroy(go);
            foreach (var go in instantiatedScaledAssets) Destroy(go);
            generatedPlanes.Clear();
            instantiatedScaledAssets.Clear();
        }

        public void Load()
        {
            ResetScene();
            saveLoadData.LoadPlanesFromFile(ref generatedPlanes, ref vrAnchorPoint);
            GenerateTransitionPoints();
            GeneratePlaneDictionary();
            GenerateScaledAssets();
            GeneratePivotPoints();
            GenerateTeleportPoints();
            ToggleShowPlanes();
        }

    }
}