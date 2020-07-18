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
        public Material highLightMaterial;
        private bool showAssets = true;
        public GameObject[] AssetsToScale; // Prefabs of the assets to spawn
        private List<GameObject> _pivotPoints;
        [SerializeField]
        private List<GameObject> scaledAssets; // The assets after instantiation
        private List<GameObject> generatedPlanes;
        private int assetIndex;
        private SaveLoadData saveLoadData;

        public GameObject Level;
        // private float floorLevel;



        // [SerializeField]
        // private SteamVR_Action_Boolean mapButton = SteamVR_Input.GetBooleanAction("CalibrateButton");
        [SerializeField]
        private SteamVR_Action_Boolean orientateButton = SteamVR_Input.GetBooleanAction("GrabPinch");
        [SerializeField]
        public SteamVR_Behaviour_Pose pose;

        void Start()
        {
            if (pose == null)
                pose = GetComponent<SteamVR_Behaviour_Pose>();
            if (pose == null)
                Debug.Log("No SteamVR_Behaviour_Pose component found on this object", this);

            // mapButton.AddOnStateDownListener(MapVertex, pose.inputSource);
            orientateButton.AddOnStateDownListener(OrientateLevelButtonHandler, pose.inputSource);

            _pivotPoints = new List<GameObject>();
            saveLoadData = GetComponent<SaveLoadData>();
            // floorLevel = Level.transform.position.y;
            generatedPlanes = new List<GameObject>();
            scaledAssets = new List<GameObject>();
            Load();
            GeneratePivotPoints();
            assetIndex = 0;
        }

        public void ToggleShowPlanes()
        {
            foreach (GameObject go in generatedPlanes) go.SetActive(!go.activeSelf);
        }

        private void GenerateScaledAsset(GameObject plane, Mesh planeMesh)
        {
            GameObject assetScaled = AssetScaler.ScaleAsset(plane, planeMesh.normals[0], AssetsToScale[assetIndex], false);
            scaledAssets.Add(assetScaled);
            // RotateLevel(plane);
            assetIndex++;
        }

        private void OrientateLevelButtonHandler(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            assetIndex = assetIndex < AssetsToScale.Length - 1 ? assetIndex + 1 : 0;
            Debug.Log(assetIndex);
            RotateLevel(generatedPlanes[0]);
        }

        private void GeneratePivotPoints()
        {
            foreach (var asset in AssetsToScale)
            {
                GameObject pivotPoint = new GameObject("Pivot point");
                pivotPoint.transform.position = asset.transform.position;
                pivotPoint.transform.rotation = asset.transform.rotation;
                _pivotPoints.Add(pivotPoint);
            }
        }

        private void RotateLevel(GameObject planeToMoveTo)
        {
            OrientateLevel.Orientate(planeToMoveTo, _pivotPoints[assetIndex], Level);
        }

        private void ScaleAllAssets()
        {
            // TODO: This works on the assumption that there is an equal number of planes to assets.
            // foreach (var plane in generatedPlanes)
            foreach (var asset in AssetsToScale)
            {
                // As plane get's passed as reference and modified in AssetScaler it's important to make a visual copy of the original plane
                GameObject planeCopy = Instantiate(generatedPlanes[0]); // TODO: For the moment this scales every asset to one plane only.
                Mesh planeMesh = planeCopy.GetComponent<MeshFilter>().mesh;
                GenerateScaledAsset(planeCopy, planeMesh);
            }
        }

        private void ResetScene()
        {
            foreach (var go in generatedPlanes) Destroy(go);
            foreach (var go in scaledAssets) Destroy(go);
            generatedPlanes.Clear();
            scaledAssets.Clear();
            assetIndex = 0;
        }

        public void Load()
        {
            ResetScene();
            saveLoadData.LoadPlanesFromFile(ref generatedPlanes);
            ScaleAllAssets();
        }


    }
}
