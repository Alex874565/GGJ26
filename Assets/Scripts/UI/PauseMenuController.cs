using System.Collections;
using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    public MenuAnimator pauseMenu;  // assign your PauseUI MenuAnimator
    public UIManager uiManager;     // optional, for Resume/Exit buttons

    private bool isPaused = false;

    private void Awake()
{
    // This finds the PauseUI object actually living in your Hierarchy
    // Make sure your Pause UI object has the 'MenuAnimator' script on it!
    if (pauseMenu == null)
    {
        pauseMenu = GameObject.Find("PauseUI").GetComponent<MenuAnimator>();
    }
}

    void Start()
    {
        // Force the menu to be closed at the very start
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenu.gameObject.SetActive(false); 
    }

    private float lastToggleTime;
private float toggleCooldown = 0.2f;



private float nextActionTime = 0f;

void Update()
{
    if (InputManager.CancelWasPressed && Time.unscaledTime > nextActionTime)
    {
        nextActionTime = Time.unscaledTime + 0.2f; // Cooldown of 0.2 seconds

        if (isPaused)
            Resume();
        else
            Pause();
    }
}

public void Pause()
{
    if (isPaused) return;
    
    isPaused = true;
    
    // 1. Turn the menu ON
    pauseMenu.Open(); 

    // 2. Force the Canvas to calculate its layout and graphics IMMEDIATELY
    // This bypasses the need for the next frame
    Canvas.ForceUpdateCanvases();

    // 3. If you have a CanvasGroup, force its alpha to 1 just in case
    if (pauseMenu.TryGetComponent(out CanvasGroup group))
    {
        group.alpha = 1;
    }

    // 4. Freeze the game AFTER the graphics are forced to update
    Time.timeScale = 0f;
    
    Debug.Log("Pause UI forced to render before freezing.");
}

public void Resume()
{
    isPaused = false;
    Time.timeScale = 1f;
    pauseMenu.Close(); // Let the animation play
}
}
