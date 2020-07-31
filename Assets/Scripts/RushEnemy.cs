using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RushEnemy : MonoBehaviour
{
    private GameObject player;
    private PlayerDamageController dmgController;
    private EnemyAI enemyAIController;
    private Transform cachedTransform;

    public GameObject Explosion;

    public int Health = 1;

    private float moveSpeed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("BodyColliderDamage");
        dmgController = player.GetComponent<PlayerDamageController>();

        enemyAIController = GetComponent<EnemyAI>();
        enemyAIController.Health = Health;
        cachedTransform = transform;
    }

    void OnEnable()
    {
        if (player == null) return;
        transform.LookAt(player.transform);
    }

    // Update is called once per frame
    void Update()
    {
        cachedTransform.position += cachedTransform.forward * (moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            dmgController.TakeDamage();
        }

        GameObject explosionClone = Instantiate(Explosion, transform.position, cachedTransform.rotation);
        Destroy(explosionClone, 1f);
        Destroy(gameObject);
    }
}
