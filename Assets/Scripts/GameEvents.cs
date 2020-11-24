using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class GameEvents : MonoBehaviour
{

    public static GameEvents current;

    private void Awake()
    {
        current = this;
    }

    public event Action onSceneLoaded;

    public void SceneLoaded()
    {
        onSceneLoaded?.Invoke();
    }

    public void ResetSubscriptions() => onSceneLoaded = delegate { };

}
