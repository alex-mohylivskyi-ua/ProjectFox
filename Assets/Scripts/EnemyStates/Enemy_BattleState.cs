using System;
using UnityEngine;

public class Enemy_BattleState : EnemyState
{
    private Transform player;
    public Enemy_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        Debug.Log("inside battle state");
    }

    public override void Update()
    {
        base.Update();

        if ((enemy.lastTimeWasInBattle + enemy.battleTimeDuration) <= enemy.inGameTime)
        {
            stateMachine.ChangeState(enemy.idleState);
        }

        if (player == null)
        {
            player = enemy.PlayerDetection().transform;    
        }

        if (WithinAttachRange() && enemy.PlayerDetection())
        {
            stateMachine.ChangeState(enemy.attackState);
        }
        else
        {
            if (distanceToPlayer() > 0.26)
            {
                enemy.SetVelocity(enemy.battleMoveSpeed * DirectionToPlayer(), enemy.rb.linearVelocity.y);
            }
        }
    }

    private int DirectionToPlayer()
    {
        if (player == null)
        {
            return 0;
        }
        return player.transform.position.x > enemy.transform.position.x ? 1 : -1;
    }

    private bool WithinAttachRange()
    {
        return distanceToPlayer() <= enemy.attackDistance;
    }

    private float distanceToPlayer()
    {
        if (player == null)
        {
            return float.MaxValue;
        }

        return Math.Abs(enemy.transform.position.x - player.transform.position.x);
    }
}
