﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    private Rigidbody rb;
    private float bulletSpeed = 30f;
    public GameObject Explosion;

    public void SetBulletSpeed(float speed)
    {
        bulletSpeed = speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.layer == 13) // TODO: Check this
        {
            if (other.gameObject != null)
            {
                EnemyAI ai = other.gameObject.GetComponentInParent<EnemyAI>();
                if (ai != null) ai.TakeDamage();
            }
        };
        GameObject explosionClone = Instantiate(Explosion, transform.position, transform.rotation);
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
