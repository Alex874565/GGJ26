using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public MenuAnimator mainMenu;
    public MenuAnimator settingsMenu;

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

        from.Close();

        yield return new WaitForSeconds(from.closeDuration);

        to.Open();

        transitioning = false;
    }
}
