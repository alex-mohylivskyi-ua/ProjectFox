using UnityEngine;

public class Player_GroundedState : PlayerState
{
    public Player_GroundedState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    override public void Update()
    {
        base.Update();
        
        if (player.droppedThroughOneWayPlatformThisFrame)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }
        
        if (player.canUseLadder && Mathf.Abs(player.moveInput.y) > 0.1f)
        {
            stateMachine.ChangeState(player.ladderState);
            return;
        }
        
        if (player.jumpBuffered && (player.groundDetected || player.canUseCoyoteJump))
        {
            player.ConsumeJumpBuffer();
            player.ConsumeCoyoteTime();
            stateMachine.ChangeState(player.jumpState);
            return;
        }

        if (rb.linearVelocity.y < 0 && !player.groundDetected)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }

        if (player.inputReader.attackPressed)
        {
            stateMachine.ChangeState(player.basicAttackState);
        }
    }
}
