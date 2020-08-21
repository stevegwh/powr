using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIfPlayerDucking : MonoBehaviour
{
    public float Dist = 0.8f;
    public bool CheckDucking(GameObject toCheck)
    {
        return Vector3.Distance(toCheck.transform.position, transform.position) < Dist;
    }

}
