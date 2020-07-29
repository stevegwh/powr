using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricLines;

public class EnemyLaser : MonoBehaviour
{
    private VolumetricLineBehavior laserBeam;

    private SphereCollider col;

    public void SetLaserStartPos(Vector3 newPos)
    {
        laserBeam.StartPos = newPos;
    }


    void Start()
    {
        laserBeam = GetComponentInParent<VolumetricLineBehavior>();
        col = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Destroy(transform.parent.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if ( Vector3.Distance(laserBeam.StartPos, laserBeam.EndPos)  < 10000f )
        {
            laserBeam.EndPos = new Vector3(laserBeam.EndPos.x, laserBeam.EndPos.y, laserBeam.EndPos.z + 100f);
            col.center = laserBeam.EndPos;
        }

    }
}
