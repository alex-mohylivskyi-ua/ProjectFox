using UnityEngine;

public class Player_MoveState : Player_GroundedState
{
    public Player_MoveState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    override public void Update()
    {
        base.Update();

        player.SetVelocity(player.moveInput.x * player.moveSpeed, rb.linearVelocity.y);

        if (player.moveInput.x == 0)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
