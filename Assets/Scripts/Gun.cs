using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Gun : MonoBehaviour
{
    public GameObject bullet;
    public GameObject explosion;
    public Transform fireNozzle;
    private AudioSource audioSource;

    [SerializeField]
    private SteamVR_Action_Boolean fireButton = SteamVR_Input.GetBooleanAction("InteractUI");
    [SerializeField]
    public SteamVR_Behaviour_Pose pose;

    // TODO: This shoots without you holding a gun
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (pose == null)
            pose = GetComponent<SteamVR_Behaviour_Pose>();
        if (pose == null)
            Debug.Log("No SteamVR_Behaviour_Pose component found on this object", this);

        fireButton.AddOnStateDownListener(Fire, pose.inputSource);
    }

    private void Fire(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        GameObject go = Instantiate(bullet, fireNozzle.position, fireNozzle.rotation);
        Bullet b = go.GetComponent<Bullet>();
        b.SetBulletLayer(13);
        b.Explosion = explosion;
        audioSource.Play();
        Destroy(go, 5f);
    }

}
