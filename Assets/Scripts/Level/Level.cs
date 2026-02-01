using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class Level : MonoBehaviour
{
    [SerializeField] private MaskManager _maskManager;
    [SerializeField] private Tilemap _tilemapSelflessness;
    [SerializeField] private Tilemap _tilemapHappiness;
    [SerializeField] private Tilemap _tilemapConfidence;
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
            case MaskType.SELFNESSNESS:
                _tilemapSelflessness.gameObject.SetActive(true);
                
                _tilemapHappiness.gameObject.SetActive(false);
                _tilemapConfidence.gameObject.SetActive(false);
                break;
            case MaskType.HAPPINESS:
                _tilemapHappiness.gameObject.SetActive(true);

                _tilemapSelflessness.gameObject.SetActive(false);
                _tilemapConfidence.gameObject.SetActive(false);
                break;
            case MaskType.CONFIDENCE:
                _tilemapConfidence.gameObject.SetActive(true);

                _tilemapSelflessness.gameObject.SetActive(false);
                _tilemapHappiness.gameObject.SetActive(false);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
