using UnityEngine;

public class Player_WallSlideState : EntityState
{
    public Player_WallSlideState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    public override void Update()
    {
        base.Update();

        HandleWallSlide();

        if (player.groundDetected)
        {
            stateMachine.ChangeState(player.idleState);
            player.Flip();
        }
        
        else if (!player.wallDetected)
        {
            stateMachine.ChangeState(player.fallState);
        }
    }

    private void HandleWallSlide()
    {
        if (player.moveInput.y < 0)
        {
            player.SetVelocity(player.moveInput.x * player.moveSpeed, rb.linearVelocity.y);
        } else {
            player.SetVelocity(player.moveInput.x * player.moveSpeed, rb.linearVelocity.y * player.wallSlideSlowMultiplyier);
        }
    }
}

// TODO check if we have a bug with 1px gap when wallSliding on the left side.
