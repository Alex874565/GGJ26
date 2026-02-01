using System.Collections;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public MenuAnimator mainMenu;
    public SaveManager saveManager;
    public MenuItemBase continueButton; // assign the Continue button here

    void Start()
    {
        UpdateContinueButton();
    }

    public void OpenMainMenu()
    {
        mainMenu.Open();
    }

    public void CloseMainMenu()
    {
        mainMenu.Close();
    }

    public void Refresh()
    {
        mainMenu.Close();
        StartCoroutine(Reopen());
    }

    IEnumerator Reopen()
    {
        yield return new WaitForSeconds(0.15f);
        mainMenu.Open();
    }

    void UpdateContinueButton()
    {
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(saveManager.HasSaveFile());
        }
    }

    // --- Button handlers ---
    public void OnNewGame()
    {
        // reset game state
        saveManager.Save(); // create initial save
        UpdateContinueButton(); // Continue now appears

        // optionally start the first level
        // SceneManager.LoadScene("Level1");
    }

    public void OnContinue()
    {
        if (!saveManager.HasSaveFile()) return;

        saveManager.Load();
        // load the saved level
        // SceneManager.LoadScene(savedLevelName);
    }
}
