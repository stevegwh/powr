using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;
using Valve.VR.InteractionSystem;

public  class SetDefaults : MonoBehaviour
{
    public GameType gameType { get; set; }

    //-------------------------------------------------
    private static SetDefaults _instance;
    public static SetDefaults instance
    {
        get
        {
            if ( _instance == null )
            {
                _instance = new GameObject("Persistent Settings").AddComponent<SetDefaults>();

                DontDestroyOnLoad( _instance.gameObject );
            }
            return _instance;
        }
    }


    //-------------------------------------------------

    // Start is called before the first frame update
    void Awake()
    {
        // SteamVR_Settings.instance.trackingSpace = ETrackingUniverseOrigin.TrackingUniverseStanding;
        // Debug.Log($"Room set to {SteamVR_Settings.instance.trackingSpace}");
    }

}
