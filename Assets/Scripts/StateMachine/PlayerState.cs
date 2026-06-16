using UnityEngine;

public abstract class PlayerState: EntityState
{
    protected Player player;

    public PlayerState(Player player, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        // this === PlayerState
        this.player = player;

        anim = player.anim;
        rb = player.rb;
    }

    // public - can be called from outside, virtual - can be inherited and extended in new instance
    public override void Update()
    {
        // we're going to run logic of the state
        // Debug.Log("I run update of " + animBoolName);
        base.Update();
        
        stateTimer -= Time.deltaTime;

        if (player.inputReader.dashPressed && CanUseDashAbility() && CanEnterDashState())
        {
            stateMachine.ChangeState(player.dashState);
        }
    }
    
    private bool CanUseDashAbility()
    {
        return player.abilities != null && player.abilities.CanDash;
    }

    private bool CanEnterDashState()
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

    public override void UpdateAnimationParameters()
    //this method is called in EntityState.Update();
    //We can override it here and updated version will be called from Entity
    {
        base.UpdateAnimationParameters();
        
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
    }
}
