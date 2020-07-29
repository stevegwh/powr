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

    // TODO: Gettign warning about stretching to negative
    void Start()
    {
        if (GameManager.instance.gameType == GameManager.GameType.TransitionShooterControl) return;

        // TODO: Doesn't account for stretchers below the object
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
}
