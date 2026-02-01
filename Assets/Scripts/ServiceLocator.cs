using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    public static ServiceLocator Instance => _instance;
    private static ServiceLocator _instance;
    
    public PlayerManager PlayerManager => _playerManager;
    public LevelManager LevelManager => _levelManager;
    public SaveManager SaveManager => _saveManager;
    
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private SaveManager _saveManager;
    
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }
    }
}