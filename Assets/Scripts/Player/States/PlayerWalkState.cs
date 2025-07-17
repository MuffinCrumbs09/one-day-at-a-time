using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStateMachine stateMachine)
        : base(stateMachine) { }

    private readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
    private float moveDir;

    public override void Enter() { }

    public override void Tick(float deltaTime)
    {
        _stateMachine.NearbyColliders();
        
        if (InputManager.instance.MovementValue.magnitude <= 0.1f)
            _stateMachine.SwitchState(new PlayerIdleState(_stateMachine));
        moveDir = InputManager.instance.MovementValue.x;

        Debug.Log(moveDir);

        if (_stateMachine.isFacingRight && moveDir < 0f)
            Flip(false);
        else if (!_stateMachine.isFacingRight && moveDir > 0f)
            Flip(true);

        Move(moveDir * (_stateMachine.walkSpeed * 100), deltaTime);

        _stateMachine._anim.SetFloat(MoveSpeedHash, 1f);
    }
}
