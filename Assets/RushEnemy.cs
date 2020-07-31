using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RushEnemy : MonoBehaviour
{
    private GameObject player;
    private EnemyAI enemyAIController;
    private Transform cachedrTransform;

    private float moveSpeed = 2f;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("VRCamera");
        enemyAIController = GetComponent<EnemyAI>();
        enemyAIController.Health = 0;
        cachedrTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        cachedrTransform.LookAt(player.transform.position);
        cachedrTransform.position += cachedrTransform.forward * (moveSpeed * Time.deltaTime);

    }
}
