using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStateMachine stateMachine)
        : base(stateMachine) { }

    private readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");

    public override void Enter() { }

    public override void Tick(float deltaTime)
    {
        CollisionCheck();

        if (InputManager.instance.MovementValue.magnitude <= 0.1f)
            _stateMachine.SwitchState(new PlayerIdleState(_stateMachine));

        if (_stateMachine._isGrounded)
            Move(
                _stateMachine._stats.groundAcel,
                _stateMachine._stats.groundDecel,
                InputManager.instance.MovementValue,
                deltaTime
            );
        else
            Move(
                _stateMachine._stats.airAcel,
                _stateMachine._stats.airDecel,
                InputManager.instance.MovementValue,
                deltaTime
            );

        _stateMachine._anim.SetFloat(MoveSpeedHash, 1f);
    }
}
