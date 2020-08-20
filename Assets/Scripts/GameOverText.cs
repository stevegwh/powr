using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverText : MonoBehaviour
{
    public GameObject Player;
    private float posY;

    private Transform cachedTransform;
    // Start is called before the first frame update
    void Start()
    {
        posY = transform.position.y;
        cachedTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        cachedTransform.rotation = Quaternion.LookRotation(cachedTransform.position - Player.transform.position);
        // cachedTransform.position = (transform.position - Player.transform.position).normalized + Player.transform.position;
        // cachedTransform.position = new Vector3(cachedTransform.position.x, posY, cachedTransform.position.z);
        
    }
}
