using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    private bool gamePaused;

    public void GamePause()
    {
        Time.timeScale = Convert.ToInt32(gamePaused);
        gamePaused = !gamePaused;

    }

}
