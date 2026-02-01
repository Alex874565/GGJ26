using UnityEngine;
using UnityEngine.Events;

public class Level : MonoBehaviour
{
    public int levelIndex;
    public UnityEvent playerEntered;
    public UnityEvent playerLeft;
    public Checkpoint spawnPoint;
    public Checkpoint endPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnPoint._checkpointReached.AddListener(() => playerEntered.Invoke());
        endPoint._checkpointReached.AddListener(() => playerLeft.Invoke());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
