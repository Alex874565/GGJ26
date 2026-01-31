using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : MonoBehaviour
{
    public PlayableDirector director;

    private PlayerManager _playerManager;

    void Start()
    {
        _playerManager = ServiceLocator.Instance.PlayerManager;
    }
    
    public void Pause()
    {
        director.Pause();
    }

    void ChangeTutorialStep()
    {
        Debug.Log("Tutorial Step");
        director.Resume();
        _playerManager.OnJump -= ChangeTutorialStep;
    }
    
    public void AskForMovement()
    {
        director.Pause();
        _playerManager.OnMove += ChangeTutorialStep;
    }

    public void AskForJump()
    {
        director.Pause();
        _playerManager.OnJump += ChangeTutorialStep;
    }

    public void AskForDoubleJump()
    {
        director.Pause();
        _playerManager.OnJump += ChangeTutorialStep;
    }

    public void AskForDash()
    {
        director.Pause();
        _playerManager.OnDash += ChangeTutorialStep;
    }
    
}
