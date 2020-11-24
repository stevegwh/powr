using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class CalibrationMarkerController : MonoBehaviour
{
    private bool moveMarkerEnabled;
    [SerializeField]
    private SteamVR_Behaviour_Pose pose;

    private GameObject marker;

    private Transform hand;

    private Hand handScript;

    public SteamVR_Action_Boolean moveMarkerUp;
    public SteamVR_Action_Boolean moveMarkerDown;
    private Vector3[] markerDirections = new[] { new Vector3(0.01f, 0, 0), new Vector3(0, 0.01f, 0), new Vector3(0, 0, 0.01f) };
    private int markerMoveIndex = 0;

    [SerializeField]
    private SteamVR_Action_Boolean mapButton = SteamVR_Input.GetBooleanAction("CalibrateButton");

    private void NextMarker(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (!moveMarkerEnabled) return;
        if (markerMoveIndex < markerDirections.Length - 1)
        {
            markerMoveIndex++;
        }
        else
        {
            moveMarkerEnabled = false;
            ControllerButtonHints.HideAllTextHints(hand.GetComponent<Hand>());
        }
    }
    private void MoveMarkerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (!moveMarkerEnabled) return;
        marker.transform.localPosition += markerDirections[markerMoveIndex];
    }
    private void MoveMarkerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) 
    {
        if (!moveMarkerEnabled) return;
        marker.transform.localPosition -= markerDirections[markerMoveIndex];
    }
    public void MoveMarker()
    {
        moveMarkerEnabled = true;
        ControllerButtonHints.ShowTextHint(handScript, mapButton, "Next Axis");
        ControllerButtonHints.ShowTextHint(handScript, moveMarkerDown, "Move Marker");
        markerMoveIndex = 0; 
    }

    void Awake()
    {
        marker = transform.GetChild(0).gameObject;
        GameEvents.current.onSceneLoaded += OnceSceneLoaded;
    }

    private void OnceSceneLoaded()
    {
        hand = GetComponent<CalibrateRoom>().hand.transform;
        handScript = hand.gameObject.GetComponent<Hand>();
        pose = hand.GetComponent<SteamVR_Behaviour_Pose>();
        marker.transform.position = hand.position;
        marker.transform.rotation = hand.rotation;
        marker.transform.parent = hand.gameObject.transform;
        mapButton.AddOnStateDownListener(NextMarker, pose.inputSource);
        moveMarkerUp.AddOnStateDownListener(MoveMarkerUp, pose.inputSource);
        moveMarkerDown.AddOnStateDownListener(MoveMarkerDown , pose.inputSource);
    }

    private void OnDestroy()
    {
        ResetListeners();
    }

    public void ResetListeners()
    {
        mapButton.RemoveOnStateDownListener(NextMarker, pose.inputSource);
        moveMarkerUp.RemoveOnStateDownListener(MoveMarkerUp, pose.inputSource);
        moveMarkerDown.RemoveOnStateDownListener(MoveMarkerDown , pose.inputSource);
    }
}
