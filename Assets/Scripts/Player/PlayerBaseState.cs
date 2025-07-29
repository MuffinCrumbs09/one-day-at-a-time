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

    protected void Jump()
    {
        // Apply Gravity While Jumping
        if (_stateMachine.isJumping)
        {
            // Check For Head Bump
            if (_stateMachine._bumpedHead)
            {
                _stateMachine.isFastFalling = true;
            }

            // Ascending Gravity
            if (_stateMachine.verticalVelocity >= 0f)
            {
                // Apex Controls
                _stateMachine.apexPoint = Mathf.InverseLerp(
                    _stateMachine._stats.intialJumpVelocity,
                    0f,
                    _stateMachine.verticalVelocity
                );

                if (_stateMachine.apexPoint > _stateMachine._stats.apexThreshold)
                {
                    if (!_stateMachine.isPastApexPoint)
                    {
                        _stateMachine.isPastApexPoint = true;
                        _stateMachine.timePastApexPoint = 0f;
                    }

                    if (_stateMachine.isPastApexPoint)
                    {
                        _stateMachine.timePastApexPoint += Time.fixedDeltaTime;
                        if (_stateMachine.timePastApexPoint < _stateMachine._stats.apexHangTime)
                        {
                            _stateMachine.verticalVelocity = 0f;
                        }
                        else
                        {
                            _stateMachine.verticalVelocity = -0.01f;
                        }
                    }
                }
                // Gravity On Ascending But Not Past Apex Threshold
                else
                {
                    _stateMachine.verticalVelocity +=
                        _stateMachine._stats.gravity * Time.fixedDeltaTime;
                    if (_stateMachine.isPastApexPoint)
                        _stateMachine.isPastApexPoint = false;
                }
            }
            // Decending Gravity
            else if (!_stateMachine.isFastFalling)
            {
                _stateMachine.verticalVelocity +=
                    _stateMachine._stats.gravity
                    * _stateMachine._stats.gravityOnReleaseMultipler
                    * Time.fixedDeltaTime;
            }
            else if (_stateMachine.verticalVelocity < 0f)
            {
                if (!_stateMachine.isFalling)
                    _stateMachine.isFalling = true;
            }
        }

        // Jump Cut
        if (_stateMachine.isFastFalling)
        {
            if (_stateMachine.fastFallTime >= _stateMachine._stats.timeForUpCancel)
            {
                _stateMachine.verticalVelocity +=
                    _stateMachine._stats.gravity * _stateMachine._stats.gravityOnReleaseMultipler;
            }
            else if (_stateMachine.fastFallTime < _stateMachine._stats.timeForUpCancel)
            {
                _stateMachine.verticalVelocity = Mathf.Lerp(
                    _stateMachine.fastFallReleaseSpeed,
                    0f,
                    (_stateMachine.fastFallTime / _stateMachine._stats.timeForUpCancel)
                );
            }

            _stateMachine.fastFallTime += Time.fixedDeltaTime;
        }

        // Normal Gravity
        if (!_stateMachine._isGrounded && !_stateMachine.isJumping)
        {
            if (!_stateMachine.isFalling)
            {
                _stateMachine.isFalling = true;
            }

            _stateMachine.verticalVelocity += _stateMachine._stats.gravity * Time.fixedDeltaTime;
        }

        // Clamp Fall Speed
        _stateMachine.verticalVelocity = Mathf.Clamp(_stateMachine.verticalVelocity, -_stateMachine._stats.maxFallSpeed, 50f);

        _stateMachine._rb.linearVelocity = new Vector2(_stateMachine._rb.linearVelocityX, _stateMachine.verticalVelocity);
    }

    protected void JumpChecks()
    {
        // Press Jump Button
        if (InputManager.instance.JumpWasPressed)
        {
            _stateMachine.jumpBufferTime = _stateMachine._stats.jumpBufferTime;
            _stateMachine.jumpReleaseDuringBuffer = false;
        }
        // Release Jump Button
        if (InputManager.instance.JumpWasReleased)
        {
            if (_stateMachine.jumpBufferTime > 0f)
                _stateMachine.jumpReleaseDuringBuffer = true;

            if (_stateMachine.isJumping && _stateMachine.verticalVelocity >= 0f)
                if (_stateMachine.isPastApexPoint)
                {
                    _stateMachine.isPastApexPoint = false;
                    _stateMachine.isFastFalling = true;
                    _stateMachine.fastFallTime = _stateMachine._stats.timeForUpCancel;
                    _stateMachine.verticalVelocity = 0f;
                }
                else
                {
                    _stateMachine.isFastFalling = true;
                    _stateMachine.fastFallReleaseSpeed = _stateMachine.verticalVelocity;
                }
        }

        // Intitate Jump With Buffer And Coyote Time
        if (
            _stateMachine.jumpBufferTime > 0f
            && !_stateMachine.isJumping
            && (_stateMachine._isGrounded || _stateMachine.coyoteTime > 0f)
        )
        {
            InitJump(1);

            if (_stateMachine.jumpReleaseDuringBuffer)
            {
                _stateMachine.isFastFalling = true;
                _stateMachine.fastFallReleaseSpeed = _stateMachine.verticalVelocity;
            }
        }
        // Double Jump
        else if (
            _stateMachine.jumpBufferTime > 0f
            && _stateMachine.isJumping
            && _stateMachine.numberOfJumpsUsed < _stateMachine._stats.maxJumps - 1
        )
        {
            _stateMachine.isFastFalling = false;
            InitJump(1);
        }
        // Air Jump After Coyote
        else if (
            _stateMachine.jumpBufferTime > 0f
            && _stateMachine.isFalling
            && _stateMachine.numberOfJumpsUsed < _stateMachine._stats.maxJumps
        )
        {
            InitJump(2);
            _stateMachine.isFastFalling = false;
        }
        // Land
        if (
            (_stateMachine.isJumping || _stateMachine.isFalling)
            && _stateMachine._isGrounded
            && _stateMachine.verticalVelocity <= 0f
        )
        {
            _stateMachine.isJumping = false;
            _stateMachine.isFalling = false;
            _stateMachine.isFastFalling = false;
            _stateMachine.fastFallTime = 0f;
            _stateMachine.isPastApexPoint = false;
            _stateMachine.numberOfJumpsUsed = 0;

            _stateMachine.verticalVelocity = Physics2D.gravity.y;
        }
    }

    private void InitJump(int numberOfJumpsUsed)
    {
        if (!_stateMachine.isJumping)
            _stateMachine.isJumping = true;

        _stateMachine.jumpBufferTime = 0f;
        _stateMachine.numberOfJumpsUsed += numberOfJumpsUsed;
        _stateMachine.verticalVelocity = _stateMachine._stats.intialJumpVelocity;
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

    #region Timers
    protected void CountTimers()
    {
        _stateMachine.jumpBufferTime -= Time.deltaTime;

        if (!_stateMachine._isGrounded)
            _stateMachine.coyoteTime -= Time.deltaTime;
        else
            _stateMachine.coyoteTime = _stateMachine._stats.jumpCoyoteTime;
    }
    #endregion

    #region Collision Checks

    protected void CollisionCheck()
    {
        IsGrounded();
        BumpedHead();
        NearbyColliders();
    }

    private void BumpedHead()
    {
        Vector2 boxCastOrigin = new Vector2(
            _stateMachine._feetCol.bounds.center.x,
            _stateMachine._feetCol.bounds.max.y
        );
        Vector2 boxCastSize = new Vector2(
            _stateMachine._feetCol.bounds.size.x * _stateMachine._stats.headWidth,
            _stateMachine._stats.headDetectionRayLength
        );

        _stateMachine._headHit = Physics2D.BoxCast(
            boxCastOrigin,
            boxCastSize,
            0f,
            Vector2.up,
            _stateMachine._stats.headDetectionRayLength,
            _stateMachine._stats.groundLayer
        );
        if (_stateMachine._headHit.collider != null)
        {
            _stateMachine._bumpedHead = true;
        }
        else
        {
            _stateMachine._bumpedHead = false;
        }
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
    #endregion
}
