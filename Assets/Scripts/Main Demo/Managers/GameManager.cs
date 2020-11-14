using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Valve.VR;
using Valve.VR.Extras;
using Valve.VR.InteractionSystem;

// Main class responsible for loading all necessary component of the game. 
// Also responsible for transitioning from one focal point to the next.
public class GameManager : MonoBehaviour
{

    public GameObject ReadyCube;

    public GameType GameType { get; private set; }

    public AssetController StartingObject;
    public AssetController currentFocalPoint;
    public Transition activeTransition;
    private AudioSource _audioSource;
    private bool gamePaused;
    private GameObject Level;
    private GameObject Environment;

    private GameObject TeleportPoint;
    private GameObject TransitionPoint;
    public GameObject vrAnchorPoint;

    private SaveLoadData saveLoadData;

    private List<GameObject> _assetsToScale = new List<GameObject>(); // Prefabs of the assets to spawn
    private List<GameObject> instantiatedScaledAssets; // The assets after instantiation
    private List<GameObject> generatedPlanes;

    private Dictionary<PlaneType, List<GameObject>> planeDictionary = new Dictionary<PlaneType, List<GameObject>>();


    [SerializeField]
    private SteamVR_Action_Boolean orientateButton = SteamVR_Input.GetBooleanAction("CalibrateButton");
    [SerializeField]
    public SteamVR_Behaviour_Pose pose;

    //-------------------------------------------------
    private static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            if ( _instance == null )
            {
                _instance = GameObject.FindObjectOfType<GameManager>();

                // DontDestroyOnLoad( _instance.gameObject );
            }
            return _instance;
        }
    }


    //-------------------------------------------------

    void Awake()
    {
        // Set game type
        if (SetDefaults.instance != null)
        {
            GameType = SetDefaults.instance.gameType;
        }
        else
        {
            Debug.Log("No game type specified. Using control scene.");
            GameType = GameType.TransitionShooterControl;
        }
        // Find/load necessary game objects in scene
        vrAnchorPoint = new GameObject();
        _audioSource = GetComponent<AudioSource>();
        Level = GameObject.Find("Level");
        Environment = Level.transform.Find("Environment").gameObject;
        TransitionPoint = Resources.Load<GameObject>("TransitionPointView");
        TeleportPoint = Resources.Load<GameObject>("TeleportPoint");
        ReadyCube = GameObject.Find("ReadyCube");

        // Bind button
        if (pose == null)
            pose = GetComponent<SteamVR_Behaviour_Pose>();
        if (pose == null)
            Debug.Log("No SteamVR_Behaviour_Pose component found on this object", this);
        orientateButton.AddOnStateDownListener(PauseGame, pose.inputSource);

        saveLoadData = GetComponent<SaveLoadData>();
        generatedPlanes = new List<GameObject>();
        instantiatedScaledAssets = new List<GameObject>();

        // Load game.
        if (GameType == GameType.TransitionShooter)
        {
            Load();
        }
        else if (GameType == GameType.TransitionShooterControl)
        {
            TransitionControlSceneLoad();
        }

    }

    private void TransitionControlSceneLoad()
    {
        ResetScene();
        instantiatedScaledAssets = new List<GameObject>(_assetsToScale); // as we don't need to scale we can just bypass it
        // Necessary part of GenerateScaledAssets that we need
        foreach (var asset in _assetsToScale)
        {
            Transform child = asset.transform.GetChild(0);
            float objectHeight = child.GetComponent<MeshFilter>().mesh.bounds.size.y / 2;
            GameObject floorMarker = new GameObject("FloorMarker");
            floorMarker.transform.parent = asset.transform;
            floorMarker.transform.localPosition = new Vector3(child.localPosition.x,
                child.localPosition.y - objectHeight, child.localPosition.z);
        }
        GenerateTeleportPoints();
        // Allows the player to reload by bringing the gun over their shoulder. Only enabled in control version of game.
        GameObject.FindObjectOfType<CheckIfPlayerDucking>().gameObject.GetComponent<BoxCollider>().enabled = true;
    }

    private void PauseGame(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        GetComponent<PauseGame>().GamePause();
    }

    IEnumerator Start()
    {
        currentFocalPoint = StartingObject;
        if (GameType == GameType.TransitionShooter)
        {
            StartTransition();
            // activeTransition.TriggerTransition();
        }
        else if (GameType == GameType.TransitionShooterControl)
        {
            GameObject player = GameObject.Find("Player");
            player.transform.position = currentFocalPoint.transform.Find("TeleportPoint(Clone)").position;
            // currentFocalPoint.StartEnemyWave();
            // currentFocalPoint = currentFocalPoint.NextObject;
        }
        yield return null;
    }

    public void TransitionControlSceneTeleportEnd()
    {
        currentFocalPoint.AssociatedTeleportPoint.SetActive(false);
        ReadyCube.SetActive(true);
    }


    # region Helper Functions
    public void ToggleShowPlanes()
    {
        foreach (GameObject go in generatedPlanes)
        {
            MeshRenderer renderer = go.GetComponent<MeshRenderer>();
            renderer.enabled = !renderer.enabled;
        }
    }
    public void PauseGame(bool toggle)
    {
        Time.timeScale = Convert.ToInt32(toggle);
        gamePaused = toggle;
    }
    public void EnableNextTeleportPoint()
    {
        if (currentFocalPoint == null)
        {
            GameOverManager.instance.Victory();
            return;
        }
        if (currentFocalPoint.AssociatedTeleportPoint == null)
        {
            Debug.Log("No more teleports");
            return;
        }
        currentFocalPoint.AssociatedTeleportPoint.SetActive(true);
    }

    public void RegisterAssetToSpawn(GameObject asset)
    {
        _assetsToScale.Add(asset);
    }
    public void RotateLevel(GameObject planeToMoveTo, GameObject pivotPoint)
    {
        // _audioSource.Play();
        var levelOrientator = new LevelOrientator();
        levelOrientator.Orientate(planeToMoveTo, pivotPoint, Level);
    }
    public void StartTransition()
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
            // New
            Transform floorMarker = asset.transform.Find("FloorMarker");
            telepoint.transform.position = new Vector3(telepoint.transform.position.x, floorMarker.position.y, telepoint.transform.position.z);
            //
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
            // TEST CODE
            // asset.transform.parent = Environment.transform;
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

            // Instantiate a marker for the floor position
            Transform child = assetScaled.transform.GetChild(0);
            float objectHeight = child.GetComponent<MeshFilter>().mesh.bounds.size.y/2;
            GameObject floorMarker = new GameObject("FloorMarker");
            floorMarker.transform.parent = assetScaled.transform;
            floorMarker.transform.localPosition = new Vector3(child.localPosition.x, child.localPosition.y - objectHeight, child.localPosition.z);

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

    // Without doing this the other assets in the scene are not aligned on the floor correctly
    // TODO: Optimization: Only scale the assets that the player can see
    public void MoveAllAssetsToFloorLevel()
    {
        foreach (var asset in instantiatedScaledAssets)
        {
            // if (asset.GetComponent<AssetController>() == currentFocalPoint) continue;

            // Set the floorMarker as the parent of the object and drag it down to the floor.
            Transform floorMarker = asset.transform.Find("FloorMarker");
            Transform formerParent = asset.transform.parent;
            floorMarker.parent = asset.transform.parent;
            asset.transform.parent = floorMarker.transform;

            floorMarker.position = new Vector3(floorMarker.position.x, Environment.transform.position.y, floorMarker.position.z);

            asset.transform.parent = formerParent;
            floorMarker.parent = asset.transform;
        }
    }
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
