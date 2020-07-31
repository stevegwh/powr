using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class TimeManager : MonoBehaviour
{
    public bool bulletTimeEnabled;
    private static TimeManager _instance;
    public static TimeManager instance
    {
        get
        {
            if ( _instance == null )
            {
                _instance = GameObject.FindObjectOfType<TimeManager>();

                // DontDestroyOnLoad( _instance.gameObject );
            }
            return _instance;
        }
    }
    //-------------------------------------------------

    private bool _timeOverrideEnabled;
    public bool TimeOverrideEnabled
    {
        get => _timeOverrideEnabled;
        set
        {
            if (!bulletTimeEnabled) _timeOverrideEnabled = false;
            if (value)
            {
                StartCoroutine(SlowmoBurst());
            }

            _timeOverrideEnabled = value;
        }
    }

    public bool gameActive { get; set; }

    public Hand handR;
    public Hand handL;
    private Vector3 headPositionLastFrame;
    private readonly float velocityChangeThreshold = 0.2f;
    private float lastTotalVel;

    private float headMultiplier = 10;
    private float sensitivity = 2;


    void Start()
    {
        gameActive = false;
        TimeOverrideEnabled = false;
        headPositionLastFrame = transform.position;
    }

    private void CalcSlowMo()
    {
        //hands
        Vector3 handLSpeed = handL.GetTrackedObjectVelocity();
        Vector3 handRSpeed = handR.GetTrackedObjectVelocity();
        var totalVel = (handLSpeed.magnitude + handRSpeed.magnitude) / 2;
        //head
        var headDistanceFromLastFrame = Vector3.Distance(transform.position, headPositionLastFrame);

        totalVel += (headDistanceFromLastFrame * headMultiplier);

        Mathf.Clamp(totalVel, 0, 1);

        // if (Math.Abs(totalVel - lastTotalVel) < velocityChangeThreshold) return;

        // Time.timeScale = TimeOverrideEnabled ? 1 : totalVel;
        Time.timeScale = totalVel;
        // Debug.Log(totalVel);
        // Debug.Log(headDistanceFromLastFrame);

        headPositionLastFrame = transform.position;
        lastTotalVel = totalVel;
    }

    private IEnumerator SlowmoBurst()
    {
        yield return new WaitForSecondsRealtime(.5f);
        TimeOverrideEnabled = false;
    }

    private void LateUpdate()
    {
        if (!bulletTimeEnabled) return;
        if (!gameActive)
        {
            Time.timeScale = 1;
            return;
        }
        CalcSlowMo();

    }
}
