using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Captures the initial state of the VR Camera. This is so we have an anchor point to base our planes off of.
public class SetVRAnchor : MonoBehaviour
{
    private VRCameraAnchor anchor;
    void Awake()
    {
        anchor = GameObject.FindObjectOfType<VRCameraAnchor>();
        StartCoroutine(WaitForVRPos());
    }

    private IEnumerator WaitForVRPos()
    {
        while (transform.position == Vector3.zero)
        {
            yield return null;
        }
        anchor.SetAnchorPosition(transform);
    }

}
