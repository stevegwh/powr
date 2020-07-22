using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
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
        if (gameObject.layer == 12) return;
        if (gameObject.layer == 13)
        {
            Destroy(other.gameObject);
        };
        GameObject explosionClone = Instantiate(Explosion, transform.position, transform.rotation);
        Destroy(explosionClone, 1f);
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
