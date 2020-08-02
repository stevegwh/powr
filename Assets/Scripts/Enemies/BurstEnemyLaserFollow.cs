using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstEnemyLaserFollow : MonoBehaviour
{
    private Transform cachedTransform;
    public GameObject Player;
    // Start is called before the first frame update
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
