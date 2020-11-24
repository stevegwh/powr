using UnityEngine;

// Ensures that the enemy is always looking at the player.

public class BurstEnemyLaserFollow : MonoBehaviour
{
    private Transform cachedTransform;
    private Transform _player;
    void Start()
    {
        cachedTransform = transform;
        GameEvents.current.onSceneLoaded += OnceSceneLoaded;
    }

    private void OnceSceneLoaded()
    {
        if (_player == null)
        {
            _player = FindObjectOfType<Camera>().transform;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (GameOverManager.instance.GameOver) return;
        cachedTransform.LookAt(_player.transform);
    }
}
