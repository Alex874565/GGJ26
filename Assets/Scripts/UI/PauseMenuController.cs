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
        var go = GameObject.Find("PauseUI");
        if (go != null)
            pauseMenu = go.GetComponent<MenuAnimator>();
    }
}

    void Start()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pauseMenu != null)
            pauseMenu.gameObject.SetActive(false);
    }

    private float lastToggleTime;
private float toggleCooldown = 0.2f;



private float nextActionTime = 0f;

void Update()
{
    bool pausePressed = InputManager.PauseWasPressed || InputManager.CancelWasPressed;
    if (pausePressed && Time.unscaledTime > nextActionTime && pauseMenu != null)
    {
        nextActionTime = Time.unscaledTime + 0.2f;

        if (isPaused)
            Resume();
        else
            Pause();
    }
}

public void Pause()
{
    if (pauseMenu == null || isPaused) return;
    
    isPaused = true;
    
    // Ensure pause menu fills the screen (fixes off-screen when parent canvas differs)
    if (pauseMenu.TryGetComponent<RectTransform>(out var rt))
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
    
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
    if (pauseMenu != null)
        pauseMenu.Close();
}
}
