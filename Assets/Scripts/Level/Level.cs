using UnityEngine;
using UnityEngine.Events;

public class Level : MonoBehaviour
{
    [SerializeField] private MaskManager _maskManager;
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
        _maskManager.maskPicked.AddListener(OnMaskPicked);
    }

    void OnMaskPicked(MaskType maskType)
    {
        switch (maskType)
        {
            case MaskType.BASIC:
                Debug.Log("Basic mask picked in level " + levelIndex);
                break;
            case MaskType.SELFNESSNESS:
                Debug.Log("Selflessness mask picked in level " + levelIndex);
                break;
            case MaskType.HAPPINESS:
                Debug.Log("Happiness mask picked in level " + levelIndex);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
