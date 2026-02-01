using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    private const string SAVE_FILE_NAME = "savegame.json";
    
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private PlayerCombat _playerCombat;

    private string SaveFilePath => Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);

    void Awake()
    {
        Debug.Log("Save exists: " + HasSaveFile());
    }

    /// <summary>
    /// Saves the current game state to a file.
    /// </summary>
    public void Save()
    {
        SaveData data = new SaveData(
            _levelManager.CurrentLevelIndex,
            _levelManager.CurrentCheckpointPosition,
            _playerManager.CurrentHealth,
            _playerCombat.PotionCount
        );

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SaveFilePath, json);
        
        Debug.Log($"Game saved to {SaveFilePath}");

        FindFirstObjectByType<MainMenuUI>()?.Refresh();


    }

    /// <summary>
    /// Loads the game state from a file.
    /// </summary>
    public void Load()
    {
        if (!File.Exists(SaveFilePath))
        {
            Debug.LogWarning("No save file found!");
            return;
        }

        string json = File.ReadAllText(SaveFilePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // Apply loaded data
        _levelManager.SetCurrentLevel(data.currentLevelIndex);
        _levelManager.SetCheckpointPosition(data.GetCheckpointPosition());
        _playerManager.SetHealth(data.playerHealth);
        _playerCombat.SetPotionCount(data.potionCount);
        
        // Move player to checkpoint position
        _playerManager.transform.position = data.GetCheckpointPosition();

        Debug.Log($"Game loaded from {SaveFilePath}");
    }

    /// <summary>
    /// Checks if a save file exists.
    /// </summary>
    public bool HasSaveFile()
    {
        return File.Exists(SaveFilePath);
    }

    /// <summary>
    /// Deletes the save file if it exists.
    /// </summary>
    public void DeleteSave()
    {
        if (File.Exists(SaveFilePath))
        {
            File.Delete(SaveFilePath);
            Debug.Log("Save file deleted.");
        }

        FindFirstObjectByType<MainMenuUI>()?.Refresh();


    }
}
