using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public MenuAnimator mainMenu;

    public void OpenMainMenu()
    {
        mainMenu.Open();
    }

    public void CloseMainMenu()
    {
        mainMenu.Close();
    }
}
