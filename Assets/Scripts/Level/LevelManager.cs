using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Level[] _levels;
    public UnityEvent finalLevelLeft;
    [SerializeField] private PlayerManager _playerManager; //PlayerManager is player
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < _levels.Length-1; i++)
        {
            int index = i; // Capture the current index for the lambda
            _levels[i].playerLeft.AddListener(() => 
            {
                OnlyEnableLevel(index + 1);
                _playerManager.gameObject.transform.position = _levels[index + 1].spawnPoint.transform.position;
                Debug.Log("Player moved to level " + (index + 1));
            });
        }

        _levels[_levels.Length-1].playerLeft.AddListener(() => 
        {
            finalLevelLeft.Invoke();
            Debug.Log("Final level left!");
        });
    }

    void OnlyEnableLevel(int levelIndex)
    {
        for (int i = 0; i < _levels.Length; i++)
        {
            _levels[i].gameObject.SetActive(i == levelIndex);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
