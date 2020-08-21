using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class FollowHand : MonoBehaviour
{
    public Transform hand;

    private Transform cachedTransform;

    private Hand handScript;
    // Start is called before the first frame update
    void Start()
    {
        cachedTransform = transform;
        handScript = hand.gameObject.GetComponent<Hand>();
    }

    // Update is called once per frame
    void Update()
    {
        cachedTransform.position = hand.position;
        cachedTransform.rotation = hand.rotation;
        handScript.ShowController(true);
    }
}
