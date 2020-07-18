using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Serialization;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace Valve.VR.Extras
{
    [RequireComponent(typeof(SaveLoadData))]
    public class ControllerHandler : MonoBehaviour
    {
        public GameObject[] AssetsToScale; // Prefabs of the assets to spawn
        [SerializeField]
        private List<GameObject> scaledAssets; // The assets after instantiation
        private List<GameObject> generatedPlanes;
        private int assetIndex;
        public GameObject Level;
        public GameObject[] CalibrationPoints;
        private int calibrationPointIndex;
        // private float floorLevel;
        
        [SerializeField]
        private List<Vector3> controllerVertices;

        // The left and right marker in 3D space.
        private GameObject leftMarker;

        [SerializeField]
        public SteamVR_Behaviour_Pose pose;

        [SerializeField]
        public Hand hand;

        [SerializeField]
        private SteamVR_Action_Boolean mapButton = SteamVR_Input.GetBooleanAction("CalibrateButton");
        [SerializeField]
        private SteamVR_Action_Boolean orientateButton = SteamVR_Input.GetBooleanAction("GrabPinch");
        void Start()
        {
            // floorLevel = Level.transform.position.y;
            generatedPlanes = new List<GameObject>();
            scaledAssets = new List<GameObject>();
            leftMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            leftMarker.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            leftMarker.SetActive(false);
            controllerVertices = new List<Vector3>();
            if (pose == null)
                pose = GetComponent<SteamVR_Behaviour_Pose>();
            if (pose == null)
                Debug.Log("No SteamVR_Behaviour_Pose component found on this object", this);

            mapButton.AddOnStateDownListener(MapVertex, pose.inputSource);
            orientateButton.AddOnStateDownListener(OrientateLevelButtonHandler, pose.inputSource);

            SaveLoadData saveLoadData = GetComponent<SaveLoadData>();
            saveLoadData.LoadPlanesFromFile(ref generatedPlanes);
            ScaleAllAssets();
        }

         private void ScaleAllAssets()
         {
            foreach (var plane in generatedPlanes)
            {
                // As plane get's passed as reference and modified in AssetScaler it's important to make a visual copy of the original plane
                GameObject planeCopy = Instantiate(plane);
                Mesh planeMesh = planeCopy.GetComponent<MeshFilter>().mesh;
                GetScaledAsset(planeCopy, planeMesh);
            }

         }
        private void GetScaledAsset(GameObject plane, Mesh planeMesh)
        {
            GameObject assetScaled = AssetScaler.ScaleAsset(plane, planeMesh.normals[0], AssetsToScale[assetIndex], true);
            scaledAssets.Add(assetScaled);
            assetIndex++;
        }


        private void MapVertex(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) 
        {
            controllerVertices.Add(hand.transform.position);

            if (!leftMarker.activeSelf)
            {
                leftMarker.SetActive(true);
                leftMarker.transform.position = hand.transform.position;
                Debug.Log("Left marker created at: " + hand.transform.position);
                return;
            }
            Debug.Log("Right marker created at: " + hand.transform.position);
            leftMarker.SetActive(false);
        }


        // Using for debugging to cycle through the calibration points array and orientate the level.
        private void OrientateLevelButtonHandler(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            SaveLoadData saveLoadData = GetComponent<SaveLoadData>();
            saveLoadData.SavePlanesToFile(ref generatedPlanes);
            return;
            GameObject assetScaled = scaledAssets[calibrationPointIndex];
            if (assetScaled == null)
            {
                Debug.LogError("No scaled asset found");
                return;
            }

            OrientateLevel.Orientate(generatedPlanes[calibrationPointIndex], CalibrationPoints[calibrationPointIndex], Level);

            calibrationPointIndex =
                calibrationPointIndex + 1 >= CalibrationPoints.Length ? 0 : calibrationPointIndex + 1;

            // For the moment pressing trigger will just cycle through the different orientations
        }


        private void Update()
        {
            if (controllerVertices.Count < 2) return;
            GameObject plane = PlaneGenerator.GeneratePlane(controllerVertices[0], controllerVertices[1]);
            Mesh planeMesh = plane.GetComponent<MeshFilter>().mesh;
            controllerVertices.Clear();
            if (AssetsToScale[assetIndex] == null) return;
            // As plane get's passed as reference and modified in AssetScaler it's important to make a visual copy of the original plane
            GameObject planeCopy = Instantiate(plane);
            generatedPlanes.Add(planeCopy);
            GetScaledAsset(plane, planeMesh);
        }
    }
}
