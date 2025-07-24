using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerBaseState : State
{
    protected PlayerStateMachine _stateMachine;

    // Constructor
    public PlayerBaseState(PlayerStateMachine stateMachine)
    {
        this._stateMachine = stateMachine;
    }

    // Moves the player
    protected void Move(float acel, float decel, Vector2 moveInput, float fixedDeltaTime)
    {
        if (moveInput != Vector2.zero)
        {
            TurnCheck(moveInput);

            float targetSpeed = InputManager.instance.IsRunning
                ? _stateMachine._stats.maxRunSpeed
                : _stateMachine._stats.maxWalkSpeed;
            _stateMachine.currentSpeed = Mathf.Lerp(
                _stateMachine.currentSpeed,
                targetSpeed * moveInput.x,
                acel * fixedDeltaTime
            );
        }
        else
        {
            _stateMachine.currentSpeed = Mathf.Lerp(
                _stateMachine.currentSpeed,
                0,
                decel * fixedDeltaTime
            );
        }

        _stateMachine._rb.linearVelocity = new Vector2(
            _stateMachine.currentSpeed,
            _stateMachine._rb.linearVelocityY
        );
    }

    // Check if we need to turn
    protected void TurnCheck(Vector2 moveInput)
    {
        if (_stateMachine.isFacingRight && moveInput.x < 0)
            Turn(false);
        if (!_stateMachine.isFacingRight && moveInput.x > 0)
            Turn(true);
    }

    // Turns the player
    private void Turn(bool turnRight)
    {

        _stateMachine.isFacingRight = turnRight;
        Vector3 newScale = _stateMachine.transform.localScale;
        newScale.x = turnRight ? Mathf.Abs(newScale.x) : -Mathf.Abs(newScale.x);
        _stateMachine.transform.localScale = newScale;
    }

    #region Collision Checks

    protected void CollisionCheck()
    {
        IsGrounded();
        NearbyColliders();
    }

    private void IsGrounded()
    {
        Vector2 boxCastOrigins = new Vector2(
            _stateMachine._feetCol.bounds.center.x,
            _stateMachine._feetCol.bounds.min.y
        );
        Vector2 boxCastSize = new Vector2(
            _stateMachine._feetCol.bounds.size.x,
            _stateMachine._stats.groundDetectionRayLength
        );

        _stateMachine._groundHit = Physics2D.BoxCast(
            boxCastOrigins,
            boxCastSize,
            0f,
            Vector2.down,
            _stateMachine._stats.groundDetectionRayLength,
            _stateMachine._stats.groundLayer
        );

        if (_stateMachine._groundHit.collider != null)
            _stateMachine._isGrounded = true;
        else
            _stateMachine._isGrounded = false;
    }

    List<Collider2D> nearbyColliders = new List<Collider2D>();

    private void NearbyColliders()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(
            _stateMachine.transform.position,
            _stateMachine.interactRadius
        );
        List<Collider2D> currentFrameColliders = new List<Collider2D>(hitColliders);

        foreach (Collider2D col in currentFrameColliders)
        {
            if (!nearbyColliders.Contains(col))
                nearbyColliders.Add(col);

            if (col.TryGetComponent<IInteractable>(out IInteractable interact))
            {
                col.GetComponent<SpriteRenderer>()
                    .material.SetVector(
                        "_OutlineThickness",
                        new Vector4(0.025f, 0.025f, -0.025f, -0.025f)
                    );
            }
        }

        for (int i = nearbyColliders.Count - 1; i >= 0; i--)
        {
            Collider2D col = nearbyColliders[i];
            if (!currentFrameColliders.Contains(col))
            {
                if (col != null && col.TryGetComponent<IInteractable>(out IInteractable interact))
                {
                    // Reset outline thickness before removing
                    col.GetComponent<SpriteRenderer>()
                        .material.SetVector("_OutlineThickness", Vector4.zero);
                }

                nearbyColliders.RemoveAt(i);
            }
        }
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
    #endregion
}
