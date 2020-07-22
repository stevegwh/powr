using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class EnemyAI : MonoBehaviour
{
    public GameObject Player;

    public GameObject Nozzle;

    public GameObject Bullet;

    private float _bulletTimer;

    private readonly float bulletDelay = 1f;

    private int _bulletPool = 3;



    void Start()
    {
    }

    private void Fire()
    {
        GameObject go = Instantiate(Bullet, Nozzle.transform.position, Nozzle.transform.rotation);
        go.GetComponent<Bullet>().SetBulletLayer(12);
        go.GetComponent<Bullet>().SetBulletSpeed(2f);
        Destroy(go, 5f);
    }

    // Update is called once per frame
    void Update()
    {
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
