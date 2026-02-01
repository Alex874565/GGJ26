using UnityEngine;
using UnityEngine.InputSystem;

public class PauseUI : MonoBehaviour
{
    public MenuAnimator menuAnimator; // assign the MenuAnimator
    public PlayerInput playerInput;   // assign your InputManager's PlayerInput here

    private InputAction cancelAction;
    private bool isPaused = false;

    void Awake()
    {
        gameObject.SetActive(false); // ensure starts hidden

        if (playerInput != null)
            cancelAction = playerInput.actions["Cancel"];
    }

    void OnEnable()
    {
        if (cancelAction != null)
            cancelAction.performed += OnCancel;
    }

    void OnDisable()
    {
        if (cancelAction != null)
            cancelAction.performed -= OnCancel;
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    void Update()
    {
        if (InputManager.PlayerInput != null &&
            InputManager.PlayerInput.actions["Cancel"].WasPerformedThisFrame())
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;   // freeze game
        menuAnimator.Open();    // only now the object becomes active
    }


    public void Resume()
    {
        isPaused = false;
        menuAnimator.Close();
        Time.timeScale = 1f;
    }

    // Optional: buttons
    public void ResumeButton() => Resume();
    public void ExitToMainMenu() => UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    public void QuitGame() => Application.Quit();
}
