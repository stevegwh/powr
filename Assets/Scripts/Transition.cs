using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Valve.VR.Extras;

public class Transition
{
    private AssetController currentFocalPoint;
    private GameObject plane;
    private GameObject pivotPoint;
    private GameObject transitionPoint;
    private GameObject environment;
    private GameObject level;
    private PostProcessVolume postProcessingVol;

    public void TriggerTransition()
    {
        // transitionPoint.GetComponent<BoxCollider>().enabled = false;
        transitionPoint.SetActive(false);
        level.SetActive(true);
        float floorLevel = currentFocalPoint.transform.Find("FloorMarker").transform.position.y;
        environment.transform.position = new Vector3(environment.transform.position.x, floorLevel, environment.transform.position.z);
        TransitionShooterRoom.RotateLevel(plane, pivotPoint);
        TransitionShooterRoom.activeTransition = null;
        TransitionShooterRoom.ToggleShowPlanes();
        TransitionShooterRoom.currentFocalPoint.StartEnemyWave();
        TransitionShooterRoom.OnTeleportFinish();
        // TransitionShooterRoom.PauseGame(false);
        // postProcessingVol.enabled = false;
    }

    public Transition(PostProcessVolume postProcessingVol, GameObject level, AssetController currentFocalPoint)
    {
        this.currentFocalPoint = currentFocalPoint;
        plane = currentFocalPoint.AssociatedPlane;
        pivotPoint = currentFocalPoint.AssociatedPivotPoint;
        transitionPoint = plane.transform.GetChild(0).gameObject;
        transitionPoint.SetActive(true);
        this.postProcessingVol = postProcessingVol;
        this.level = level;
        environment = level.transform.Find("Environment").gameObject;
        level.SetActive(false);
        TransitionShooterRoom.ToggleShowPlanes();
        // postProcessingVol.enabled = true;
        // TransitionShooterRoom.PauseGame(true);
    }
}
