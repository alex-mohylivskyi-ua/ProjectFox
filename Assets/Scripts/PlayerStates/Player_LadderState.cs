using UnityEngine;

public class Player_LadderState : PlayerState
{
    private float originalGravityScale;

    public Player_LadderState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        originalGravityScale = rb.gravityScale;
        rb.gravityScale = 0f;

        player.ClearJumpBuffer();
        player.SetVelocity(0f, 0f);
    }

    public override void Update()
    {
        base.Update();

        if (!player.canUseLadder)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }

        if (player.inputReader.jumpPressed)
        {
            Debug.Log("Jumping from ladder");
            // rb.gravityScale = originalGravityScale;
            // stateMachine.ChangeState(player.jumpState);
            // return;
            
            if (player.inputReader.jumpPressed)
            {
                rb.gravityScale = originalGravityScale;
                player.StartLadderJumpCooldown();
                stateMachine.ChangeState(player.jumpState);
                return;
            }
        }

        float yInput = player.moveInput.y;
        float xInput = player.moveInput.x;
        Ladder ladder = player.CurrentLadder;

        if (ladder != null && ladder.CenterPlayerOnLadder)
        {
            if (xInput != 0f)
            {
                player.HandleFlip(xInput);
            }
            
            player.movement.ClimbLadderCentered(
                yInput,
                player.MovementData.ladderClimbSpeed,
                ladder.CenterX,
                ladder.CenterSpeed,
                ladder.CenterSnapDistance
            );
        }
        else
        {
            player.movement.ClimbLadder(
                player.moveInput.x,
                yInput,
                player.MovementData.ladderClimbSpeed,
                player.MovementData.ladderHorizontalSpeedMultiplier
            );
        }

        if (player.groundDetected && yInput < 0f)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }
    }

    public override void Exit()
    {
        rb.gravityScale = originalGravityScale;

        base.Exit();
    }
}