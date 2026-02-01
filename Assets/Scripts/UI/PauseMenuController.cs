using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    public MenuAnimator pauseMenu;  // assign your PauseUI MenuAnimator
    public UIManager uiManager;     // optional, for Resume/Exit buttons

    private bool isPaused = false;

    void Start()
    {
        // Force the menu to be closed at the very start
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenu.gameObject.SetActive(false); 
    }

    private float lastToggleTime;
private float toggleCooldown = 0.2f;

void Update()
{
    if (InputManager.CancelWasPressed && Time.unscaledTime > lastToggleTime + toggleCooldown)
    {
        lastToggleTime = Time.unscaledTime;
        
        if (isPaused)
            Resume();
        else
            Pause();
    }
}

    public void Pause()
    {
        
        isPaused = true;
        Time.timeScale = 0f;    // pause the game
        pauseMenu.Open();
    }

    public void Resume()
{
    // 1. Check if the menu is even active before trying to close it
    if (!pauseMenu.gameObject.activeInHierarchy) return;

    isPaused = false;
    Time.timeScale = 1f;
    
    // 2. Call close
    pauseMenu.Close();
}
}
