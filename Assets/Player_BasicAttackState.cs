using UnityEngine;

public class Player_BasicAttackState : EntityState
{
    public Player_BasicAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();

        GenerateAttackVelocity();
    }

    private float attackVelocityTimer;

    public override void Update()
    {
        base.Update();

        HandleAttackVelocity();

        if (triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

    private void HandleAttackVelocity()
    {   
        // Probably later i won't lock player moving so commenting it for now
        // rb.linearVelocity = new Vector2(player.moveInput.x * player.moveSpeed, rb.linearVelocity.y);
        attackVelocityTimer -= Time.deltaTime;

        if (attackVelocityTimer < 0)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    private void GenerateAttackVelocity()
    {
        attackVelocityTimer = player.attackVelocityDuration;
        player.SetVelocity(player.attackVelocity.x * player.facingDirection, player.attackVelocity.y);
    }
}
