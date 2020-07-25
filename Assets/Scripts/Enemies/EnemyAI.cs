using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Valve.VR.InteractionSystem;

public class EnemyAI : MonoBehaviour
{
    [FormerlySerializedAs("assetController")] public AssetController enemyWaveController;
    public bool Dead { get; set; }
    public GameObject DeathExplosion;

    private int _health;

    private AudioSource audioSource;

    public AudioClip enemyHit;

    public AudioClip enemyExplosion;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        _health = 1;
    }

    public void TakeDamage()
    {
        if (Dead) return;
        _health--;
        if (_health < 0)
        {
            audioSource.Stop();
            audioSource.clip = enemyExplosion;
            audioSource.Play();
            foreach (var renderer in GetComponentsInChildren<MeshRenderer>())
            {
                renderer.enabled = false;
            }

            GameObject explosionClone = Instantiate(DeathExplosion, transform.position, transform.rotation);
            Destroy(explosionClone, 1f);
            Dead = true;
            enemyWaveController.RemoveEnemy(gameObject);
            return;
        }
        audioSource.Stop();
        audioSource.clip = enemyHit;
        audioSource.Play();
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        if (Dead) Die();
    }
    

}
