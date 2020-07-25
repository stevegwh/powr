using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerDamageController : MonoBehaviour
{
    private PostProcessVolume postProcess;

    private float invincibilityDelay;

    private float maxInvincibilityDelay = 2f;
    // Start is called before the first frame update
    void Start()
    {
        var vrCamera = GameObject.Find("VRCamera");
        postProcess = vrCamera.GetComponent<PostProcessVolume>();
    }

    public void TakeDamage()
    {
        postProcess.enabled = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        TakeDamage();
        invincibilityDelay = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (invincibilityDelay < maxInvincibilityDelay)
        {
            invincibilityDelay += Time.deltaTime;
        }
        else
        {
            postProcess.enabled = false;
        }
        
    }
}
