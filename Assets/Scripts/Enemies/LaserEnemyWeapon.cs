using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEnemyWeapon : MonoBehaviour
{
    public GameObject Player;

    public GameObject Muzzle;

    public GameObject Bullet;

    private EnemyLaser laser;
    private void Fire()
    {
        GameObject go = Instantiate(Bullet, Muzzle.transform.position, Muzzle.transform.rotation);
        go.transform.parent = Muzzle.transform;
        laser = go.GetComponentInChildren<EnemyLaser>();
        // laser.SetMuzzle(Muzzle);
    }

    void Update()
    {
        transform.LookAt(Player.transform);

        if (laser == null)
        {
            Fire();
        }
        else
        {
            // laser.SetLaserStartPos(Muzzle.transform.position);
        }

    }
}
