using UnityEngine;
using System;

public class Enemy : Entity
{
    public Enemy_IdleState idleState;
    public Enemy_MoveState moveState;
    public Enemy_AttackState attackState;
    public Enemy_BattleState battleState;
    public Enemy_DeadState deadState;

    [Header("Movement details")]
    [SerializeField] public float moveSpeed;
    [SerializeField] protected Transform groundAheadCheck;
    
    [Range(0, 2)]
    public float moveAnimSpeedMultiplier = 1;
    public float idleTime = 2;
    public bool groundAheadDetected = false;

    [Header("Battle details")] 
    // [SerializeField] private float attackDistance = 2;
    [field: SerializeField] public float attackDistance { get; private set; } = 2f;
    [field: SerializeField] public float battleMoveSpeed { get; private set; } = 3;
    public float battleTimeDuration = 5;
    public float lastTimeWasInBattle;
    public float minRetreatDistance = 1;
    public Vector2 retreatVelocity;
    public float inGameTime { get; private set; }
    public Transform player;

    [Header("Player detection details")]
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private Transform playerCheck;
    [SerializeField] private float playerCheckDistance = 10;

    protected override void Update()
    {
        base.Update();

        UpdateInGameTime();
        
        if (player == null)
        {
            player = PlayerDetected().transform;
        }

        if (PlayerDetected() == true)
        {
            UpdateBattleTimer();    
        }
    }

    public void TryEnterBattleState(Transform player)
    {
        if (stateMachine.currentState == battleState ||
            stateMachine.currentState == attackState)
        {
            return;
        }

        this.player = player;
        stateMachine.ChangeState(battleState);
    }

    public override void EntityDeath()
    {
        base.EntityDeath();
        
        stateMachine.ChangeState(deadState);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        // Ground ahead check Gizmos line
        Gizmos.DrawLine(groundAheadCheck.position, groundAheadCheck.position + new Vector3(0, -groundCheckDistance));
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDirection * playerCheckDistance), playerCheck.position.y));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDirection * attackDistance), playerCheck.position.y));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDirection * minRetreatDistance), playerCheck.position.y));
    }

    public bool BattleTimeIsOver()
    {
        return inGameTime >= lastTimeWasInBattle + battleTimeDuration;
    }

    public bool WithinAttackRange()
    {
        return distanceToPlayer() <= attackDistance;
    }

    public void Move(float x, float y)
    {
        SetVelocity(x, y);
    }

    public int DirectionToPlayer()
    {
        if (player == null)
        {
            return 0;
        }
        return player.transform.position.x > transform.position.x ? 1 : -1;
    }
    
    public float distanceToPlayer()
    {
        if (player == null)
        {
            return float.MaxValue;
        }

        return Math.Abs(transform.position.x - player.transform.position.x);
    }

    public void UpdateBattleTimer()
    {
        lastTimeWasInBattle = Time.time;
    }

    private void UpdateInGameTime()
    {
        inGameTime = Time.time;
    }

    public RaycastHit2D PlayerDetected()
    {
        RaycastHit2D hit = Physics2D.Raycast(playerCheck.position, Vector2.right * facingDirection, playerCheckDistance,  whatIsPlayer | whatIsGround);
        // if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        if (hit && hit.collider.CompareTag("Player"))
        {
            return hit;
        }
        
        return default;
    }

    public bool ShouldRetreat()
    {
        return minRetreatDistance > distanceToPlayer();
    }

    protected override void HandleCollisionDetection()
    {
        base.HandleCollisionDetection();
        Debug.Log("HandleCollisionDetection second");
        groundAheadDetected = Physics2D.Raycast(groundAheadCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    }

    private void OnEnable()
    {
        Player.OnPlayerDeath += HandlePlayerDeath;
    }
    
    private void OnDisable()
    {
        Player.OnPlayerDeath -= HandlePlayerDeath;
    }

    private void HandlePlayerDeath()
    {
        Debug.Log("HandlePlayerDeath");
        stateMachine.ChangeState(idleState);
    }
}
