using System;
using UnityEngine;

public class Enemy_BattleState : EnemyState
{
    public Enemy_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.UpdateBattleTimer();

        if (enemy.ShouldRetreat())
        {
            // We use linearVelocity instead of enemy.Move(), to skip enemy Flip()
            rb.linearVelocity = new Vector2(enemy.retreatVelocity.x * -enemy.DirectionToPlayer(), enemy.retreatVelocity.y);
            enemy.HandleFlip(enemy.DirectionToPlayer());
        }

        Debug.Log("inside battle state");
    }

    public override void Update()
    {
        base.Update();

        if (enemy.BattleTimeIsOver())
        {
            stateMachine.ChangeState(enemy.idleState);
        }
        
        if (enemy.WithinAttackRange() && enemy.PlayerDetected())
        {
            stateMachine.ChangeState(enemy.attackState);
        }
        else
        {
            if (enemy.distanceToPlayer() > 0.26)
            {
                enemy.Move(enemy.battleMoveSpeed * enemy.DirectionToPlayer(), enemy.rb.linearVelocity.y);
            }
        }
    }
}
