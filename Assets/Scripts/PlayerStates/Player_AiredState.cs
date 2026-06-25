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
            player.MovementData.moveSpeed,
            player.MovementData.airMoveMultiplier,
            player.MovementData.airMoveDeceleration,
            player.MovementData.apexThreshold,
            player.MovementData.apexMoveMultiplier
        );
        
        player.movement.ApplyFallGravity(
            player.MovementData.fallGravityMultiplier,
            player.MovementData.maxFallSpeed
        );
        
        if (player.canUseLadder && Mathf.Abs(player.moveInput.y) > 0.1f)
        {
            stateMachine.ChangeState(player.ladderState);
            return;
        }
        
        if (player.inputReader.attackPressed && player.abilities.CanAirAttack)
        {
            stateMachine.ChangeState(player.jumpAttackState);
        }
    }
}
