using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public MenuAnimator mainMenu;
    public MenuAnimator settingsMenu;
    public MenuAnimator pauseMenu;

    bool transitioning;

    // We use this to find what is currently open
    private MenuAnimator GetActiveMenu()
    {
        if (pauseMenu != null && pauseMenu.gameObject.activeInHierarchy) return pauseMenu;
        if (mainMenu != null && mainMenu.gameObject.activeInHierarchy) return mainMenu;
        return null;
    }

    public void OpenSettings()
    {
        if (transitioning) return;
        
        MenuAnimator current = GetActiveMenu();
        if (current != null)
            StartCoroutine(SwitchMenu(current, settingsMenu));
        else
            // Fallback: If no menu is detected, just force settings open
            settingsMenu.Open();
    }

    public void CloseSettings()
    {
        if (transitioning) return;

        // If pauseMenu exists in scene, return there. Otherwise Main Menu.
        // Check if the object exists, NOT if it is currently active
MenuAnimator target = (pauseMenu != null) ? pauseMenu : mainMenu;
        if (target != null)
            StartCoroutine(SwitchMenu(settingsMenu, target));
        else
            settingsMenu.Close();
    }

    IEnumerator SwitchMenu(MenuAnimator from, MenuAnimator to)
    {
        transitioning = true;

        // 1. Tell the current menu to start disappearing
        // We call the Coroutine but we don't 'yield' it if we fear the object will vanish
        if (from != null && from.gameObject.activeInHierarchy)
        {
            // Instead of yield return StartCoroutine(from.DisappearSequence()), 
            // we calculate the wait time manually so the UIManager stays alive.
            from.Close(); 
            
            // Wait for the duration of the close animation + a tiny buffer
            // Use Realtime because the game is paused!
            yield return new WaitForSecondsRealtime(from.closeDuration + 0.1f);
        }

        // 2. Now open the new menu
        if (to != null)
        {
            to.Open();
            Debug.Log("UIManager: Successfully opened " + to.gameObject.name);
        }

        transitioning = false;
    }

    public void OpenMainMenu()
    {
        if (transitioning) return;
        StartCoroutine(SwitchMenu(settingsMenu, mainMenu));
    }

    

    public void ResumeGame()
    {
        pauseMenu.Close();
        Time.timeScale = 1f;
    }

    public void ExitToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("quit");
        Application.Quit();
    }

}
