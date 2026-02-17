using UnityEngine;

public class Player_AiredState : EntityState
{
    public Player_AiredState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    override public void Update()
    {
        base.Update();

        if (player.moveInput.x !=0)
        {
            player.SetVelocity(player.moveInput.x * (player.moveSpeed * player.airMoveMultiplyier), rb.linearVelocity.y);
        }
    }
}
