using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetScaler : MonoBehaviour
{
    public static GameObject ScaleAsset(GameObject plane, Vector3 normal, GameObject assetToScale, bool immediateScaleAndMove = false)
    {

        // GameObject assetToScale = AssetsToScale[assetIndex];
        Mesh planeMesh = plane.GetComponent<MeshFilter>().mesh;

        // Align both the plane and the asset with an axis so that we can calculate their bounds accurately.
        // As mesh bounds are 'axis aligned' bounding boxes it's important to make sure they are aligned to an axis ;-)
        plane.transform.rotation = Quaternion.FromToRotation(normal, Vector3.forward);
        Quaternion assetToScalePreviousRot = assetToScale.transform.rotation;
        assetToScale.transform.rotation = Quaternion.identity; // It might make more sense to align this to Vector3.forward like the plane.
        planeMesh.RecalculateBounds();

        #region Asset Scaling
        // Scale the model
        // Does not scale by depth for the moment (would require one additional point set by the user)
        GameObject assetScaledChild = assetToScale.transform.GetChild(0).gameObject; // assetToScale is just a parent GO for pivoting purposes. The child is the real object.
        Mesh assetScaledMesh = assetScaledChild.GetComponent<MeshFilter>().mesh;
        Vector3 targetScale = plane.GetComponent<MeshRenderer>().bounds.size;
        Vector3 modelScale = assetScaledChild.GetComponent<MeshRenderer>().bounds.size;

        float xFraction = modelScale.x / targetScale.x;
        float yFraction = modelScale.y / targetScale.y;
        // float zFraction = modelScale.z / targetScale.z;
        Vector3 newScale = new Vector3(assetToScale.transform.localScale.x/xFraction, assetToScale.transform.localScale.y/yFraction, assetToScale.transform.localScale.z);
        assetToScale.transform.localScale = newScale;

        #endregion 

        assetToScale.transform.rotation = assetToScalePreviousRot;

        // By enabling this you can automatically move the asset to the plane it was scaled to.
        if (immediateScaleAndMove)
        {
            // TODO: make this a public function so you can move assets to a plane's position at any time.
            assetToScale.transform.rotation = Quaternion.FromToRotation(assetToScale.transform.forward, -normal);
            assetToScale.transform.position = plane.transform.position;
        }

        // DEBUG
        // LevelOrientator(assetToScale, assetIndex);
        //

        assetScaledMesh.RecalculateBounds();
        assetScaledMesh.RecalculateNormals();

        #region Sphere Normals Debug
        // Just to make the direction the object is facing easier to see
        // var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // sphere.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        // sphere.transform.position = plane.transform.position + normal;
        // var sphere2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // sphere2.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        // sphere2.transform.position = assetToScale.transform.position;
        #endregion

        Destroy(plane);
        return assetToScale;
    }
}
