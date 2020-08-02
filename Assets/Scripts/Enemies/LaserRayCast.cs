using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserRayCast : MonoBehaviour
{
    private PlayerDamageController playerCollider;
    private LayerMask layerMask;
    private EnemyAI enemyAiController;
    private AudioSource audioSource;

    private LineRenderer lr;
    // Start is called before the first frame update
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
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyAiController.Dead)
        {
            audioSource.Stop();
            lr.SetPosition(0, Vector3.zero);
            return;
        }
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
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
