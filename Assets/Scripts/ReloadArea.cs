using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadArea : MonoBehaviour
{
    private CheckIfPlayerDucking _duckCheck;

    void Start()
    {
        _duckCheck = GameObject.FindObjectOfType<CheckIfPlayerDucking>();
    }
    private void OnTriggerStay(Collider other)
    {
        GunObject gun = other.gameObject.GetComponent<GunObject>();
        if (gun == null) return;
        if (!_duckCheck.CheckDucking(gun.gameObject)) return;
        gun.Reload();
    }
}
