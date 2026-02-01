using UnityEngine;

public class MaskMenuController : MonoBehaviour
{
    public MenuAnimator maskMenuAnimator; // Assign the MenuAnimator on your Radial Menu
    private bool isMenuOpen = false;

    void Start()
    {
        // Ensure menu is hidden at start
        if (maskMenuAnimator != null)
            maskMenuAnimator.gameObject.SetActive(false);
    }

    void Update()
    {
        // Toggle Mask - WasPressed
        if (InputManager.ToggleMaskWasPressed)
        {
            Open();
        }

        // Toggle Mask - WasReleased (Optional: if you want it to close when letting go of the key)
        if (InputManager.ToggleMaskWasReleased)
        {
            Close();
        }
    }

    public void Open()
    {
        isMenuOpen = true;
        maskMenuAnimator.Open();
        // Usually radial menus slow down time rather than fully pausing
        Time.timeScale = 0.2f; 
    }

    public void Close()
    {
        if (!isMenuOpen) return;
        
        isMenuOpen = false;
        maskMenuAnimator.Close();
        Time.timeScale = 1f;
    }
}