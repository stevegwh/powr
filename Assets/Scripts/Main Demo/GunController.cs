﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;

// Responsible for the hand holding the gun: mainly attaching the gun to the hand and binding/handling its firing function.
public class GunController : MonoBehaviour
{
    public GameObject GunGameObject;
    private GunObject gun;

    [SerializeField]
    private SteamVR_Action_Boolean fireButton = SteamVR_Input.GetBooleanAction("InteractUI");
    [SerializeField]
    public SteamVR_Behaviour_Pose pose;
    private Hand _hand;

    void Awake()
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
    public void OnDestroy()
    {
        _hand.DetachObject(GunGameObject);
    }

    private void Fire(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (_hand.currentAttachedObject != GunGameObject) return;
        gun.Fire();
    }

}