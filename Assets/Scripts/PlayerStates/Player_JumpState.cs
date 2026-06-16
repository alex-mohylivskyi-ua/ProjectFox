using UnityEngine;

public class Player_JumpState : Player_AiredState
{
    private bool jumpCutApplied;
    public Player_JumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    override public void Enter()
    {
        base.Enter();

        jumpCutApplied = false;
        player.movement.Jump(player.MovementData.jumpForce);
        if (player.bufferedJumpReleased)
        {
            jumpCutApplied = true;
            player.movement.JumpCut(player.MovementData.jumpCutMultiplier, player.MovementData.jumpCutMinVelocity);
            player.ClearBufferedJumpRelease();
        }
    }

    override public void Update()
    {
        base.Update();
        
        if (!player.inputReader.jumpHeld && rb.linearVelocity.y > 0 && !jumpCutApplied)
        {
            jumpCutApplied = true;
            player.movement.JumpCut(player.MovementData.jumpCutMultiplier, player.MovementData.jumpCutMinVelocity);
        }

        if (rb.linearVelocity.y < 0 && stateMachine.currentState != player.jumpAttackState)
        {
            stateMachine.ChangeState(player.fallState);
        }
    }
}
