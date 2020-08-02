using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
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
        private GameObject controllerMarker;
        private PlaneType _currentPlaneType { get; set; }
        private bool _userCanCalibrateObjects;
        private List<GameObject> generatedPlanes;
        private SaveLoadData saveLoadData;
        private HelpMenuController helpMenuController;
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

        public SteamVR_Action_Boolean moveMarkerUp;
        public SteamVR_Action_Boolean moveMarkerDown;
        private Vector3[] markerDirections = new[] { new Vector3(0.01f, 0, 0), new Vector3(0, 0.01f, 0), new Vector3(0, 0, 0.01f) };
        private int markerMoveIndex = 0;
        void Start()    
        {
            controllerMarker = GameObject.Find("ControllerMarker").transform.GetChild(0).gameObject;
            vrAnchor = GameObject.Find("VRAnchorController");
            saveLoadData = GetComponent<SaveLoadData>();

            helpMenuController = new HelpMenuController();
            generatedPlanes = new List<GameObject>();

            leftMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            leftMarker.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            leftMarker.SetActive(false);
            controllerVertices = new List<Vector3>();
            if (pose == null)
                pose = GetComponent<SteamVR_Behaviour_Pose>();
            if (pose == null)
                Debug.Log("No SteamVR_Behaviour_Pose component found on this object", this);
        }

        public void SetControlGame()
        {
            SetDefaults.instance.gameType = GameType.TransitionShooterControl;
        }

        public void SetNormalGame()
        {
            SetDefaults.instance.gameType = GameType.TransitionShooter;
        }

        private void NextMarker(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            if (markerMoveIndex < markerDirections.Length - 1)
            {
                markerMoveIndex++;
            }
            else
            {
                mapButton.RemoveOnStateDownListener(NextMarker, pose.inputSource);
                moveMarkerUp.RemoveOnStateDownListener(MoveMarkerUp, pose.inputSource);
                moveMarkerDown.RemoveOnStateDownListener(MoveMarkerDown , pose.inputSource);
                ControllerButtonHints.HideAllTextHints(hand);
            }
        }
        private void MoveMarkerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            controllerMarker.transform.localPosition += markerDirections[markerMoveIndex];
        }
        private void MoveMarkerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) 
        {
            controllerMarker.transform.localPosition -= markerDirections[markerMoveIndex];
        }
        public void MoveMarker()
        {
            mapButton.AddOnStateDownListener(NextMarker, pose.inputSource);
            moveMarkerUp.AddOnStateDownListener(MoveMarkerUp, pose.inputSource);
            moveMarkerDown.AddOnStateDownListener(MoveMarkerDown , pose.inputSource);
            ControllerButtonHints.ShowTextHint(hand, mapButton, "Next Axis");
            ControllerButtonHints.ShowTextHint(hand, moveMarkerDown, "Move Marker");
            markerMoveIndex = 0; 
        }
        public void StartGame()
        {
            GameObject player = GameObject.Find("Player");
            Destroy(controllerMarker);
            Destroy(player);
            GetComponent<SteamVR_LoadLevel>().enabled = true;
        }
        public void SetCalibrationType(CalibrationType calType)
        {
            _userCanCalibrateObjects = true;
            mapButton.AddOnStateDownListener(MapVertex, pose.inputSource);
            ControllerButtonHints.ShowTextHint(hand, mapButton, "Place Marker");
            _currentPlaneType = calType.planeType;
        }
        public void Restart()
        {
            ResetScene();
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
            generatedPlanes.Clear();
            controllerVertices.Clear();
            leftMarker.SetActive(false);
            _userCanCalibrateObjects = false;
            ControllerButtonHints.HideAllTextHints(hand);
            mapButton.RemoveOnStateDownListener(MapVertex, pose.inputSource);
            mapButton.RemoveOnStateDownListener(NextMarker, pose.inputSource);
            moveMarkerUp.RemoveOnStateDownListener(MoveMarkerUp, pose.inputSource);
            moveMarkerDown.RemoveOnStateDownListener(MoveMarkerDown , pose.inputSource);
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
            controllerVertices.Add(controllerMarker.transform.position);

            if (!leftMarker.activeSelf)
            {
                leftMarker.SetActive(true);
                leftMarker.transform.position = controllerMarker.transform.position;
                Debug.Log("Left marker created at: " + controllerMarker.transform.position);
                return;
            }
            Debug.Log("Right marker created at: " + controllerMarker.transform.position);
            leftMarker.SetActive(false);
        }

        private void FinishCalibrate()
        {
            leftMarker.SetActive(false);
            // rightMarker.SetActive(false);
            GameObject plane = PlaneGenerator.GeneratePlane(controllerVertices[0], controllerVertices[1]);
            plane.transform.SetParent(vrAnchor.transform, true);

            // Assign whether the current object is a door, crate etc.
            CalibrationType calType = plane.AddComponent<CalibrationType>();
            calType.planeType = _currentPlaneType;
            generatedPlanes.Add(plane);

            // Clean up
            controllerVertices.Clear();
            _userCanCalibrateObjects = false;
            ControllerButtonHints.HideAllTextHints(hand);
            mapButton.RemoveOnStateDownListener(MapVertex, pose.inputSource);
        }

        public void NextHelpMenuText(GameObject text)
        {
            text.GetComponent<Text>().text = helpMenuController.NextMessage();
        }
        public void PreviousHelpMenuText(GameObject text)
        {
            text.GetComponent<Text>().text = helpMenuController.PreviousMessage();
        }
        public void FirstHelpMenuText(GameObject text)
        {
            text.GetComponent<Text>().text = helpMenuController.FirstMessage();
        }

        private void Update()
        {
            if (controllerVertices.Count < 2 || !_userCanCalibrateObjects) return;
            FinishCalibrate();
        }
    }
}
