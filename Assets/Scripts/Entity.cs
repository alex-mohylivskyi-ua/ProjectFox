using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    // [ContextMenu("Special attack!")]

    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    protected StateMachine stateMachine;

    [SerializeField] private bool facingRight = true;
    [SerializeField] public int facingDirection = 1;
    

    [Header("Collision detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;
    public bool groundDetected { get; private set; }
    public bool wallDetected { get; private set; }
    public bool canWallSlide { get; private set; }
    
    [SerializeField] private Transform topWallCheck;
    [SerializeField] private Transform bottomWallCheck;
    
    private Entity_Knockback knockbackController;

    // protected virtual void Awake() - need to override it later
    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        stateMachine = new StateMachine();
        knockbackController = GetComponent<Entity_Knockback>();
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        // It runs Update inside PlayerState without having MonoBehaviour
        HandleCollisionDetection();
        stateMachine.UpdateActiveState();
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        if (knockbackController.isKnocked)
            return;
        
        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlip(xVelocity);
    }

    public void HandleFlip(float xVelocity)
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

    protected virtual void HandleCollisionDetection()
    {
        RaycastHit2D topWallHit = Physics2D.Raycast(
            topWallCheck.position,
            Vector2.right * facingDirection,
            wallCheckDistance,
            whatIsGround
        );

        RaycastHit2D bottomWallHit = default;
        if (bottomWallCheck != null)
        {
            bottomWallHit = Physics2D.Raycast(
                bottomWallCheck.position,
                Vector2.right * facingDirection,
                wallCheckDistance,
                whatIsGround
            );
        }

        wallDetected = topWallHit || bottomWallHit;
        canWallSlide = topWallHit && bottomWallHit;
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    }

    protected virtual void OnDrawGizmos()
    {
        // Ground check Gizmos line
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + new Vector3(0, -groundCheckDistance));
        // Wall check Gizmos line top
        Gizmos.DrawLine(topWallCheck.position, topWallCheck.position + new Vector3(wallCheckDistance * facingDirection, 0));

        if (bottomWallCheck != null)
        {
            // Wall check Gizmos line bottom
            Gizmos.DrawLine(bottomWallCheck.position, bottomWallCheck.position + new Vector3(wallCheckDistance * facingDirection, 0));    
        }
    }

    public void CurrentStateAnimationTrigger()
    {
        stateMachine.currentState.AnimationTrigger();
    }
}
