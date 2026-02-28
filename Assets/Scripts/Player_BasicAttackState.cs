using UnityEngine;

public class Player_BasicAttackState : EntityState
{
    private int comboIndex = 0;
    private int maxComboIndex = 3;
    private float lastTimeAttacked;
    private bool combatAttackQueued;
    private int attackDirection;

    public Player_BasicAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        if (maxComboIndex != player.attackVelocity.Length)
        {
            Debug.LogWarning("I've adjusted combo limit, according to attack velocity array size!");
        }
    }

    public override void Enter()
    {
        base.Enter();

        // Define attack direction according to player input
        attackDirection = player.moveInput.x != 0 ? ((int)player.moveInput.x) : player.facingDirection;

        combatAttackQueued = false;
        ResetComboIndexIfNeeded();
        anim.SetInteger("basicAttackIndex", comboIndex);
        ApplyAttackVelocity();
    }

    private float attackVelocityTimer;

    public override void Update()
    {
        base.Update();

        HandleAttackVelocity();

        if (input.Player.Attack.WasPressedThisFrame())
        {
            QueuedNextAttack();
        }

        if (triggerCalled)
        {
            HandleStateExit();
        }
    }

    private void HandleStateExit()
    {
        if (combatAttackQueued)
        {
            anim.SetBool(animBoolName, false);
            player.EnterAttackStateWithDelay();
        } else 
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

    private void ApplyAttackVelocity()
    {
        attackVelocityTimer = player.attackVelocityDuration;

        Vector2 attackVelocity = player.attackVelocity[comboIndex];

        player.SetVelocity(attackVelocity.x * attackDirection, attackVelocity.y);
    }

    private void HandlePlayerBasicAttackCount()
    {
        // (0 + 1) % 3 = 1
        // (1 + 1) % 3 = 2
        // (2 + 1) % 3 = 0
        comboIndex = (comboIndex + 1) % maxComboIndex;
    }

    private void ResetComboIndexIfNeeded()
    {
        // If the reset combo time has passed
        if (Time.time > lastTimeAttacked + player.comboResetTime)
        {
            comboIndex = 0;
        }
    }

    private void QueuedNextAttack()
    {
        if (comboIndex < maxComboIndex - 1)
        {
            combatAttackQueued = true;
        }
    }

    public override void Exit()
    {
        base.Exit();
        HandlePlayerBasicAttackCount();
        lastTimeAttacked = Time.time;
    }
}
