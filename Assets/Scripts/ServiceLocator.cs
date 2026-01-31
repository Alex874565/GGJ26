using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    public static ServiceLocator Instance => _instance;
    private static ServiceLocator _instance;
    
    public PlayerManager PlayerManager => _playerManager;
    
    [SerializeField] private PlayerManager _playerManager;
    
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