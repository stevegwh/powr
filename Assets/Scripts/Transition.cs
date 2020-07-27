using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.Extras;

public class Transition
{
    private AssetController currentFocalPoint;
    private readonly GameObject plane;
    private readonly GameObject pivotPoint;
    private GameObject transitionPoint;
    private GameObject environment;
    private GameObject level;

    public void TriggerTransition()
    {
        // currentFocalPoint.transform.parent = level.transform;
        transitionPoint.SetActive(false);
        level.SetActive(true);
        TransitionShooterRoom instance = TransitionShooterRoom.instance;
        instance.RotateLevel(plane, pivotPoint);
        instance.activeTransition = null;
        instance.ToggleShowPlanes();
        instance.currentFocalPoint.StartEnemyWave();

        // Set up an object at the base of the focal point in order to raise the floor the correct level
        Transform child = currentFocalPoint.transform.GetChild(0);
        float objectHeight = child.GetComponent<MeshFilter>().mesh.bounds.size.y/2;

        GameObject floorMarker = new GameObject("FloorMarker");
        floorMarker.transform.parent = currentFocalPoint.transform;
        floorMarker.transform.localPosition = new Vector3(child.localPosition.x, child.localPosition.y - objectHeight, child.localPosition.z);

        Transform localFloorPos = floorMarker.transform;
        Vector3 worldFloorPos = localFloorPos.transform.TransformPoint(localFloorPos.position); 
        environment.transform.position = new Vector3(environment.transform.position.x, worldFloorPos.y, environment.transform.position.z);

        // Fix teleport points' position
        currentFocalPoint.AssociatedTeleportPoint.SetActive(false);
        if (currentFocalPoint.NextObject != null)
        {
            // Make the teleport point of the next object the floor height of the current focal point.
            // Doesn't use worldFloorPos as the teleport point is local to the currentFocalPoint
            Vector3 telepoint = instance.currentFocalPoint.NextObject.AssociatedTeleportPoint.transform.position;
            instance.currentFocalPoint.NextObject.AssociatedTeleportPoint.transform.position = new Vector3(
                telepoint.x,
                localFloorPos.position.y,
                telepoint.z
            );

            // Now that we've finished instantiating the focalpoint we can prime the next one
            // currentFocalPoint.transform.parent = environment.transform;
            instance.currentFocalPoint = currentFocalPoint.NextObject;
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
        TransitionShooterRoom.instance.ToggleShowPlanes();
        // TransitionShooterRoom.PauseGame(true);
    }
}
