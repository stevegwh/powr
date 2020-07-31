using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    private float bulletSpeed = 50f;
    private Transform cachedTransform;
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
        GameObject explosionClone = Instantiate(Explosion, cachedTransform.position, cachedTransform.rotation);
        Destroy(explosionClone, 1f);
        Destroy(gameObject);
    }

    void Start()
    {
        // rb = GetComponent<Rigidbody>();
        cachedTransform = transform;
        TimeManager.instance.TimeOverrideEnabled = true;
    }

    void FixedUpdate()
    {
        cachedTransform.position += cachedTransform.forward * (bulletSpeed * Time.deltaTime);
        // rb.velocity = rb.transform.forward * bulletSpeed;
    }

}
