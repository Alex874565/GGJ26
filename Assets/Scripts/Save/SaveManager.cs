using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;

public class SaveManager : MonoBehaviour
{
    private const string SAVE_FILE_NAME = "savegame.json";

    /// <summary>
    /// Save data stored when loading from main menu. Applied when entering level.
    /// </summary>
    private static SaveData s_pendingLoadData;

    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private PlayerCombat _playerCombat;

    private string SaveFilePath => Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);

    private void Start()
    {
        if (HasLevelReferences)
        {
            StartCoroutine(TryApplyPendingLoadNextFrame());
        }
    }

    private bool HasLevelReferences =>
        _levelManager != null && _playerManager != null && _playerCombat != null;

    private IEnumerator TryApplyPendingLoadNextFrame()
    {
        yield return null;
        TryApplyPendingLoad();
    }

    /// <summary>
    /// Checks if a save file exists. Safe to call from main menu (no level refs needed).
    /// </summary>
    public bool HasSaveFile()
    {
        return File.Exists(SaveFilePath);
    }

    /// <summary>
    /// Deletes the save file if it exists. Safe to call from main menu.
    /// </summary>
    public void DeleteSave()
    {
        if (File.Exists(SaveFilePath))
        {
            File.Delete(SaveFilePath);
            s_pendingLoadData = null;
            Debug.Log("Save file deleted.");
        }
    }

    /// <summary>
    /// Loads the game state. From main menu: stores data for when level loads.
    /// In level: applies immediately to LevelManager/PlayerManager/PlayerCombat.
    /// </summary>
    /// <returns>True if load succeeded, false if no save file.</returns>
    public bool Load()
    {
        if (!File.Exists(SaveFilePath))
        {
            Debug.LogWarning("No save file found!");
            return false;
        }

        string json = File.ReadAllText(SaveFilePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        if (HasLevelReferences)
        {
            ApplySaveData(data);
            Debug.Log($"Game loaded from {SaveFilePath}");
        }
        else
        {
            s_pendingLoadData = data;
            Debug.Log($"Save data prepared for level load from {SaveFilePath}");
        }

        return true;
    }

    /// <summary>
    /// Loads save data and transitions to the game scene. Call from main menu Continue button.
    /// The loaded level will apply the save data on start.
    /// </summary>
    /// <param name="gameSceneName">Name of the game/level scene to load (e.g. "SampleScene")</param>
    /// <returns>True if save existed and scene will load, false if no save file.</returns>
    public bool LoadAndContinue(string gameSceneName)
    {
        if (!Load())
            return false;

        SceneManager.LoadScene(gameSceneName);
        return true;
    }

    /// <summary>
    /// Applies pending save data when entering level from main menu Continue.
    /// Called automatically on level start. Can be called manually if needed.
    /// </summary>
    /// <returns>True if pending data was applied.</returns>
    public bool TryApplyPendingLoad()
    {
        if (s_pendingLoadData == null || !HasLevelReferences)
            return false;

        ApplySaveData(s_pendingLoadData);
        s_pendingLoadData = null;
        Debug.Log("Applied pending save data.");
        return true;
    }

    private void ApplySaveData(SaveData data)
    {
        _levelManager.SetCurrentLevel(data.currentLevelIndex);
        _levelManager.SetCheckpointPosition(data.GetCheckpointPosition());
        _playerManager.SetHealth(data.playerHealth);
        _playerCombat.SetPotionCount(data.potionCount);
        _playerManager.transform.position = data.GetCheckpointPosition();
    }

    /// <summary>
    /// Saves the current game state to a file. Requires level references (only works in level).
    /// </summary>
    public void Save()
    {
        if (!HasLevelReferences)
        {
            Debug.LogWarning("SaveManager: Cannot save - level references not set.");
            return;
        }

        SaveData data = new SaveData(
            _levelManager.CurrentLevelIndex,
            _levelManager.CurrentCheckpointPosition,
            _playerManager.CurrentHealth,
            _playerCombat.PotionCount
        );

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SaveFilePath, json);

        Debug.Log($"Game saved to {SaveFilePath}");
    }
}
