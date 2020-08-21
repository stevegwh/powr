using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.Extras;

public class ChangeRoomPosition : MonoBehaviour
{
    void Awake()
    {
        gameObject.layer = 10;
    }
    void OnTriggerEnter(Collider other)
    {
        GameManager.instance.activeTransition.TriggerTransition();
    }
}
