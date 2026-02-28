using UnityEngine;

public class Player_DashState : PlayerState
{
    private float originalGravityScale;
    private int dashDir;

    public Player_DashState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();

        dashDir = player.moveInput.x != 0 ? ((int)player.moveInput.x) : player.facingDirection;
        stateTimer = player.dashDuration;
        originalGravityScale = rb.gravityScale;
        rb.gravityScale = 0;

        // TOENJOY:  bulshit i wrote, we give impulse just once here inside 1 frame
        // player.SetVelocity(player.dashSpeed * player.facingDirection, 0);
    }

    public override void Update()
    {
        base.Update();

        CancelDashIfNeeded();

        player.SetVelocity(player.dashSpeed * dashDir, 0);

        if (stateTimer <= 0)
        {
            if (player.groundDetected)
            {
                stateMachine.ChangeState(player.idleState);
            } else
            {
                stateMachine.ChangeState(player.fallState);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();

        // we reset velocity after dash, so we don't have to care about it in other states
        player.SetVelocity(0, rb.linearVelocity.y);
        rb.gravityScale = originalGravityScale;
    }

    private void CancelDashIfNeeded()
    {
        if (player.wallDetected)
        {
            if (player.groundDetected)
            {
                stateMachine.ChangeState(player.idleState);
            } else
            {
                stateMachine.ChangeState(player.wallSlideState);
            }
        }
    }
}
