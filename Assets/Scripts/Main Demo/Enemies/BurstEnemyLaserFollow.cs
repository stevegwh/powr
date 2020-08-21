using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ensures that the enemy is always looking at the player.

public class BurstEnemyLaserFollow : MonoBehaviour
{
    private Transform cachedTransform;
    public GameObject Player;
    void Start()
    {
        cachedTransform = transform;
        if (Player == null)
        {
            Player = GameObject.Find("VRCamera");
        }
    }

    // Update is called once per frame
    void Update()
    {
        cachedTransform.LookAt(Player.transform);
    }
}
