using UnityEngine;

public class Player_FallState : EntityState
{
    public Player_FallState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    override public void Update()
    {
        base.Update();

        // if (player.IsGrounded())
        // {
        //     stateMachine.ChangeState(player.idleState);
        // }
    }
}
