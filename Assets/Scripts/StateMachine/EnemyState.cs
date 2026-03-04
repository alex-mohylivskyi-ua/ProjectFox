using UnityEngine;

public class EnemyState : EntityState
{
    protected Enemy enemy;
    public EnemyState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        // this === PlayerState
        this.enemy = enemy;

        anim = enemy.anim;
        rb = enemy.rb;
    }

    public override void Enter()
    {
        base.Enter();

        Debug.Log("START");
    }

    public override void Update()
    {
        base.Update();
        
        float battleAnimSpeedMultiplier = enemy.battleMoveSpeed / enemy.moveSpeed;
        
        anim.SetFloat("xVelocity", rb.linearVelocity.x);
        anim.SetFloat("battleAnimSpeedMultiplier", battleAnimSpeedMultiplier);
        anim.SetFloat("moveAnimSpeedMultiplier", enemy.moveAnimSpeedMultiplier);
    } 
}
