using System;
using UnityEngine;

public class Enemy_BattleState : EnemyState
{
    private Transform player;
    public Enemy_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        if (player == null)
        {
            player = enemy.PlayerDetection().transform;    
        }

        if (WithinAttachRange())
        {
            stateMachine.ChangeState(enemy.attackState);
        }
        else
        {
            enemy.SetVelocity(enemy.battleMoveSpeed * DirectionToPlayer(), enemy.rb.linearVelocity.y);
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
