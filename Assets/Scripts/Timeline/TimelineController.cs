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

        _playerManager.OnAttack -= ChangeTutorialStep;
        _playerManager.OnHeavyAttack -= ChangeTutorialStep;
        _playerManager.OnComboAttack -= ChangeTutorialStep;
        _playerManager.OnDashAttack -= ChangeTutorialStep;
        _playerManager.OnJumpAttack -= ChangeTutorialStep;
        _playerManager.OnParry -= ChangeTutorialStep;
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
    
    public void AskForAttack()
    {
        director.Pause();
        _playerManager.OnAttack += ChangeTutorialStep;
    }

    public void AskForHeavyAttack()
    {
        director.Pause();
        _playerManager.OnHeavyAttack += ChangeTutorialStep;
    }

    public void AskForComboAttack()
    {
        director.Pause();
        _playerManager.OnComboAttack += ChangeTutorialStep;
    }

    public void AskForDashAttack()
    {
        director.Pause();
        _playerManager.OnDashAttack += ChangeTutorialStep;
    }

    public void AskForJumpAttack()
    {
        director.Pause();
        _playerManager.OnJumpAttack += ChangeTutorialStep;
    }

    public void AskForParry()
    {
        director.Pause();
        _playerManager.OnParry += ChangeTutorialStep;
    }
}
