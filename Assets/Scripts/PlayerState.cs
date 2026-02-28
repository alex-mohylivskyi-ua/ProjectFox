using UnityEngine;

public abstract class PlayerState
{
    protected Player player;
    protected StateMachine stateMachine;
    protected string animBoolName;
    protected Animator anim;
    protected Rigidbody2D rb;
    protected PlayerInputSet input;
    protected float stateTimer;
    protected bool triggerCalled;

    public PlayerState(Player player, StateMachine stateMachine, string animBoolName)
    {
        // this === PlayerState
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
        triggerCalled = false;
    }

    public virtual void Update()
    {
        // we're going to run logic of the state
        // Debug.Log("I run update of " + animBoolName);
        stateTimer -= Time.deltaTime;
        anim.SetFloat("yVelocity", rb.linearVelocity.y);

        if (input.Player.Dash.WasPressedThisFrame() && CanDash())
        {
            stateMachine.ChangeState(player.dashState);
        }
    }

    public virtual void Exit()
    {
        // This will be called, everytime we Exit state and change to a new one
        // Debug.Log("I exit " + animBoolName);
        anim.SetBool(animBoolName, false);
    }

    private bool CanDash()
    {
        if (player.wallDetected) {
            return false;
        }

        // Checking current state
        if (stateMachine.currentState == player.dashState)
        {
            return false;
        }

        return true;
    }

    public void CallAnimationTrigger()
    {
        triggerCalled = true;
    }
}
