using UnityEngine;

public class Enemy : Entity
{
    public Enemy_IdleState idleState;
    public Enemy_MoveState moveState;
    public Enemy_AttackState attackState;

    [Header("Movement details")]
    [SerializeField] public float moveSpeed;
    [SerializeField] protected Transform groundAheadCheck;
    
    [Range(0, 2)]
    public float moveAnimSpeedMultiplyer = 1;
    public float idleTime = 2;
    public bool groundAheadDetected = false;

    protected override void Update()
    {
        base.Update();

    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        // Ground ahead check Gizmos line
        Gizmos.DrawLine(groundAheadCheck.position, groundAheadCheck.position + new Vector3(0, -groundCheckDistance));
    }

    protected override void HandleCollisionDetection()
    {
        base.HandleCollisionDetection();
        Debug.Log("HandleCollisionDetection second");
        groundAheadDetected = Physics2D.Raycast(groundAheadCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    }
}
