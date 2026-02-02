using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Level[] _levels;
    public UnityEvent finalLevelLeft;
    [SerializeField] private PlayerManager _playerManager; //PlayerManager is player

    private int _currentLevelIndex = 0;
    private Vector3 _currentCheckpointPosition;

    public int CurrentLevelIndex => _currentLevelIndex;
    public Vector3 CurrentCheckpointPosition => _currentCheckpointPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize checkpoint position to first level's spawn point
        if (_levels.Length > 0 && _levels[0].spawnPoint != null)
        {
            _currentCheckpointPosition = _levels[0].spawnPoint.transform.position;
        }
        for (int i = 0; i < _levels.Length-1; i++)
        {
            int index = i; // Capture the current index for the lambda
            _levels[i].playerLeft.AddListener(() => 
            {
                OnlyEnableLevel(index + 1);
                _currentLevelIndex = index + 1;
                _currentCheckpointPosition = _levels[index + 1].spawnPoint.transform.position;
                _playerManager.gameObject.transform.position = _currentCheckpointPosition;
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

    /// <summary>
    /// Sets the current level by index and enables only that level.
    /// </summary>
    public void SetCurrentLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= _levels.Length)
        {
            Debug.LogWarning($"Invalid level index: {levelIndex}");
            return;
        }

        _currentLevelIndex = levelIndex;
        OnlyEnableLevel(levelIndex);
    }

    /// <summary>
    /// Sets the current checkpoint position (used for save/load).
    /// </summary>
    public void SetCheckpointPosition(Vector3 position)
    {
        _currentCheckpointPosition = position;
    }

    /// <summary>
    /// Updates the checkpoint position when the player reaches a new checkpoint.
    /// Call this from checkpoint triggers.
    /// </summary>
    public void UpdateCheckpoint(Vector3 position)
    {
        _currentCheckpointPosition = position;
        Debug.Log($"Checkpoint updated to {position}");
    }

    /// <summary>
    /// Respawns the player at the last saved checkpoint (full health, teleport).
    /// </summary>
    public void RespawnAtCheckpoint()
    {
        _playerManager.transform.position = _currentCheckpointPosition;
        _playerManager.Respawn();
    }

    /// <summary>
    /// Advances to the next level. Call this when a boss is defeated or other progression triggers.
    /// </summary>
    public void AdvanceToNextLevel()
    {
        int nextIndex = _currentLevelIndex + 1;
        if (nextIndex >= _levels.Length)
        {
            finalLevelLeft?.Invoke();
            SceneManager.LoadScene("MainMenu");
            return;
        }

        OnlyEnableLevel(nextIndex);
        _currentLevelIndex = nextIndex;
        _currentCheckpointPosition = _levels[nextIndex].spawnPoint.transform.position;
        _playerManager.gameObject.transform.position = _currentCheckpointPosition;
        Debug.Log($"Advanced to level {nextIndex}");
    }
}
