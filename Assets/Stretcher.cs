using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stretcher : MonoBehaviour
{
    public GameObject destination;
    private Vector3 originalScale;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    void Start()
    {
        Transform formerParent = transform.parent;
        transform.parent = null;
        float dist = Vector3.Distance(destination.transform.position, transform.position);
        transform.localScale = new Vector3(dist, originalScale.y, transform.localScale.z);
        transform.parent = formerParent;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
