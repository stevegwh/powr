using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEnemyWeapon : MonoBehaviour
{
    private AudioSource audioSource;
    private bool fireLaserCharging;
    private bool FireLaserCharging
    {
        get => fireLaserCharging;
        set
        {
            if (value)
            {
                StartCoroutine(LaserCountdown());
            }

            fireLaserCharging = value;
        }
    }
    public GameObject Player;

    public GameObject Muzzle;

    public GameObject Bullet;

    private EnemyLaser laser;


    private void Fire()
    {
        StartCoroutine(LaserFire());
    }

    private void OnEnable()
    {
        FireLaserCharging = true;
        GameObject go = Instantiate(Bullet, Muzzle.transform.position, Muzzle.transform.rotation);
        go.transform.parent = Muzzle.transform;
        laser = go.GetComponentInChildren<EnemyLaser>();
    }

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        Player = GameObject.Find("BodyColliderDamage");
    }


    private IEnumerator LaserCountdown()
    {
        audioSource.volume = 0.2f;
        yield return new WaitForSeconds(3f);
        FireLaserCharging = false;
        Fire();
    }

    private IEnumerator LaserFire()
    {
        yield return new WaitForSeconds(3f);
        FireLaserCharging = true;
    }

    void Update()
    {
        if (FireLaserCharging) return;
        audioSource.volume += 0.01f;
        Mathf.Clamp(audioSource.volume, 0, 1);
        transform.Rotate(0, 1, 0);

    }



}
