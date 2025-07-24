using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine stateMachine)
        : base(stateMachine) { }

    private readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");

    public override void Enter()
    {
        InputManager.instance.InteractEvent += Interact;
    }

    public override void Tick(float deltaTime)
    {
        CollisionCheck();

        if (InputManager.instance.MovementValue.magnitude >= 0.1f)
            _stateMachine.SwitchState(new PlayerWalkState(_stateMachine));

        if (_stateMachine._isGrounded)
            Move(
                _stateMachine._stats.groundAcel,
                _stateMachine._stats.groundDecel,
                Vector2.zero,
                deltaTime
            );
        else
            Move(
                _stateMachine._stats.airAcel,
                _stateMachine._stats.airDecel,
                Vector2.zero,
                deltaTime
            );

        _stateMachine._anim.SetFloat(MoveSpeedHash, 0f);
    }

    public override void Exit()
    {
        InputManager.instance.InteractEvent -= Interact;
    }
}
