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
    public class CalibrateRoom : MonoBehaviour
    {
        private GameObject vrAnchor;
        private PlaneType _currentPlaneType { get; set; }
        private bool showAssets;
        private bool _userCanCalibrateObjects;
        public GameObject[] AssetsToScale; // Prefabs of the assets to spawn
        [SerializeField]
        private List<GameObject> scaledAssets; // The assets after instantiation
        private List<GameObject> generatedPlanes;
        private int assetIndex;
        private SaveLoadData saveLoadData;
        // private float floorLevel;
        
        [SerializeField]
        private List<Vector3> controllerVertices;

        // The left and right marker in 3D space.
        private GameObject leftMarker;
        private GameObject rightMarker;

        [SerializeField]
        public SteamVR_Behaviour_Pose pose;

        [SerializeField]
        public Hand hand;

        [SerializeField]
        private SteamVR_Action_Boolean mapButton = SteamVR_Input.GetBooleanAction("CalibrateButton");
        // [SerializeField]
        // private SteamVR_Action_Boolean orientateButton = SteamVR_Input.GetBooleanAction("GrabPinch");
        void Start()
        {
            vrAnchor = GameObject.Find("VRAnchorController");
            saveLoadData = GetComponent<SaveLoadData>();
            // _userCanCalibrateObjects = true;
            // floorLevel = Level.transform.position.y;
            generatedPlanes = new List<GameObject>();
            scaledAssets = new List<GameObject>();
            leftMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            leftMarker.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            leftMarker.SetActive(false);
            // rightMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            // rightMarker.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            // rightMarker.SetActive(false);
            controllerVertices = new List<Vector3>();
            if (pose == null)
                pose = GetComponent<SteamVR_Behaviour_Pose>();
            if (pose == null)
                Debug.Log("No SteamVR_Behaviour_Pose component found on this object", this);

            mapButton.AddOnStateDownListener(MapVertex, pose.inputSource);
            // orientateButton.AddOnStateDownListener(OrientateLevelButtonHandler, pose.inputSource);

        }

        public void StartGame()
        {
            GameObject player = GameObject.Find("Player");
            Destroy(player);
            GetComponent<SteamVR_LoadLevel>().enabled = true;
        }

        public void SetCalibrationType(CalibrationType calType)
        {
            _userCanCalibrateObjects = true;
            _currentPlaneType = calType.planeType;
        }


        public void Restart()
        {
            ResetScene();
            _userCanCalibrateObjects = true;
        }

        public void ToggleShowAssets()
        {
            showAssets = !showAssets;
            // foreach (GameObject go in scaledAssets) go.SetActive(!go.activeSelf);
        }

        public void ToggleShowPlanes()
        {
            foreach (GameObject go in generatedPlanes) go.SetActive(!go.activeSelf);
        }

        private void GenerateScaledAsset(GameObject plane, Mesh planeMesh)
        {
            GameObject objectToScale = Instantiate(AssetsToScale[assetIndex]);
            GameObject assetScaled = AssetScaler.ScaleAsset(plane, planeMesh.normals[0], objectToScale, true);
            scaledAssets.Add(assetScaled);
            assetScaled.SetActive(showAssets);
            assetIndex++;
        }
         private void ScaleAllAssets()
         {
            foreach (var plane in generatedPlanes)
            {
                // As plane get's passed as reference and modified in AssetScaler it's important to make a visual copy of the original plane
                GameObject planeCopy = Instantiate(plane);
                Mesh planeMesh = planeCopy.GetComponent<MeshFilter>().mesh;
                GenerateScaledAsset(planeCopy, planeMesh);
            }
         }

        public void Save(GameObject text)
        {
            if (_userCanCalibrateObjects)
            {
                text.GetComponent<Text>().text = "Cannot Save. Finish Calibration.";
                return;
            }; // If calibration hasn't finished.
            text.GetComponent<Text>().text = "Calibration saved!";
            saveLoadData.SavePlanesToFile(ref generatedPlanes, vrAnchor.transform);
        }

        private void ResetScene()
        {
            foreach (var go in generatedPlanes) Destroy(go);
            foreach (var go in scaledAssets) Destroy(go);
            generatedPlanes.Clear();
            scaledAssets.Clear();
            controllerVertices.Clear();
            assetIndex = 0;
            leftMarker.SetActive(false);
            _userCanCalibrateObjects = false;
        }

        public void Load()
        {
            ResetScene();
            GameObject vrAnchorCurrent = new GameObject();
            vrAnchorCurrent.transform.position = vrAnchor.transform.position;
            vrAnchorCurrent.transform.rotation = vrAnchor.transform.rotation;
            saveLoadData.LoadPlanesFromFile(ref generatedPlanes, ref vrAnchor);
            Transform vrAnchorPos = vrAnchor.transform;
            // vrAnchor.transform.position = new Vector3(
            //     vrAnchorPos.position.x - vrAnchorCurrent.position.x, 
            //     vrAnchorPos.position.y - vrAnchorCurrent.position.y,
            //     vrAnchorPos.position.z - vrAnchorCurrent.position.z
            //     );
            // vrAnchor.transform.rotation = new Quaternion(
            //     Math.Abs( vrAnchorPos.rotation.x - vrAnchorCurrent.transform.rotation.x ) ,
            //     Math.Abs( vrAnchorPos.rotation.y - vrAnchorCurrent.transform.rotation.y ) ,
            //     Math.Abs( vrAnchorPos.rotation.z - vrAnchorCurrent.transform.rotation.z ),
            //     Math.Abs( vrAnchorPos.rotation.w - vrAnchorCurrent.transform.rotation.w )
            //     );
            // ScaleAllAssets();
        }


        private void MapVertex(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) 
        {
            if (!_userCanCalibrateObjects) return;
            controllerVertices.Add(hand.transform.position);

            if (!leftMarker.activeSelf)
            {
                leftMarker.SetActive(true);
                leftMarker.transform.position = hand.transform.position;
                Debug.Log("Left marker created at: " + hand.transform.position);
                return;
            }
            // if (!rightMarker.activeSelf)
            // {
            //     rightMarker.SetActive(true);
            //     rightMarker.transform.position = hand.transform.position;
            //     Debug.Log("Right marker created at: " + hand.transform.position);
            //     return;
            // }
            Debug.Log("Right marker created at: " + hand.transform.position);
            leftMarker.SetActive(false);
            // rightMarker.SetActive(false);
        }

        //
        // // Using for debugging to cycle through the calibration points array and orientate the level.
        // private void OrientateLevelButtonHandler(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        // {
        //     SaveLoadData saveLoadData = GetComponent<SaveLoadData>();
        //     saveLoadData.SavePlanesToFile(ref generatedPlanes);
        // }

        private void FinishCalibrate()
        {
            leftMarker.SetActive(false);
            // rightMarker.SetActive(false);
            GameObject plane = PlaneGenerator.GeneratePlane(controllerVertices[0], controllerVertices[1]);
            Mesh planeMesh = plane.GetComponent<MeshFilter>().mesh;
            // if (AssetsToScale[assetIndex] == null) return;
            // As plane get's passed as reference and modified in AssetScaler it's important to make a visual copy of the original plane
            GameObject planeCopy = Instantiate(plane, vrAnchor.transform, true);
            // Assign whether the current object is a door, crate etc.
            CalibrationType calType = planeCopy.AddComponent<CalibrationType>();
            calType.planeType = _currentPlaneType;
            generatedPlanes.Add(planeCopy);
            // GenerateScaledAsset(plane, planeMesh);
            controllerVertices.Clear();
            _userCanCalibrateObjects = false;
        }


        private void Update()
        {
            if (controllerVertices.Count < 2 || !_userCanCalibrateObjects) return;
            FinishCalibrate();
            // if (scaledAssets.Count == AssetsToScale.Length) _userCanCalibrateObjects = false;
        }
    }
}
