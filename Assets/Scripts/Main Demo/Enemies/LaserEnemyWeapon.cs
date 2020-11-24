using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Responsible for the behaviour of the LaserEnemy's laser. Each laser will rotate a certain direction, wait, and then go back the opposite direction.
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
        cachedTransform.position = new Vector3(cachedTransform.position.x, AssociatedAsset.transform.position.y + distToFloor + 0.3f, cachedTransform.position.z);
    }

    void Start()
    {
        GameEvents.current.onSceneLoaded += OnceSceneLoaded;
        _rotation = initialDirection == LaserDirection.Right ? 0.2f : -0.2f;
        EnemyAI enemyAiComponent = GetComponent<EnemyAI>() ?? GetComponentInParent<EnemyAI>();
        enemyAiComponent.Health = Health;
    }

    private void OnceSceneLoaded()
    {
        Player = GameObject.Find("BodyColliderDamage");
    }

    private IEnumerator LaserCountdown()
    {
        yield return new WaitForSeconds(3f);
        FireLaserCharging = false;
        Fire();
    }

    private IEnumerator LaserFire()
    {
        yield return new WaitForSeconds(6f);
        FireLaserCharging = true;
        _rotation *= -1;
    }

    void Update()
    {
        if (FireLaserCharging) return;
        cachedTransform.Rotate(0, _rotation, 0);

    }



}
