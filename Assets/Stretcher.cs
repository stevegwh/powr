using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stretcher : MonoBehaviour
{
    public GameObject destination;

    void Start()
    {
        Transform formerParent = transform.parent;
        transform.parent = null;
        float dist = Vector3.Distance(destination.transform.position, transform.position);
        transform.localScale = new Vector3(dist, transform.localScale.y, transform.localScale.z);
        transform.parent = formerParent;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
