using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserRayCast : MonoBehaviour
{
    private PlayerDamageController playerCollider;
    private LayerMask layerMask;

    private LineRenderer lr;
    // Start is called before the first frame update
    void Start()
    {
        layerMask = LayerMask.GetMask("BodyCollider");
        playerCollider = GameObject.FindObjectOfType<PlayerDamageController>();
        lr = GetComponent<LineRenderer>();

    }

    // Update is called once per frame
    void Update()
    {

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            playerCollider.TakeDamage();
            lr.SetPositions(new[] {transform.position, hit.point});
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            lr.SetPositions(new[]
                {transform.position, transform.TransformDirection(Vector3.forward) * 1000});
        }
    }
}
