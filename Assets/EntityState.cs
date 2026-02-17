using UnityEngine;

public abstract class EntityState
{
    protected Player player;
    protected StateMachine stateMachine;
    protected string animBoolName;
    protected Animator anim;
    protected Rigidbody2D rb;
    protected PlayerInputSet input;

    public EntityState(Player player, StateMachine stateMachine, string animBoolName)
    {
        // this === EntityState
        this.player = player;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;

        anim = player.anim;
        rb = player.rb;
        input = player.input;
    }

    // public - can be called from outside, virtual - can be inherited and extended in new instance
    public virtual void Enter()
    {
        // Everytime state will be changed any Enter, Enter will be called
        // Debug.Log("I enter " + animBoolName);
        anim.SetBool(animBoolName, true);
    }

    public virtual void Update()
    {
        // we're going to run logic of the state
        // Debug.Log("I run update of " + animBoolName);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    public virtual void Exit()
    {
        // This will be called, everytime we Exit state and change to a new one
        // Debug.Log("I exit " + animBoolName);
        anim.SetBool(animBoolName, false);
    }
}
