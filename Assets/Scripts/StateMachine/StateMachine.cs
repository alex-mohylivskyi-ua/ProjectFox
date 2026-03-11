using UnityEngine;

public class StateMachine
{
    public EntityState currentState { get; private set; }
    private bool canChangeState;

    public void Initialize(EntityState startState)
    {
        canChangeState = true;
        currentState = startState;
        currentState.Enter();
    }

    public void ChangeState(EntityState newState)
    {
        if (!canChangeState)
        {
            Debug.LogError("Trying to change state while not allowed!");
            return;
        }

        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void UpdateActiveState()
    {
        currentState.Update();
    }

    public void SwitchOffStateMachine()
    {
        canChangeState = false;
    }
}
