using UnityEngine;

public class Player_WallJumpState : EntityState
{
    public Player_WallJumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    override public void Enter()
    {
        base.Enter();

        player.SetVelocity(player.wallJumpForce.x * -player.facingDirection, player.wallJumpForce.y);
    }

    override public void Update()
    {
        base.Update();

        if (rb.linearVelocity.y < 0)
        {
            stateMachine.ChangeState(player.fallState);
        }

        
        // TOFIX he used this.
        // if (player.wallDetected)
        // {
        //     stateMachine.ChangeState(player.wallSlideState);
        // }
    }
}
