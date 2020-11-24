using UnityEngine;

// Called before any other class. Can be used to set persistent variables.
public  class PersistentSettings 
{
    public static GameType GameType { get; set; }

    // //-------------------------------------------------
    // private static PersistentSettings _instance;
    // public static PersistentSettings instance
    // {
    //     get
    //     {
    //         if ( _instance == null )
    //         {
    //             _instance = new GameObject("Persistent Settings").AddComponent<PersistentSettings>();
    //
    //             DontDestroyOnLoad( _instance.gameObject );
    //         }
    //         return _instance;
    //     }
    // }


    //-------------------------------------------------

    // Start is called before the first frame update
    void Awake()
    {
        // SteamVR_Settings.instance.trackingSpace = ETrackingUniverseOrigin.TrackingUniverseStanding;
        // Debug.Log($"Room set to {SteamVR_Settings.instance.trackingSpace}");
    }

}
