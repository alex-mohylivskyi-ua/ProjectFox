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

        if (player.groundDetected)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }

        if (player.canWallSlide)
        {
            stateMachine.ChangeState(player.wallSlideState);
        }
    }
}
