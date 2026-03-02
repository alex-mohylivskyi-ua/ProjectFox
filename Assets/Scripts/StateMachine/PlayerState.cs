using UnityEngine;

public abstract class PlayerState: EntityState
{
    protected Player player;
    protected PlayerInputSet input;

    public PlayerState(Player player, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        // this === PlayerState
        this.player = player;

        anim = player.anim;
        rb = player.rb;
        input = player.input;
    }

    // public - can be called from outside, virtual - can be inherited and extended in new instance
    public override void Update()
    {
        // we're going to run logic of the state
        // Debug.Log("I run update of " + animBoolName);
        base.Update();
        stateTimer -= Time.deltaTime;

        anim.SetFloat("yVelocity", rb.linearVelocity.y);

        if (input.Player.Dash.WasPressedThisFrame() && CanDash())
        {
            stateMachine.ChangeState(player.dashState);
        }
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
}
