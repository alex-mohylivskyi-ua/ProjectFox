using UnityEngine;

public class Player_AiredState : PlayerState
{
    public Player_AiredState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    override public void Update()
    {
        base.Update();
        
        player.movement.AirMove(
            player.moveInput.x,
            player.moveSpeed,
            player.airMoveMultiplyier,
            player.airMoveDeceleration
        );
        
        if (input.Player.Attack.WasPressedThisFrame())
        {
            stateMachine.ChangeState(player.jumpAttackState);
        }
    }
}
