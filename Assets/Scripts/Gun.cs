using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Gun : MonoBehaviour
{
    public GameObject bullet;
    public Transform fireNozzle;
    private float bulletSpeed;
    private List<Rigidbody> bullets = new List<Rigidbody>();

    [SerializeField]
    private SteamVR_Action_Boolean fireButton = SteamVR_Input.GetBooleanAction("InteractUI");
    [SerializeField]
    public SteamVR_Behaviour_Pose pose;

    void Start()
    {
        bulletSpeed = 20f;
        if (pose == null)
            pose = GetComponent<SteamVR_Behaviour_Pose>();
        if (pose == null)
            Debug.Log("No SteamVR_Behaviour_Pose component found on this object", this);

        fireButton.AddOnStateDownListener(Fire, pose.inputSource);
    }

    private void Fire(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        Rigidbody rb = Instantiate(bullet, fireNozzle.position, fireNozzle.rotation).GetComponent<Rigidbody>();
        bullets.Add(rb);

    }

    void Update()
    {
        foreach (var rb in bullets)
        {
            rb.velocity = rb.transform.forward * bulletSpeed;
        }
    }

}
