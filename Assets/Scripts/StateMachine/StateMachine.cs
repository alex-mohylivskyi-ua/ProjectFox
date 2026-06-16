using System;
using UnityEngine;

public class StateMachine
{
    public EntityState currentState { get; private set; }
    public string CurrentStateName => currentState != null ? currentState.GetType().Name : "None";
    public event Action<EntityState, EntityState> OnStateChanged;
    private bool canChangeState;

    public void Initialize(EntityState startState)
    {
        if (startState == null)
        {
            Debug.LogError("Cannot initialize StateMachine with null start state.");
            canChangeState = false;
            return;
        }
        
        canChangeState = true;
        currentState = startState;
        currentState.Enter();
    }

    public void ChangeState(EntityState newState, bool force = false)
    {
        if (!canChangeState)
        {
            Debug.LogError("Trying to change state while not allowed!");
            return;
        }
        
        if (newState == null)
        {
            Debug.LogWarning($"Cannot change state from {CurrentStateName} to null.");
            return;
        }
        
        if (!force && currentState == newState)
        {
            return;
        }
        
        EntityState previousState = currentState;

        currentState.Exit();
        currentState = newState;
        currentState.Enter();
        
        OnStateChanged?.Invoke(previousState, currentState);
    }

    public void UpdateActiveState()
    {
        currentState?.Update();
    }

    public void SwitchOffStateMachine()
    {
        canChangeState = false;
    }
}
