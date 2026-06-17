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
    [SerializeField] private float groundAheadCheckDistance;
    [SerializeField] private LayerMask whatIsGroundAhead;
    
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
        
        RaycastHit2D playerHit = PlayerDetected();
        
        if (player == null && playerHit)
        {
            player = playerHit.transform;
        }

        if (playerHit)
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
        
        // How it works. TOKNOW
        // Ground ahead check Gizmos line
        if (groundAheadCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(
                groundAheadCheck.position,
                groundAheadCheck.position + new Vector3(0, -groundAheadCheckDistance)
            );
        }
        
        if (playerCheck == null)
        {
            return;
        }
       
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
        if (playerCheck == null)
        {
            return default;
        }
        
        RaycastHit2D hit = Physics2D.Raycast(
            playerCheck.position,
            Vector2.right * facingDirection,
            playerCheckDistance,  
            whatIsPlayer | whatIsGroundAhead);
        // Filter by layer (not tag) so detection works with multiple players on the Player layer.
        if (hit && IsPlayerLayer(hit.collider.gameObject.layer))
        {
            return hit;
        }

        return default;
    }

    private bool IsPlayerLayer(int layer)
    {
        return (whatIsPlayer.value & (1 << layer)) != 0;
    }

    public bool ShouldRetreat()
    {
        return minRetreatDistance > distanceToPlayer();
    }

    protected override void HandleCollisionDetection()
    {
        base.HandleCollisionDetection();
        
        if (groundAheadCheck == null)
        {
            groundAheadDetected = false;
            return;
        }
        
        groundAheadDetected = Physics2D.Raycast(
            groundAheadCheck.position,
            Vector2.down,
            groundAheadCheckDistance,
            whatIsGroundAhead);
    }

    private void OnEnable()
    {
        // Player.OnPlayerDeath += HandlePlayerDeath;
    }
    
    private void OnDisable()
    {
        // Player.OnPlayerDeath -= HandlePlayerDeath;
    }

    private void HandlePlayerDeath(Player deadPlayer)
    {
        // Only react if the player who died is THIS enemy's current target.
        // Otherwise enemies fighting other players would wrongly reset.
        if (deadPlayer == null || player != deadPlayer.transform)
        {
            return;
        }

        player = null;
        stateMachine.ChangeState(idleState);
    }
}
