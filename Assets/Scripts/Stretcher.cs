using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stretcher : MonoBehaviour
{
    public GameObject TargetGameObject;

    void Awake()
    {
    }

    void Start()
    {
        // float dist = Vector3.Distance( transform.position, TargetGameObject.transform.position );
        float scaleX = Mathf.Abs(transform.position.x - TargetGameObject.transform.position.x);
        // float objectWidth = TargetGameObject.GetComponent<MeshFilter>().mesh.bounds.extents.x;
        transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
