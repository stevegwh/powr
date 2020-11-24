//======= Copyright (c) Valve Corporation, All rights reserved. ===============

using System;
using UnityEngine;
using System.Collections;

namespace Valve.VR.Extras
{
    public class SteamVR_LaserPointer : MonoBehaviour
    {
        public SteamVR_Behaviour_Pose pose;

        //public SteamVR_Action_Boolean interactWithUI = SteamVR_Input.__actions_default_in_InteractUI;
        public SteamVR_Action_Boolean interactWithUI = SteamVR_Input.GetBooleanAction("InteractUI");
        public SteamVR_Action_Boolean grabDown = SteamVR_Input.GetBooleanAction("GrabGrip");

        public bool active = true;

        public LayerMask mask;
        public Color color;
        public float thickness = 0.002f;
        public Color clickColor = Color.green;
        public GameObject holder;
        public GameObject pointer;
        private float _originalDist;
        public bool addRigidBody = false;
        public Transform reference;
        public event PointerEventHandler PointerIn;
        public event PointerEventHandler PointerOut;
        public event PointerEventHandler PointerClick;
        private Ray raycast;
        RaycastHit hit;

        Transform previousContact = null;


        private void Awake()
        {
            _originalDist = 100f;
            if (pose == null)
                pose = this.GetComponent<SteamVR_Behaviour_Pose>();
            if (pose == null)
                Debug.LogError("No SteamVR_Behaviour_Pose component found on this object", this);

            if (interactWithUI == null)
                Debug.LogError("No ui interaction action has been set on this component.", this);


            holder = new GameObject();
                // {layer = LayerMask.NameToLayer("UI")};
            holder.transform.parent = transform;
            holder.transform.localScale = new Vector3(thickness, thickness, 1f);
            holder.transform.localPosition = Vector3.zero;
            pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pointer.transform.parent = holder.transform;
            pointer.transform.localScale = Vector3.one;
            pointer.transform.localPosition = new Vector3(0f, 0f, 0.5f);
            pointer.transform.localRotation = Quaternion.identity;

            // Set pointer to UI layer
            holder.layer = 5;
            pointer.layer = 5;
            Destroy(pointer.GetComponent<BoxCollider>());

            Material newMaterial = new Material(Shader.Find("Unlit/Color"));
            newMaterial.SetColor("_Color", color);
            pointer.GetComponent<MeshRenderer>().material = newMaterial;

            holder.SetActive(false);
            pointer.SetActive(false);
        }

        private void OnDisable()
        {
            holder.SetActive(false);
            pointer.SetActive(false);
        }

        private void OnEnable()
        {
            holder.SetActive(true);
            pointer.SetActive(true);
        }

        public virtual void OnPointerIn(PointerEventArgs e)
        {
            PointerIn?.Invoke(this, e);
        }

        public virtual void OnPointerClick(PointerEventArgs e)
        {
            PointerClick?.Invoke(this, e);
        }

        public virtual void OnPointerOut(PointerEventArgs e)
        {
            PointerOut?.Invoke(this, e);
        }


        private void Update()
        {
            float dist = _originalDist;
            raycast = new Ray(transform.position, transform.forward);
            bool bHit = Physics.Raycast(raycast, out hit, 1000f, mask);


            if (previousContact && previousContact != hit.transform)
            {
                PointerEventArgs args = new PointerEventArgs
                {
                    fromInputSource = pose.inputSource, distance = 0f, flags = 0, target = previousContact
                };
                OnPointerOut(args);
                previousContact = null;
            }
            if (bHit && previousContact != hit.transform)
            {
                PointerEventArgs argsIn = new PointerEventArgs
                {
                    fromInputSource = pose.inputSource, distance = hit.distance, flags = 0, target = hit.transform
                };
                OnPointerIn(argsIn);
                previousContact = hit.transform;
            }
            if (!bHit)
            {
                previousContact = null;
            }
            if (bHit && hit.distance < _originalDist)
            {
                dist = hit.distance;
            }

            if (bHit && interactWithUI.GetStateUp(pose.inputSource))
            {
                PointerEventArgs argsClick = new PointerEventArgs
                {
                    fromInputSource = pose.inputSource, distance = hit.distance, flags = 0, target = hit.transform
                };
                OnPointerClick(argsClick);
            }

            holder.transform.localScale = new Vector3(thickness, thickness, dist);
        }
    }

    public struct PointerEventArgs
    {
        public SteamVR_Input_Sources fromInputSource;
        public uint flags;
        public float distance;
        public Transform target;
    }

    public delegate void PointerEventHandler(object sender, PointerEventArgs e);
}