public class PlayerState
{
    protected PlayerMovement playerMovement;
    protected PlayerCombat playerCombat;

    public PlayerState(PlayerMovement _playerMovement, PlayerCombat _playerCombat)
    {
        playerMovement = _playerMovement;
        playerCombat = _playerCombat;
    }
    virtual public void Enter()
    {
        return;
    }

    virtual public void Update()
    {
        return;
    }

    virtual public void FixedUpdate()
    {
        return;
    }

    virtual public void Exit()
    {
        return;
    }
}