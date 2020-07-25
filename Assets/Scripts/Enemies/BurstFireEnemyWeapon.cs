using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstFireEnemyWeapon : MonoBehaviour
{
    private EnemyAI enemyAIController;

    public GameObject Player;

    public GameObject Nozzle;

    public GameObject Bullet;

    private float _bulletTimer;

    private readonly float bulletDelay = 1f;

    private int _bulletPool = 2;

    private void Fire()
    {
        GameObject go = Instantiate(Bullet, Nozzle.transform.position, Nozzle.transform.rotation);
        go.GetComponent<EnemyBullet>().SetBulletLayer(12);
        go.GetComponent<EnemyBullet>().SetBulletSpeed(2f);
        Destroy(go, 5f);
    }

    void Start()
    {
        enemyAIController = GetComponent<EnemyAI>();
        Player = GameObject.Find("VRCamera");
    }
    void Update()
    {
        if (enemyAIController.Dead) return;
        transform.LookAt(Player.transform);

        // Shoots a burst of three bullets in quick succession then waits 1 second.
        if (_bulletTimer < bulletDelay)
        {
            _bulletTimer += Time.deltaTime;
        }
        else
        {
            if (_bulletPool > 0)
            {
                _bulletTimer = bulletDelay - 0.3f;
                Fire();
                _bulletPool--;
            }
            else
            {
                _bulletTimer = 0;
                _bulletPool = 3;
            }
        }
        
    }
}
