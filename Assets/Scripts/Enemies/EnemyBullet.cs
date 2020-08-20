using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricLines;

// Responsible for handling all generic enemy bullet functionality. Attached to the bullet itself.

public class EnemyBullet : MonoBehaviour
{
    public GameObject parent;
    private SphereCollider m_collider;
    private MeshRenderer m_renderer;
    private float bulletSpeed = 5f;
    public GameObject Explosion;
    private float bulletTimer = 0;
    private float maxBulletTime = 10f;
    private Transform cachedTransform;
    private Transform cachedExplosionTransform;

    void Awake()
    {
        m_renderer = GetComponent<MeshRenderer>();
        m_collider = GetComponent<SphereCollider>();
        Explosion = Instantiate(Explosion);
        cachedExplosionTransform = Explosion.transform;
        Explosion.SetActive(false);
        // rb = GetComponent<Rigidbody>();
        cachedTransform = transform;
    }

    public void SetBulletLayer(int layer)
    {
        gameObject.layer = layer;
    }

    private void OnTriggerEnter(Collider other)
    {
        Explosion.SetActive(true);
        cachedExplosionTransform.position = cachedTransform.position;
        m_renderer.enabled = false;
        m_collider.enabled = false;
        StartCoroutine(WaitForExplosion());
    }

    private void OnDisable()
    {
        Explosion.SetActive(false);
    }

    private IEnumerator WaitForExplosion()
    {
        yield return new WaitForSecondsRealtime(1f);
        // m_renderer.enabled = true;
        // m_collider.enabled = true;
        Explosion.SetActive(false);
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        bulletTimer = 0;
    }

    void FixedUpdate()
    {
        if (Explosion.activeSelf) return;
        cachedTransform.position += cachedTransform.forward * (bulletSpeed * Time.deltaTime);
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
