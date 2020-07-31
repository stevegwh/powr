using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricLines;

public class EnemyBullet : MonoBehaviour
{
    public GameObject parent;
    // public bool isActive;
    // private Rigidbody rb;
    private float bulletSpeed = 10f;
    public GameObject Explosion;
    private float bulletTimer = 0;
    private float maxBulletTime = 5f;
    private Transform cachedTransform;

    void Awake()
    {
        // Explosion = Instantiate(Explosion);
        // cachedExplosionTransform = Explosion.transform;
        // cachedExplosionParticleSystem = Explosion.GetComponent<ParticleSystem>();
        // rb = GetComponent<Rigidbody>();
        cachedTransform = transform;
    }

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
        // cachedExplosionTransform.position = cachedTransform.position;
        // cachedExplosionParticleSystem.Play();
        // Deactivate();
        gameObject.SetActive(false);

    }

    void OnEnable()
    {
        bulletTimer = 0;
    }

    void FixedUpdate()
    {
        // if (!isActive && !cachedExplosionParticleSystem.isPlaying)
        // {
        //     cachedExplosionParticleSystem.Stop();
        //     ReturnBullet();
        // }
        // if (!isActive) return;
        // rb.velocity = rb.transform.forward * bulletSpeed;
        transform.position += cachedTransform.forward * (bulletSpeed * Time.deltaTime);
        if (bulletTimer < maxBulletTime)
        {
            bulletTimer += Time.deltaTime;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

}
