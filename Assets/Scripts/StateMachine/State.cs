using UnityEngine;

public abstract class State
{
    public virtual void Enter() { }
    public virtual void Tick(float deltaTime) { }
    public virtual void FixedTick(float fixedDelta) { }
    public virtual void Exit() { }
}
