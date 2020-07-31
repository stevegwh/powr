using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class GunController : MonoBehaviour
{
    public GameObject GunGameObject;
    private GunObject gun;

    [SerializeField]
    private SteamVR_Action_Boolean fireButton = SteamVR_Input.GetBooleanAction("InteractUI");
    [SerializeField]
    public SteamVR_Behaviour_Pose pose;
    private Hand _hand;

    void Start()
    {
        if (pose == null)
            pose = GetComponent<SteamVR_Behaviour_Pose>();
        if (pose == null)
            Debug.Log("No SteamVR_Behaviour_Pose component found on this object", this);


        _hand = GetComponent<Hand>();
        _hand.HideSkeleton();
        gun = GunGameObject.GetComponent<GunObject>();
        gun.Hand = _hand;
        gun.AttachGun(_hand);
        fireButton.AddOnStateDownListener(Fire, pose.inputSource);
    }

    private void Fire(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (_hand.currentAttachedObject != GunGameObject) return;
        gun.Fire();
    }

}
