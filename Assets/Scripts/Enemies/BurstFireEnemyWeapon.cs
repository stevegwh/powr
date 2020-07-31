using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BurstFireEnemyWeapon : MonoBehaviour
{
    // private BulletManager bulletManager;

    private List<EnemyBullet> _bulletStore;

    private EnemyAI enemyAIController;

    public GameObject Player;

    public GameObject Nozzle;

    public GameObject Bullet;

    private float _bulletTimer;

    private readonly float bulletDelay = 2f;

    private int _bulletPool = 2;

    private Transform cachedTransform;

    private Transform cachedNozzleTransform;

    public int Health = 1;

    private void Awake()
    {
        cachedTransform = transform;
        cachedNozzleTransform = Nozzle.transform;
        _bulletStore = new List<EnemyBullet>();
    }

    void Start()
    {
        // bulletManager = BulletManager.instance;
        enemyAIController = GetComponent<EnemyAI>() ?? GetComponentInParent<EnemyAI>();
        enemyAIController.Health = Health;
        Player = GameObject.Find("VRCamera");
    }

    private void OnEnable()
    {
        for (int i = 0; i < 10; i++)
        {
            _bulletStore.Add(BulletManager.instance.RequestBullet());
            _bulletStore[i].parent = gameObject;
            _bulletStore[i].gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (BulletManager.instance != null)
        {
            BulletManager.instance.ReturnBullet(_bulletStore);
        }
    }

    private void Fire()
    {
        foreach (var bullet in _bulletStore.Where(bullet => !bullet.gameObject.activeSelf))
        {
            GameObject go = bullet.gameObject;
            go.SetActive(true);
            go.transform.position = cachedNozzleTransform.position;
            go.transform.rotation = cachedNozzleTransform.rotation;
            break;
        }
    }

    void Update()
    {
        if (enemyAIController.Dead) return;
        cachedTransform.LookAt(Player.transform);

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
