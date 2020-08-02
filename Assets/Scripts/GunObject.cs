using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class GunObject : MonoBehaviour
{
    public Hand Hand;
    public Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.DetachFromOtherHand | Hand.AttachmentFlags.TurnOnKinematic;
    public Transform attachmentOffset;

    public GameObject bullet;
    public Text AmmoText;
    public GameObject explosion;
    public Transform fireNozzle;
    // private Interactable _gunInteractable;
    private AudioSource audioSource;
    public int ammoCountMax;
    private int ammoCount;
    public AudioClip reloadVoiceSound;
    public AudioClip reloadSound;
    public AudioClip emptyAmmoSound;
    public AudioClip gunShootSound;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ammoCount = ammoCountMax;
        AmmoText.text = ammoCount.ToString();
    }

    public void Fire()
    {
        if (Hand == null) return;
        if (ammoCount == 0)
        {
            Hand.TriggerHapticPulse(0.2f, 1f, 0.5f);
            audioSource.clip = reloadVoiceSound;
            audioSource.Play();
            return;
        }
        Hand.TriggerHapticPulse(1.5f, 0.2f, 2f);
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
    public void AttachGun(Hand hand)
    {
        if (hand != Hand) return;
        hand.ShowSkeleton(false);
        hand.AttachObject(gameObject, GrabTypes.Grip, attachmentFlags, attachmentOffset);
    }
}
