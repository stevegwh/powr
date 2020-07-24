using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRCameraAnchor : MonoBehaviour
{
    public void SetAnchorPosition(Transform newTransform)
    {
        transform.position = newTransform.position;
        transform.rotation = newTransform.rotation;
    }

}
