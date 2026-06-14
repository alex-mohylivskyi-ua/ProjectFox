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
        player.movement.Jump(player.jumpForce);
        // player.SetVelocity(rb.linearVelocity.x, player.jumpForce); OLD Jump
        if (player.bufferedJumpReleased)
        {
            jumpCutApplied = true;
            player.movement.JumpCut(player.jumpCutMultiplier);
            player.ClearBufferedJumpRelease();
        }
    }

    override public void Update()
    {
        base.Update();
        
        if (!input.Player.Jump.IsPressed() && rb.linearVelocity.y > 0 && !jumpCutApplied)
        {
            jumpCutApplied = true;
            player.movement.JumpCut(player.jumpCutMultiplier);
        }

        if (rb.linearVelocity.y < 0 && stateMachine.currentState != player.jumpAttackState)
        {
            stateMachine.ChangeState(player.fallState);
        }
    }
}
