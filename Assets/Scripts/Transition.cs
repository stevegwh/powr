using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Valve.VR.Extras;

public class Transition
{
    private GameObject plane;
    private GameObject pivotPoint;
    private GameObject transitionPoint;
    private PostProcessVolume postProcessingVol;

    public void TriggerTransition()
    {
        // transitionPoint.GetComponent<BoxCollider>().enabled = false;
        transitionPoint.SetActive(false);
        StaticShooterRoom.RotateLevel(plane, pivotPoint);
        StaticShooterRoom.activeTransition = null;
        // StaticShooterRoom.PauseGame(false);
        postProcessingVol.enabled = false;
    }

    public Transition(GameObject plane, GameObject pivotPoint, GameObject transitionPoint, PostProcessVolume postProcessingVol)
    {
        this.plane = plane;
        this.pivotPoint = pivotPoint;
        this.transitionPoint = transitionPoint;
        this.postProcessingVol = postProcessingVol;
        postProcessingVol.enabled = true;
        // StaticShooterRoom.PauseGame(true);
    }
}
