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
        TransitionShooterRoom.RotateLevel(plane, pivotPoint);
        TransitionShooterRoom.activeTransition = null;
        TransitionShooterRoom.ToggleShowPlanes();
        TransitionShooterRoom.currentFocalPoint.StartEnemyWave();
        TransitionShooterRoom.OnTeleportFinish();
        // TransitionShooterRoom.PauseGame(false);
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
        TransitionShooterRoom.ToggleShowPlanes();
        // postProcessingVol.enabled = true;
        // TransitionShooterRoom.PauseGame(true);
    }
}
