using UnityEngine;
using UnityEngine.Events;

public class Checkpoint : MonoBehaviour
{
    public UnityEvent _checkpointReached;

    [Tooltip("If true, entering this checkpoint will save the game")]
    [SerializeField] private bool _saveOnEnter = true;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _checkpointReached.Invoke();

            if (_saveOnEnter && ServiceLocator.Instance != null)
            {
                // Update checkpoint position and save
                if (ServiceLocator.Instance.LevelManager != null)
                {
                    ServiceLocator.Instance.LevelManager.UpdateCheckpoint(transform.position);
                }
                
                if (ServiceLocator.Instance.SaveManager != null)
                {
                    ServiceLocator.Instance.SaveManager.Save();
                }
            }
        }
    }
}
