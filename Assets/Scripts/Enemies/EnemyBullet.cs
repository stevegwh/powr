﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private Rigidbody rb;
    private float bulletSpeed = 30f;
    public GameObject Explosion;

    public void SetBulletLayer(int layer)
    {
        gameObject.layer = layer;
    }

    public void SetBulletSpeed(float speed)
    {
        bulletSpeed = speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject explosionClone = Instantiate(Explosion, transform.position, transform.rotation);
        explosionClone.transform.parent = null;
        Destroy(explosionClone, 1f);
        Destroy(gameObject);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        rb.velocity = rb.transform.forward * bulletSpeed;
    }

}
