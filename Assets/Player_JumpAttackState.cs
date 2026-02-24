using UnityEngine;

public class Player_JumpAttackState : EntityState
{
    public Player_JumpAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();

        player.SetVelocity(player.jumpAttackVelocity.x * player.facingDirection, player.jumpAttackVelocity.y);
    }

    public override void Update()
    {
        base.Update();
        if (player.groundDetected)
        {
            anim.SetTrigger("jumpAttackEndTrigger");
            player.SetVelocity(0, rb.linearVelocity.y);
        }

        if (triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        anim.ResetTrigger("jumpAttackEndTrigger");
    }
}
