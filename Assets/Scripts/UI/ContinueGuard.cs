using UnityEngine;

public class ContinueGuard : MonoBehaviour
{
    public SaveManager saveManager;

    void OnEnable()
    {
        if (saveManager != null && !saveManager.HasSaveFile())
        {
            Debug.Log("ContinueGuard: forcing OFF");
            gameObject.SetActive(false);
        }
    }
}
