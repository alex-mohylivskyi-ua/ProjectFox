using UnityEngine;

public class Player_MoveState : Player_GroundedState
{
    public Player_MoveState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    // we use it as grounded state because we want to be able to jump from it, and we want to be able to move while wall sliding.

    

    override public void Enter()
    {
        base.Enter();

        // Debug.Log("We are in Move state");
    }

    override public void Update()
    {
        base.Update();

        player.SetVelocity(player.moveInput.x * player.moveSpeed, rb.linearVelocity.y);

        if (player.moveInput.x == 0 || player.wallDetected)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
