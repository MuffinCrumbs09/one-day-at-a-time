using UnityEngine;

public class PlayerBaseState : State
{
    protected PlayerStateMachine _stateMachine;

    // Constructor
    public PlayerBaseState(PlayerStateMachine stateMachine)
    {
        this._stateMachine = stateMachine;
    }

    // Moves the player
    protected void Move(float horizontal, float deltaTime)
    {
        _stateMachine._rb.linearVelocity = new Vector2(
            horizontal * deltaTime,
            _stateMachine._rb.linearVelocity.y
        );
    }

    // Interact with nearby interactables
    protected void Interact()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(
            _stateMachine.transform.position,
            _stateMachine.interactRadius
        );
        foreach (Collider2D col in hitColliders)
        {
            if (col.TryGetComponent<IInteractable>(out IInteractable interact))
            {
                interact.Interact();
            }
        }
    }

    protected void Flip(bool isRight)
    {
        _stateMachine.isFacingRight = isRight;
        Vector3 localScale = _stateMachine.transform.localScale;
        localScale.x = isRight ? Mathf.Abs(localScale.x) : -Mathf.Abs(localScale.x);
        _stateMachine.transform.localScale = localScale;
    }
}
