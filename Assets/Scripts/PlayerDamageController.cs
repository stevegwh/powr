using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerDamageController : MonoBehaviour
{
    // private Vignette vignette;

    private float invincibilityDelay;

    private float maxInvincibilityDelay = 2f;
    // Start is called before the first frame update
    void Start()
    {
        // var vrCamera = GameObject.Find("VRCamera");
        // PostProcessVolume postProcess = vrCamera.GetComponent<PostProcessVolume>();
        // vignette = postProcess.profile.GetSetting<Vignette>();
    }

    public void TakeDamage()
    {
        // vignette.enabled.value = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        TakeDamage();
        invincibilityDelay = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (invincibilityDelay < maxInvincibilityDelay)
        {
            invincibilityDelay += Time.fixedUnscaledDeltaTime;
        }
        else
        {
            // vignette.enabled.value = false;
        }
        
    }
}
