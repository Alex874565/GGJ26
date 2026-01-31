using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public MenuAnimator mainMenu;
    public MenuAnimator settingsMenu;
    public MenuAnimator pauseMenu;

    bool transitioning;

    void Start()
    {
        mainMenu.Open();
        settingsMenu.gameObject.SetActive(false);
    }

    public void OpenSettings()
    {
        if (transitioning) return;
        StartCoroutine(SwitchMenu(mainMenu, settingsMenu));
    }

    public void OpenMainMenu()
    {
        if (transitioning) return;
        StartCoroutine(SwitchMenu(settingsMenu, mainMenu));
    }

    IEnumerator SwitchMenu(MenuAnimator from, MenuAnimator to)
    {
        transitioning = true;

        yield return StartCoroutine(from.DisappearSequence()); // wait fully

        to.Open(); // now safe
        transitioning = false;
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
