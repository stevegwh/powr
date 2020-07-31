using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEnemyWeapon : MonoBehaviour
{
    public enum LaserDirection
    {
        Right,
        Left
    }

    private Transform cachedTransform;
    public LaserDirection initialDirection;
    public GameObject AssociatedAsset;
    private float _rotation;
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

    // public GameObject Bullet;
    //
    // private EnemyLaser laser;

    public int Health = 2;


    private void Fire()
    {
        StartCoroutine(LaserFire());
    }

    private void OnEnable()
    {
        cachedTransform = transform;
        FireLaserCharging = true;
        // GameObject go = Instantiate(Bullet, Muzzle.transform.position, Muzzle.transform.rotation);
        // go.transform.parent = Muzzle.transform;
        // laser = go.GetComponentInChildren<EnemyLaser>();
        if (AssociatedAsset == null) return;
        Transform floorMarker = AssociatedAsset.transform.Find("FloorMarker");
        if (floorMarker == null) return;  // Floormarker not initialized yet.
        float distToFloor = Vector3.Distance(floorMarker.position, AssociatedAsset.transform.position);
        cachedTransform.position = new Vector3(cachedTransform.position.x, AssociatedAsset.transform.position.y + distToFloor, cachedTransform.position.z);
    }

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        Player = GameObject.Find("BodyColliderDamage");
        _rotation = initialDirection == LaserDirection.Right ? 0.2f : -0.2f;
        EnemyAI enemyAiComponent = GetComponent<EnemyAI>() ?? GetComponentInParent<EnemyAI>();
        enemyAiComponent.Health = Health;

    }


    private IEnumerator LaserCountdown()
    {
        // audioSource.volume = 0.2f;
        yield return new WaitForSeconds(3f);
        FireLaserCharging = false;
        Fire();
    }

    private IEnumerator LaserFire()
    {
        yield return new WaitForSeconds(2f);
        FireLaserCharging = true;
        _rotation *= -1;
    }

    void Update()
    {
        if (FireLaserCharging) return;
        // audioSource.volume += 0.01f;
        // Mathf.Clamp(audioSource.volume, 0, 1);
        cachedTransform.Rotate(0, _rotation, 0);

    }



}
