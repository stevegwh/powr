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
        // currentFocalPoint.transform.parent = level.transform;
        // transitionPoint.GetComponent<BoxCollider>().enabled = false;
        transitionPoint.SetActive(false);
        level.SetActive(true);
        TransitionShooterRoom.RotateLevel(plane, pivotPoint);
        TransitionShooterRoom.activeTransition = null;
        TransitionShooterRoom.ToggleShowPlanes();
        TransitionShooterRoom.currentFocalPoint.StartEnemyWave();
        // TransitionShooterRoom.OnTransitionFinish(); // Calling this here which updates currentFocalPoint in AssetController and then continuing on with the same variable name below seems just a bit weird

        Transform floorVector = currentFocalPoint.transform.Find("FloorMarker").transform;
        Vector3 floorPos = floorVector.transform.TransformPoint(floorVector.position);
        environment.transform.position = new Vector3(environment.transform.position.x, floorPos.y, environment.transform.position.z);

        // Now that we've finished instantiating the focalpoint we can prime the next one
        currentFocalPoint.AssociatedTeleportPoint.SetActive(false);
        if (TransitionShooterRoom.currentFocalPoint.NextObject != null)
        {
            TransitionShooterRoom.currentFocalPoint = TransitionShooterRoom.currentFocalPoint.NextObject;
        }
        else
        {
            Debug.Log("Game over");
        }


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
