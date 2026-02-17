using UnityEngine;

public class Player_JumpState : Player_AiredState
{
    public Player_JumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    override public void Enter()
    {
        base.Enter();

        player.SetVelocity(rb.linearVelocity.x, player.jumpForce);
    }

    override public void Update()
    {
        base.Update();

        if (rb.linearVelocity.y < 0)
        {
            stateMachine.ChangeState(player.fallState);
        }
    }
}
