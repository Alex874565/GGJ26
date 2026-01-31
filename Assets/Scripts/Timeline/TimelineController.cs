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
        director.Resume();
        _playerManager.OnMove -= ChangeTutorialStep;
        _playerManager.OnJump -= ChangeTutorialStep;
        _playerManager.OnDash -= ChangeTutorialStep;
        _playerManager.OnDoubleJump -= ChangeTutorialStep;
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
        _playerManager.OnDoubleJump += ChangeTutorialStep;
    }

    public void AskForDash()
    {
        director.Pause();
        _playerManager.OnDash += ChangeTutorialStep;
    }
    
}
