using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.Extras;
using Valve.VR.InteractionSystem;

// Responsible for the calibration room scene (the first scene of the game).
public class CalibrateRoom : MonoBehaviour
{
    private static CalibrateRoom _instance;
    public GameObject controllerMarker;
    public GameObject vrAnchor;
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

    private SteamVR_Action_Boolean mapButton = SteamVR_Input.GetBooleanAction("CalibrateButton");

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Debug.Log("Duplicate Calibration");
            Destroy(this);
        }
        helpMenuController = new HelpMenuController();
        generatedPlanes = new List<GameObject>();
        controllerVertices = new List<Vector3>();
        saveLoadData = GetComponent<SaveLoadData>();
        StartCoroutine(WaitForSceneLoaded());
    }

    private IEnumerator WaitForSceneLoaded()
    {
        if (!SceneManager.GetSceneByBuildIndex(0).isLoaded) yield return null;
        var playerScript = FindObjectOfType<Player>();
        GameObject playerGO;
        if (playerScript == null)
        {
            playerGO = Instantiate( Resources.Load<GameObject>("TransitionPlayer"), Vector3.zero, Quaternion.identity);
            playerGO.name = "Player";
        }
        else
        {
            playerGO = playerScript.gameObject;
        }

        hand = GameObject.Find("RightHand").GetComponent<Hand>();
        pose = hand.GetComponent<SteamVR_Behaviour_Pose>();
        hand.gameObject.GetComponent<SteamVR_LaserPointer>().enabled = true;
        hand.gameObject.GetComponent<MenuPointerController>().enabled = true;
        hand.ShowSkeleton(true);
        hand.showController = true;
        playerGO.transform.position = Vector3.zero;
        playerGO.transform.rotation = Quaternion.identity;

        leftMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        leftMarker.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        leftMarker.SetActive(false);

        mapButton.AddOnStateDownListener(MapVertex, pose.inputSource);


        GameEvents.current.SceneLoaded();
    }

    public void SetControlGame()
    {
        PersistentSettings.GameType = GameType.TransitionShooterControl;
    }

    public void SetNormalGame()
    {
        PersistentSettings.GameType = GameType.TransitionShooter;
    }

    public void StartGame()
    {
        GameEvents.current.ResetSubscriptions();

        mapButton.RemoveOnStateDownListener(MapVertex, pose.inputSource);
        GetComponent<CalibrationMarkerController>().ResetListeners();
        ControllerButtonHints.HideAllTextHints(hand);

        Destroy(controllerMarker);
        hand.showController = false;


        hand.gameObject.GetComponent<SteamVR_LaserPointer>().enabled = false;
        hand.gameObject.GetComponent<MenuPointerController>().enabled = false;
        GetComponent<SteamVR_LoadLevel>().enabled = true;
    }

    public void SetCalibrationType(CalibrationType calType)
    {
        _userCanCalibrateObjects = true;
        // GetComponent<CalibrationMarkerController>().ResetListeners();
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
    }

    public void Load()
    {
        ResetScene();
        GameObject vrAnchorCurrent = new GameObject();
        vrAnchorCurrent.transform.position = vrAnchor.transform.position;
        vrAnchorCurrent.transform.rotation = vrAnchor.transform.rotation;
        saveLoadData.LoadPlanesFromFile(ref generatedPlanes, ref vrAnchor);
        Transform vrAnchorPos = vrAnchor.transform;
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
        FinishCalibrate();
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
}
