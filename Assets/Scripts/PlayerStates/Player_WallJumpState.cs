using UnityEngine;

public class Player_WallJumpState : PlayerState
{
    public Player_WallJumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    override public void Enter()
    {
        base.Enter();
        
        player.movement.WallJump();
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
