using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.Extras;

public class AssetController : MonoBehaviour
{

    public PlaneType planeType;
    // The real-world plane that the asset is representing.
    public GameObject AssociatedPlane;

    public GameObject AssociatedPivotPoint;

    // Iterate through generated planes and find the most appropriate one to pair with.
    // TODO: For the moment just chooses a random plane. Perhaps could be an algorithm to choose the plane closest to the actual height of the object etc.
    public void DecideBestPlane(List<GameObject> planes)
    {
        int randNum = Random.Range(0, planes.Count);
        AssociatedPlane = planes[randNum];
        Debug.Log(randNum);
    }

    void Awake()
    {
        StaticShooterRoom.RegisterAssetToSpawn(gameObject);
    }
}
