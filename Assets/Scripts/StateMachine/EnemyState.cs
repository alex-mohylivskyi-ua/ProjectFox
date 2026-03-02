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
        anim.SetFloat("moveAnimSpeedMultiplier", enemy.moveAnimSpeedMultiplier);
        
    }

    public override void Update()
    {
        base.Update();
        
        anim.SetFloat("xVelocity", rb.linearVelocity.x);
    } 
}
