using UnityEngine;

public class MoveGun : MonoBehaviour
{
    private Transform _player;
    // Start is called before the first frame update
    void Start()
    {
        _player = FindObjectOfType<Camera>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(_player);
        
    }
}
