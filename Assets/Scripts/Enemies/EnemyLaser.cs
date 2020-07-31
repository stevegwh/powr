using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricLines;

public class EnemyLaser : MonoBehaviour
{
    private LineRenderer laserBeam;

    void Start()
    {
        laserBeam = GetComponent<LineRenderer>();
    }

    void Update()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            if (hit.collider)
            {
                laserBeam.SetPosition(1, hit.point);
            }
            else
            {
                laserBeam.SetPosition(1, transform.forward*5000);
            }
        }

    }


}
