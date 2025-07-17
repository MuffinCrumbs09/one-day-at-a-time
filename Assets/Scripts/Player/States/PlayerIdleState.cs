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
        _stateMachine.NearbyColliders();
        
        if (InputManager.instance.MovementValue.magnitude >= 0.1f)
            _stateMachine.SwitchState(new PlayerWalkState(_stateMachine));

        _stateMachine._anim.SetFloat(MoveSpeedHash, 0f);
    }

    public override void Exit()
    {
        InputManager.instance.InteractEvent -= Interact;
    }
}
