using UnityEngine;

public class Entity : MonoBehaviour
{
    // [ContextMenu("Special attack!")]

    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    protected StateMachine stateMachine;

    private bool facingRight = true;
    public int facingDirection { get; private set; } = 1;
    

    [Header("Collision detection")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    public bool groundDetected { get; private set; }
    public bool wallDetected { get; private set; }
    public bool canWallSlide { get; private set; }
   
    
    [SerializeField] private Transform topWallCheck;
    [SerializeField] private Transform bottomWallCheck;
    

    // protected virtual void Awake() - need to override it later
    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        stateMachine = new StateMachine();
    }

    protected virtual void Start()
    {
        
    }

    private void Update()
    {
        // It runs Update inside EntityState without having MonoBehaviour
        HandleCollisionDetection();
        stateMachine.UpdateActiveState();
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleVelocity(xVelocity);
    }

    private void HandleVelocity(float xVelocity)
    {
        if (xVelocity > 0 && !facingRight)
        {
            Flip();
        } else if (xVelocity < 0 && facingRight)
        {
            Flip();
        }
    }

    public void Flip()
    {
        transform.Rotate(0f, 180f, 0f);
        facingRight = !facingRight;
        facingDirection = facingDirection * -1;
    }

    private void HandleCollisionDetection()
    {
        RaycastHit2D topWallHit = Physics2D.Raycast(
            topWallCheck.position,
            Vector2.right * facingDirection,
            wallCheckDistance,
            whatIsGround
        );

        RaycastHit2D bottomWallHit = Physics2D.Raycast(
            bottomWallCheck.position,
            Vector2.right * facingDirection,
            wallCheckDistance,
            whatIsGround
        );

        wallDetected = topWallHit || bottomWallHit;
        canWallSlide = topWallHit && bottomWallHit;
        groundDetected = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
    }

    private void OnDrawGizmos()
    {
        // Ground check Gizmos line
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -groundCheckDistance));
        // Wall check Gizmos line top
        Gizmos.DrawLine(topWallCheck.position, topWallCheck.position + new Vector3(wallCheckDistance * facingDirection, 0));
        // Wall check Gizmos line bottom
        Gizmos.DrawLine(bottomWallCheck.position, bottomWallCheck.position + new Vector3(wallCheckDistance * facingDirection, 0));
    }

    public void CallAnimationTrigger()
    {
        stateMachine.currentState.CallAnimationTrigger();
    }
}
