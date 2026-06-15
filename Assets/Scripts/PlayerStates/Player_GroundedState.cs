using UnityEngine;

public class Player_GroundedState : PlayerState
{
    public Player_GroundedState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    override public void Update()
    {
        base.Update();

        if (player.jumpBuffered)
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
