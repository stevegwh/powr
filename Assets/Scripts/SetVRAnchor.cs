using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Captures the initial state of the VR Camera. This is so we have an anchor point to base our planes off of.
public class SetVRAnchor : MonoBehaviour
{
    private VRCameraAnchor anchor;
    private GameObject vrCameraOffset;
    public bool offset = true;
    void Awake()
    {
        // anchor = GameObject.FindObjectOfType<VRCameraAnchor>();
        vrCameraOffset = transform.parent.gameObject;
        // StartCoroutine(WaitForVRPos());
    }

    private IEnumerator WaitForVRPos()
    {
        while (transform.position == Vector3.zero)
        {
            yield return null;
        }
        // vrCameraOffset.transform.localEulerAngles = new Vector3(0, -transform.localEulerAngles.y, 0);
    }

    void Update()
    {
        // if (!offset) return;
    }

}
