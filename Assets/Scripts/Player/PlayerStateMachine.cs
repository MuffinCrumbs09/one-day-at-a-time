using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    public static PlayerStateMachine instance { private set; get; }

    private void Awake()
    {
        // If no instance, this becomes instance
        if (instance != null && instance != this)
            Destroy(this);

        instance = this;
    }

    #region Components
    public Rigidbody2D _rb { private set; get; }
    public Animator _anim { private set; get; }
    #endregion

    #region PlayerMovementState
    [Header("PlayerMovement State")]
    [field: SerializeField]
    public PlayerMovementStats _stats { set; get; }

    [field: SerializeField]
    public Collider2D _feetCol { set; get; }

    [field: SerializeField]
    public Collider2D _bodyCol { set; get; }
    public bool isFacingRight { set; get; } = true;
    public float currentSpeed { set; get; } = 0f;
    #endregion

    #region Collision Checks
    public RaycastHit2D _groundHit { set; get; }
    public RaycastHit2D _headHit { set; get; }
    public bool _isGrounded { set; get; }
    public bool _bumpedHead { set; get; }
    #endregion

    #region Visible
    [field: SerializeField]
    public float interactRadius { get; set; }
    #endregion

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();

        SwitchState(new PlayerIdleState(this));
    }

}
