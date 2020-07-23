using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class EnemyAI : MonoBehaviour
{
    public GameObject DeathExplosion;

    private int _health;

    private AudioSource audioSource;

    public AudioClip enemyHit;

    public AudioClip enemyExplosion;

    private bool shouldDie;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        _health = 3;
    }

    public void TakeDamage()
    {
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
            shouldDie = true;
            return;
        }
        audioSource.Stop();
        audioSource.clip = enemyHit;
        audioSource.Play();
    }

    private void Die()
    {
        if (!audioSource.isPlaying)
        {
            Destroy(gameObject);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (shouldDie) Die();
    }
}
