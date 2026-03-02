using System;
using UnityEngine;

public class Enemy_BattleState : EnemyState
{
    public Enemy_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();
        
    }
}
