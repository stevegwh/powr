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

    public void TriggerTransition()
    {
        transitionPoint.SetActive(false);
        level.SetActive(true);
        TransitionShooterRoom.RotateLevel(plane, pivotPoint);
        TransitionShooterRoom.activeTransition = null;
        TransitionShooterRoom.ToggleShowPlanes();
        TransitionShooterRoom.currentFocalPoint.StartEnemyWave();

        Transform localFloorPos = currentFocalPoint.transform.Find("FloorMarker").transform; 
        Vector3 worldFloorPos = localFloorPos.transform.TransformPoint(localFloorPos.position); 
        environment.transform.position = new Vector3(environment.transform.position.x, worldFloorPos.y, environment.transform.position.z);

        currentFocalPoint.AssociatedTeleportPoint.SetActive(false);
        if (currentFocalPoint.NextObject != null)
        {
            // Make the teleport point of the next object the floor height of the current focal point.
            // Doesn't use worldFloorPos as the teleport point is local to the currentFocalPoint
            TransitionShooterRoom.currentFocalPoint.NextObject.AssociatedTeleportPoint.transform.position = new Vector3(
                TransitionShooterRoom.currentFocalPoint.NextObject.AssociatedTeleportPoint.transform.position.x, 
                localFloorPos.position.y, 
                TransitionShooterRoom.currentFocalPoint.NextObject.AssociatedTeleportPoint.transform.position.z);

            // Now that we've finished instantiating the focalpoint we can prime the next one
            TransitionShooterRoom.currentFocalPoint = currentFocalPoint.NextObject;
        }
        else
        {
            Debug.Log("Game over");
        }


    }

    public Transition(GameObject level, AssetController currentFocalPoint)
    {
        this.currentFocalPoint = currentFocalPoint;
        plane = currentFocalPoint.AssociatedPlane;
        pivotPoint = currentFocalPoint.AssociatedPivotPoint;
        transitionPoint = plane.transform.GetChild(0).gameObject;
        transitionPoint.SetActive(true);
        this.level = level;
        environment = level.transform.Find("Environment").gameObject;
        level.SetActive(false);
        TransitionShooterRoom.ToggleShowPlanes();
        // TransitionShooterRoom.PauseGame(true);
    }
}
