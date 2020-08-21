using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.Extras;

// The transition between one focal point to another.
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
        transitionPoint.SetActive(false);
        level.SetActive(true);
        // currentFocalPoint.transform.parent = level.transform; // TEST CODE
        GameManager instance = GameManager.instance;
        instance.RotateLevel(plane, pivotPoint);
        instance.activeTransition = null;
        instance.ToggleShowPlanes();
        // instance.currentFocalPoint.StartEnemyWave();

        // Move the environment's floor to the base of the object we are transitioning to
        GameObject floorMarker = currentFocalPoint.transform.Find("FloorMarker").gameObject;
        Transform localFloorPos = floorMarker.transform;
        Vector3 worldFloorPos = localFloorPos.transform.TransformPoint(localFloorPos.position); 
        environment.transform.position = new Vector3(environment.transform.position.x, worldFloorPos.y, environment.transform.position.z);
        instance.MoveAllAssetsToFloorLevel();

        // Fix teleport point's position
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
        }
        else
        {
            Debug.Log("Game over");
        }
        GameManager.instance.ReadyCube.SetActive(true);
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
        GameManager.instance.ToggleShowPlanes();
        // GameManager.PauseGame(true);
    }
}
