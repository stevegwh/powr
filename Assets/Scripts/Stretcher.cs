using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stretcher : MonoBehaviour
{
    public enum StretcherDirection
    {
        Vertical,
        Horizontal
    }
    public GameObject TargetGameObject;
    public StretcherDirection direction;

    void Start()
    {
        transform.parent = TargetGameObject.transform;
        float offset = 0;
        if (direction == StretcherDirection.Horizontal)
        {
            offset = TargetGameObject.GetComponent<MeshFilter>().mesh.bounds.extents.x;
            if (transform.localEulerAngles.y > 0) offset *= -1;
        }
        else
        {
            offset = TargetGameObject.GetComponent<MeshFilter>().mesh.bounds.extents.y;
        }
        transform.localScale = new Vector3(transform.localPosition.x - offset, transform.localScale.y, transform.localScale.z);
        transform.parent = TargetGameObject.transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
