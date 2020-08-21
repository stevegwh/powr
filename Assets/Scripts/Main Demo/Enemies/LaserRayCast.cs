using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Casts a ray until it hits either a wall of the player. This is represented by changing the start/end of the LineRenderer component.

public class LaserRayCast : MonoBehaviour
{
    private PlayerDamageController playerCollider;
    private LayerMask layerMask;
    private EnemyAI enemyAiController;
    private AudioSource audioSource;

    private LineRenderer lr;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        layerMask = LayerMask.GetMask("BodyCollider");
        playerCollider = GameObject.FindObjectOfType<PlayerDamageController>();
        lr = GetComponent<LineRenderer>();
        enemyAiController = GetComponentInParent<EnemyAI>();
    }

    private void OnEnable()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
        else
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.Play();
            }
        }
    }

    void Update()
    {
        if (enemyAiController.Dead)
        {
            audioSource.Stop();
            lr.SetPosition(0, Vector3.zero);
            return;
        }
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            // Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            playerCollider.TakeDamage();
            lr.SetPositions(new[] {transform.position, hit.point});
        }
        else
        {
            // Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            lr.SetPositions(new[]
                {transform.position, transform.TransformDirection(Vector3.forward) * 1000});
        }
    }
}
