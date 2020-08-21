using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ShowControllers : MonoBehaviour
{
    private Hand hand;
    void Start()
    {
        hand = GetComponent<Hand>();
        hand.ShowController(true);
        hand.otherHand.ShowController(true);
    }

}
