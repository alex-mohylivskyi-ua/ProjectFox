using UnityEngine;

public class Player_FallState : Player_AiredState
{
    public Player_FallState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    override public void Update()
    {
        base.Update();
        
        if (stateMachine.currentState != this)
        {
            return;
        }
        
        if (player.jumpBuffered && player.canUseCoyoteJump)
        {
            player.ConsumeJumpBuffer();
            player.ConsumeCoyoteTime();
            stateMachine.ChangeState(player.jumpState);
            return;
        }

        if (player.groundDetected && !player.shouldIgnoreGroundAfterOneWayDrop)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }

        if (player.wallSlideSurfaceDetected && player.abilities.CanWallSlide)
        {
            stateMachine.ChangeState(player.wallSlideState);
        }
    }
}
