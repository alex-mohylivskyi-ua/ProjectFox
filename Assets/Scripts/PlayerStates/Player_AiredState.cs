using UnityEngine;

public class Player_AiredState : PlayerState
{
    public Player_AiredState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    override public void Update()
    {
        base.Update();
        
        player.movement.AirMoveWithApexControl(
            player.moveInput.x,
            player.moveSpeed,
            player.airMoveMultiplier,
            player.airMoveDeceleration,
            player.apexThreshold,
            player.apexMoveMultiplier
        );
        
        player.movement.ApplyFallGravity(
            player.fallGravityMultiplier,
            player.maxFallSpeed
        );
        
        if (input.Player.Attack.WasPressedThisFrame())
        {
            stateMachine.ChangeState(player.jumpAttackState);
        }
    }
}
