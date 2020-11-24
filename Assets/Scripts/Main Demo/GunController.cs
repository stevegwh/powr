using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;

// Responsible for the hand holding the gun: mainly attaching the gun to the hand and binding/handling its firing function.
public class GunController : MonoBehaviour
{
    private GunObject gun;

    [SerializeField]
    private SteamVR_Action_Boolean fireButton = SteamVR_Input.GetBooleanAction("InteractUI");
    [SerializeField]
    public SteamVR_Behaviour_Pose pose;
    public Hand Hand;

    void Awake()
    {
        if (pose == null)
            pose = GetComponent<SteamVR_Behaviour_Pose>();
        if (pose == null)
            Debug.Log("No SteamVR_Behaviour_Pose component found on this object", this);
    }

    public void DetachGunFromPlayer()
    {
        Hand.DetachObject(gun.gameObject);
        fireButton.RemoveOnStateDownListener(Fire, pose.inputSource);
    }

    public void AttachGunToPlayer()
    {
        gun = FindObjectOfType<GunObject>();
        gun.Hand = Hand;
        gun.AttachGun(Hand);
        fireButton.AddOnStateDownListener(Fire, pose.inputSource);
    }


    private void Fire(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (Hand.currentAttachedObject != gun.gameObject) return;
        gun.Fire();
    }

}
