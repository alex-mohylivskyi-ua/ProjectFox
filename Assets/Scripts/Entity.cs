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
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.7f, 0.08f);
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

    public virtual void EntityDeath()
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
        
        RaycastHit2D groundHit = Physics2D.BoxCast(
            groundCheck.position,
            groundCheckSize,
            0f,
            Vector2.down,
            groundCheckDistance,
            whatIsGround
        );

        wallDetected = topWallHit || bottomWallHit;
        canWallSlide = topWallHit && bottomWallHit;
        groundDetected = groundHit;
    }

    protected virtual void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Vector3 groundBoxCenter = groundCheck.position + Vector3.down * groundCheckDistance;
            Gizmos.DrawWireCube(groundBoxCenter, groundCheckSize);
        }
        
        if (topWallCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(topWallCheck.position, topWallCheck.position + new Vector3(wallCheckDistance * facingDirection, 0));
        }
        
        if (bottomWallCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(bottomWallCheck.position, bottomWallCheck.position + new Vector3(wallCheckDistance * facingDirection, 0));    
        }
    }

    public void CurrentStateAnimationTrigger()
    {
        stateMachine.currentState.AnimationTrigger();
    }
}
