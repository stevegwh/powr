using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

// Responsible for handling what happens when the player is hit by an enemy bullet.
public class PlayerDamageController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip;

    private int health = 20;

    public GameObject Player;

    public Hand hand;

    public Text healthText;
    private Vignette vignette;

    private bool playerIsInvincible;
    private float invincibilityDelay;

    private float maxInvincibilityDelay = 2f;
    // Start is called before the first frame update
    void Start()
    {
        var vrCamera = GameObject.Find("VRCamera");
        PostProcessVolume postProcess = vrCamera.GetComponent<PostProcessVolume>();
        vignette = postProcess.profile.GetSetting<Vignette>();
        healthText.text = health.ToString();
    }

    public void TakeDamage()
    {
        if (playerIsInvincible) return;
        audioSource.clip = audioClip;
        audioSource.Play();
        invincibilityDelay = 0;
        playerIsInvincible = true;
        vignette.enabled.value = true;
        health--;
        healthText.text = health.ToString();
        hand.TriggerHapticPulse(1f, 1f, 1f);
        hand.otherHand.TriggerHapticPulse(1f, 1f, 1f);
        if (health == 0) GameOverManager.instance.InitGameOver();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;
        TakeDamage();
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
            vignette.enabled.value = false;
            playerIsInvincible = false;
        }
        
    }

    void Update()
    {
        healthText.transform.rotation = Quaternion.LookRotation(healthText.transform.position - Player.transform.position);
    }
}
