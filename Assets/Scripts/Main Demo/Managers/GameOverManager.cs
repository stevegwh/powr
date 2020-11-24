using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.Extras;
using Valve.VR.InteractionSystem;

// Handles both positive and negative game over states in the game
public class GameOverManager : MonoBehaviour
{
    public GameObject GameOverText;
    public Hand hand;
    public MenuPointerController menuPointerController;
    public SteamVR_LaserPointer laserPoint;
    public GameObject Gun;
    public GameObject player;
    private SteamVR_LoadLevel levelLoader;
    public bool GameOver;

    //-------------------------------------------------
    private static GameOverManager _instance;
    public static GameOverManager instance
    {
        get
        {
            if ( _instance == null )
            {
                _instance = GameObject.FindObjectOfType<GameOverManager>();

                // DontDestroyOnLoad( _instance.gameObject );
            }
            return _instance;
        }
    }

    //-------------------------------------------------
    void Awake()
    {
        GameEvents.current.onSceneLoaded += OnceSceneLoaded;
        levelLoader = GetComponent<SteamVR_LoadLevel>();
    }

    private void OnceSceneLoaded()
    {
        menuPointerController = FindObjectOfType<MenuPointerController>();
        laserPoint = FindObjectOfType<SteamVR_LaserPointer>();
    }

    public void InitGameOver()
    {
        // Time.timeScale = 0;
        // GameOverText.SetActive(true);
        // var vrCamera = GameObject.Find("VRCamera");
        // PostProcessVolume postProcess = vrCamera.GetComponent<PostProcessVolume>();
        // var blackAndWhite = postProcess.profile.GetSetting<ColorGrading>();
        // blackAndWhite.enabled.value = true;
        // GameOverText.transform.position = vrCamera.transform.position;
        // GameOverText.transform.position += vrCamera.transform.forward;
        GameOver = true;
        menuPointerController.enabled = true;
        laserPoint.enabled = true;
        FindObjectOfType<GunController>().DetachGunFromPlayer();
        Destroy(Gun);
        GameEvents.current.ResetSubscriptions();
        levelLoader.enabled = true;
    }

    public void Victory()
    {
        GameOverText.SetActive(true);
        GameOverText.GetComponentInChildren<Text>().text = "Congratulations! You've finished the game.";
        var vrCamera = GameObject.Find("VRCamera");
        GameOverText.transform.position = vrCamera.transform.position;
        GameOverText.transform.position += vrCamera.transform.forward;
    }
}
