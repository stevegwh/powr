using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;
using Valve.VR.InteractionSystem;

public  class SetDefaults : MonoBehaviour
{

    // Start is called before the first frame update
    void Awake()
    {
        SteamVR_Settings.instance.trackingSpace = ETrackingUniverseOrigin.TrackingUniverseStanding;
        Debug.Log($"Room set to {SteamVR_Settings.instance.trackingSpace}");
    }

}
