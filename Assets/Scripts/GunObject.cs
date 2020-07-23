using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class GunObject : MonoBehaviour
{
    public GameObject bullet;
    public Text AmmoText;
    public GameObject explosion;
    public Transform fireNozzle;
    // private Interactable _gunInteractable;
    private AudioSource audioSource;
    public int ammoCountMax = 6;
    private int ammoCount;
    public AudioClip reloadVoiceSound;
    public AudioClip reloadSound;
    public AudioClip emptyAmmoSound;
    public AudioClip gunShootSound;
    public Hand hand;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ammoCount = ammoCountMax;
    }

    public void Fire()
    {
        if (ammoCount == 0)
        {
            hand.TriggerHapticPulse(0.2f, 1f, 0.5f);
            audioSource.clip = reloadVoiceSound;
            audioSource.Play();
            return;
        }
        hand.TriggerHapticPulse(1.5f, 0.2f, 2f);
        // if (ammoCount < 0)
        // {
        //     audioSource.clip = emptyAmmoSound;
        //     audioSource.Play();
        //     return;
        // }
        ammoCount--;
        AmmoText.text = ammoCount.ToString();
        GameObject go = Instantiate(bullet, fireNozzle.position, fireNozzle.rotation);
        audioSource.clip = gunShootSound;
        audioSource.Play();
        Destroy(go, 5f);
    }

    public void Reload()
    {
        if (ammoCount > 0) return;
        audioSource.clip = reloadSound;
        audioSource.Play();
        ammoCount = ammoCountMax;
        AmmoText.text = ammoCount.ToString();
    }



}
