using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputManager : MonoBehaviour, Controls.IPlayerActions
{
    public static InputManager instance;
    private Controls _controls;

    #region Public Values
    public Vector2 MovementValue { private set; get; }
    #endregion

    #region Events
    public event Action InteractEvent;
    #endregion

    private void Awake()
    {
        // If no instance, this becomes instance
        if (instance != null && instance != this)
            Destroy(this);

        instance = this;
    }

    private void Start()
    {
        _controls = new Controls();
        _controls.Player.SetCallbacks(this);

        ToggleControls(true);
        ToggleCursor(false);
    }

    // Enable or Disable Cursor
    private void ToggleCursor(bool toggle)
    {
        if (toggle)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Enable or Disable Controls
    private void ToggleControls(bool toggle)
    {
        if (toggle)
            _controls.Player.Enable();
        else
            _controls.Player.Disable();
    }

    public void OnMovement(InputAction.CallbackContext context) =>
        MovementValue = context.ReadValue<Vector2>();

    public void OnInteract(InputAction.CallbackContext context) => InteractEvent?.Invoke();
}
