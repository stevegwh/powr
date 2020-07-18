using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ShowControllers : MonoBehaviour
{
    private Hand hand;
    // Start is called before the first frame update
    void Start()
    {
        hand = GetComponent<Hand>();
        hand.ShowController(true);
        hand.otherHand.ShowController(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
