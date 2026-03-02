using UnityEngine;

public class Enemy : Entity
{
    public Enemy_IdleState idleState;
    public Enemy_MoveState moveState;
    public Enemy_AttackState attackState;
    public Enemy_BattleState battleState;

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
    
    [Header("Player detection details")]
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private Transform playerCheck;
    [SerializeField] private float playerCheckDistance = 10;

    protected override void Update()
    {
        base.Update();

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
    }
    
    public RaycastHit2D PlayerDetection()
    {
        RaycastHit2D hit = Physics2D.Raycast(playerCheck.position, Vector2.right * facingDirection, playerCheckDistance,  whatIsPlayer | whatIsGround);
        // if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        if (hit && hit.collider.CompareTag("Player"))
        {
            Debug.Log("Hit Player");
            return hit;
        }
        
        Debug.Log("No Hit Player");

        return default;
    }

    protected override void HandleCollisionDetection()
    {
        base.HandleCollisionDetection();
        Debug.Log("HandleCollisionDetection second");
        groundAheadDetected = Physics2D.Raycast(groundAheadCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    }
}
