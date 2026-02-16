using UnityEngine;

public class Player_MoveState : EntityState
{
    public Player_MoveState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    override public void Update()
    {
        base.Update();

        if (player.moveInput.x == 0)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }  
}
