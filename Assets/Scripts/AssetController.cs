using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.Extras;

public class AssetController : MonoBehaviour
{

    public PlaneType planeType;

    // The real-world plane that the asset is representing.
    public GameObject AssociatedPlane;

    public GameObject AssociatedPivotPoint;

    [SerializeField]
    private List<GameObject> enemies;

    // Iterate through generated planes and find the most appropriate one to pair with.
    // TODO: For the moment just chooses a random plane. Perhaps could be an algorithm to choose the plane closest to the actual height of the object etc.
    public void DecideBestPlane(List<GameObject> planes)
    {
        int randNum = Random.Range(0, planes.Count);
        AssociatedPlane = planes[randNum];
        Debug.Log(randNum);
    }

    void Start()
    {
        enemies = new List<GameObject>();
        Transform enemiesContainer = transform.Find("Enemies");
        if (enemiesContainer == null) return;
        for (int i = 0; i < enemiesContainer.childCount; i++)
        {
            GameObject enemy = enemiesContainer.GetChild(i).gameObject;
            enemy.GetComponent<EnemyAI>().enemyWaveController = this;
            enemies.Add(enemy);
            enemiesContainer.GetChild(i).gameObject.SetActive(false);
        }

    }

    // Ensures the enemies do not get scaled along with its parent asset
    public void PostScale()
    {
        Transform enemiesContainer = transform.Find("Enemies");
        enemiesContainer.transform.localScale = Vector3.one; // Normalise the enemy scale
    }
    void Awake()
    {
        TransitionShooterRoom.RegisterAssetToSpawn(gameObject);
    }

    public void StartEnemyWave()
    {
        foreach (var enemy in enemies)
        {
            enemy.SetActive(true);
        }

    }

    public void RemoveEnemy(GameObject toRemove)
    {
        enemies.Remove(toRemove);
        if (enemies.Count == 0) TransitionShooterRoom.EnableNextTeleportPoint();
        this.enabled = false;
    }



}
