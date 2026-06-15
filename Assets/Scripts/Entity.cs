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
    protected Entity_Collision collision;
    public bool groundDetected => collision != null && collision.groundDetected;
    public bool wallDetected => collision != null && collision.wallDetected;
    public bool canWallSlide => collision != null && collision.canWallSlide;
    
    private Entity_Knockback knockbackController;

    // protected virtual void Awake() - need to override it later
    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        stateMachine = new StateMachine();
        knockbackController = GetComponent<Entity_Knockback>();
        collision = GetComponent<Entity_Collision>();
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
        if (knockbackController != null && knockbackController.isKnocked)
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
        collision?.HandleCollisionDetection(facingDirection);
    }
    
    protected virtual void OnDrawGizmos()
    {
    }

    public void CurrentStateAnimationTrigger()
    {
        stateMachine.currentState.AnimationTrigger();
    }
}
