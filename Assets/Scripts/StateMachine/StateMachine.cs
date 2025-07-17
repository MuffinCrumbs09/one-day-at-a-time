using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    private State _curState;
    private bool _isSwitching;

    // Tick when not switching
    private void Update()
    {
        _curState.Tick(Time.deltaTime);
    }

    // Switch State
    public void SwitchState(State newState)
    {
        if (_isSwitching)
            return;

        _isSwitching = true;

        _curState?.Exit();
        _curState = newState;
        _curState?.Enter();

        _isSwitching = false;
    }
}
