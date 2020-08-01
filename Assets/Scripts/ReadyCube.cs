using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ReadyCube : MonoBehaviour
{
    private Interactable interactable;

    private Hand hand;


    private AssetController assetController;
    // Start is called before the first frame update
    void Start()
    {
        interactable = GetComponent<Interactable>();
        hand = GameObject.Find("RightHand").GetComponent<Hand>();
    }

    private void OnEnable()
    {
        var nextPos = GameManager.instance.currentFocalPoint;
        if (nextPos == null) return; // If game not loaded
        if (nextPos.planeType == PlaneType.Crate)
        {
            transform.position = new Vector3(nextPos.transform.position.x, nextPos.transform.position.y + 1f, nextPos.transform.position.z);
        }
        else if (nextPos.planeType == PlaneType.Door)
        {
            transform.position = nextPos.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (interactable.hoveringHand == hand && hand.IsGrabbingWithType(GrabTypes.Grip) ||
            interactable.hoveringHand == hand.otherHand && hand.otherHand.IsGrabbingWithType(GrabTypes.Grip))
        {
            // GameManager.instance.currentFocalPoint.StartEnemyWave();
            if (GameManager.instance.GameType == GameType.TransitionShooterControl)
            {
                // GameManager.instance.TransitionControlSceneTeleportEnd();
                GameManager.instance.currentFocalPoint.StartEnemyWave();
                GameManager.instance.currentFocalPoint = GameManager.instance.currentFocalPoint.NextObject;
            }
            else if (GameManager.instance.GameType == GameType.TransitionShooter)
            {
                GameManager.instance.currentFocalPoint.StartEnemyWave();
                GameManager.instance.currentFocalPoint = GameManager.instance.currentFocalPoint.NextObject;
            }
            gameObject.SetActive(false);
        }
        
    }
}
