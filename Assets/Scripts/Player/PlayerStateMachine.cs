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

    #region Visible
    [field: SerializeField]
    public float walkSpeed { get; private set; }

    [field: SerializeField]
    public float interactRadius { get; set; }
    #endregion
    public bool isFacingRight { get; set; }
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();

        SwitchState(new PlayerIdleState(this));
    }

    List<Collider2D> nearbyColliders = new List<Collider2D>();

    public void NearbyColliders()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, interactRadius);
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
}
