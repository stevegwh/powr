using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Valve.VR.InteractionSystem;

public class EnemyAI : MonoBehaviour
{
    [FormerlySerializedAs("assetController")] public AssetController enemyWaveController;
    public bool Dead { get; set; }
    public int Health { get; set; }
    public GameObject DeathExplosion;

    private AudioSource audioSource;

    public AudioClip enemyHit;

    public AudioClip enemyExplosion;



    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void TakeDamage()
    {
        if (Dead) return;
        Health--;
        if (Health < 0)
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
        // audioSource.pitch = Time.timeScale;
        // Mathf.Clamp(audioSource.pitch, 0.6f, 1);
    }
    

}
