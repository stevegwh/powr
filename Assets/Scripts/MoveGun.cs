using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGun : MonoBehaviour
{
    private GameObject Player;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("VRCamera");
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Player.transform);
        
    }
}
