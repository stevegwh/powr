using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public GameObject Bullet;

    private List<EnemyBullet> bulletPool;

    private static BulletManager _instance;
    public static BulletManager instance
    {
        get
        {
            if ( _instance == null )
            {
                _instance = GameObject.FindObjectOfType<BulletManager>();

                // DontDestroyOnLoad( _instance.gameObject );
            }
            return _instance;
        }
    }
    //-------------------------------------------------

    void Awake()
    {
        bulletPool = new List<EnemyBullet>();
        GenerateMoreBullets();
    }

    private void GenerateMoreBullets()
    {
        for (int i = 0; i < 50; i++)
        {
            GameObject go = Instantiate(Bullet);
            EnemyBullet bullet = go.GetComponent<EnemyBullet>();
            bullet.SetBulletLayer(12);
            bullet.SetBulletSpeed(2f);
            bulletPool.Add(bullet);
            go.SetActive(false);
        }
    }

    public EnemyBullet RequestBullet()
    {
        if (bulletPool.Count <= 0) GenerateMoreBullets();
        EnemyBullet bullet = bulletPool[0];
        bulletPool.RemoveAt(0);
        return bullet;
    }

    public void ReturnBullet(EnemyBullet bullet)
    {
        bulletPool.Add(bullet);
    }
    public void ReturnBullet(List<EnemyBullet> bullets)
    {
        bulletPool = bulletPool.Concat(bullets).ToList();
    }
}
