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
    private GameObject level;
    private PostProcessVolume postProcessingVol;

    public void TriggerTransition()
    {
        // transitionPoint.GetComponent<BoxCollider>().enabled = false;
        transitionPoint.SetActive(false);
        level.SetActive(true);
        StaticShooterRoom.RotateLevel(plane, pivotPoint);
        StaticShooterRoom.activeTransition = null;
        StaticShooterRoom.ToggleShowPlanes();
        // StaticShooterRoom.PauseGame(false);
        // postProcessingVol.enabled = false;
    }

    public Transition(GameObject plane, GameObject pivotPoint, GameObject transitionPoint, PostProcessVolume postProcessingVol, GameObject level)
    {
        this.plane = plane;
        this.pivotPoint = pivotPoint;
        this.transitionPoint = transitionPoint;
        this.postProcessingVol = postProcessingVol;
        this.level = level;
        level.SetActive(false);
        StaticShooterRoom.ToggleShowPlanes();
        // postProcessingVol.enabled = true;
        // StaticShooterRoom.PauseGame(true);
    }
}
