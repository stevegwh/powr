using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelOrientator 
{
    // Each of the core assets in the level have had their original positions and rotations stored. With this class we will use them to rotate
    // the level. The entire level becomes a child of the pivot point therefore it will be subject to the same transformations we make to the pivot point.
    // Simply put - we move the pivot point to the same rotation and position of the plane and the entire level follows suit.
    public void Orientate(GameObject plane, GameObject pivotPoint, GameObject level)
    {
        pivotPoint.transform.parent = null; // Might be unnecessary 
        // Reset level's orientation and position from any previous transformations
        // level.transform.position = new Vector3(0, floorLevel, 0);
        level.transform.position = Vector3.zero;
        level.transform.rotation = Quaternion.identity;

        // Reset the asset to its original position. Necessary due to the object's position constantly changing due to the floor moving.
        pivotPoint.transform.parent = TransitionShooterRoom.instance.currentFocalPoint.transform.parent;
        TransitionShooterRoom.instance.currentFocalPoint.transform.position = pivotPoint.transform.position;
        pivotPoint.transform.parent = null; // Might be unnecessary 
        //

        // Parent level to the pivot point
        level.transform.parent = pivotPoint.transform;

        Vector3 normal = plane.GetComponent<MeshFilter>().mesh.normals[0]; // Get the direction the plane is facing

        // Save pivot points' data pre-transform
        Vector3 oldPos = pivotPoint.transform.position;
        Quaternion oldRot = pivotPoint.transform.rotation;

        // Move things
        // Now that the level is a child of the pivot point we need to rotate the pivot point to 0. This means it will be axis aligned so we can then rotate it once more to the plane's normal
        pivotPoint.transform.rotation = Quaternion.identity;
        pivotPoint.transform.rotation = Quaternion.FromToRotation(pivotPoint.transform.forward, -normal); // negative as plane normal faces player
        pivotPoint.transform.position = plane.transform.position;

        // Deparent and move pivot point back to original position so it can be used again.
        level.transform.parent = null;
        pivotPoint.transform.rotation = oldRot;
        pivotPoint.transform.position = oldPos;
    }
}
