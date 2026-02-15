using UnityEngine;

public abstract class EntityState
{
    protected StateMachine stateMachine;
    protected string stateName;

    public EntityState(StateMachine stateMachine, string stateName)
    {
        // this === EntityState
        this.stateMachine = stateMachine;
        this.stateName = stateName;
    }

    // public - can be called from outside, virtual - can be inherited and extended in new instance
    public virtual void Enter()
    {
        // Everytime state will be changed any Enter, Enter will be called
        Debug.Log("I enter " + stateName);
    }

    public virtual void Update()
    {
        // we're going to run logic of the state
        Debug.Log("I run update of " + stateName);
    }

    public virtual void Exit()
    {
        // This will be called, everytime we Exit state and change to a new one
        Debug.Log("I exit " + stateName);
    }
}
