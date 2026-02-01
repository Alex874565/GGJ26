using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int currentLevelIndex;
    public float checkpointX;
    public float checkpointY;
    public float checkpointZ;
    public int playerHealth;
    public int potionCount;

    public SaveData()
    {
        currentLevelIndex = 0;
        checkpointX = 0;
        checkpointY = 0;
        checkpointZ = 0;
        playerHealth = 0;
        potionCount = 0;
    }

    public SaveData(int levelIndex, Vector3 checkpointPosition, int health, int potions)
    {
        currentLevelIndex = levelIndex;
        checkpointX = checkpointPosition.x;
        checkpointY = checkpointPosition.y;
        checkpointZ = checkpointPosition.z;
        playerHealth = health;
        potionCount = potions;
    }

    public Vector3 GetCheckpointPosition()
    {
        return new Vector3(checkpointX, checkpointY, checkpointZ);
    }
}
