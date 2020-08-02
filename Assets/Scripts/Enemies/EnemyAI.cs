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

    private GameObject explosionClone;

    private AudioSource audioSource;

    public AudioClip enemyHit;

    public AudioClip enemyExplosion;

    private List<Collider> m_colliders;
    private List<MeshRenderer> m_renderers;



    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        explosionClone = Instantiate(DeathExplosion);
        explosionClone.SetActive(false);
        m_colliders = new List<Collider>(GetComponentsInChildren<Collider>());
        m_renderers = new List<MeshRenderer>(GetComponentsInChildren<MeshRenderer>());
        if (TryGetComponent(out Collider col))
        {
            m_colliders.Add(col);
        }
        if (TryGetComponent(out MeshRenderer rend))
        {
            m_renderers.Add(rend);
        }
    }

    public void TakeDamage()
    {
        if (Dead) return;
        Health--;
        if (Health < 0)
        {
            Dead = true;
            audioSource.Stop();
            audioSource.clip = enemyExplosion;
            audioSource.Play();
            foreach (var r in m_renderers)
            {
                r.enabled = false;
            }
            foreach (var c in m_colliders)
            {
                c.enabled = false;
            }

            explosionClone.SetActive(true);
            explosionClone.transform.position = transform.position;
            enemyWaveController.RemoveEnemy(gameObject);
            Destroy(explosionClone, 1f);
            Destroy(gameObject, 1f);
            return;
        }
        audioSource.Stop();
        audioSource.clip = enemyHit;
        audioSource.Play();
    }
    

}
