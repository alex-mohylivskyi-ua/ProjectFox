using UnityEngine;

public class Entity : MonoBehaviour
{
    // [ContextMenu("Special attack!")]

    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    protected StateMachine stateMachine;
    public string CurrentStateName => stateMachine != null ? stateMachine.CurrentStateName : "None";
    public string PreviousStateName => stateMachine != null ? stateMachine.PreviousStateName : "None";

    [SerializeField] private bool facingRight = true;
    [SerializeField] public int facingDirection = 1;
    

    [Header("Collision detection")]
    protected Entity_Collision collision;
    public bool groundDetected => collision != null && collision.groundDetected;
    public bool wallDetected => collision != null && collision.wallDetected;
    public bool wallSlideSurfaceDetected => collision != null && collision.wallSlideSurfaceDetected;

    // HUD start
    public bool debugGroundHitDetected => collision != null && collision.debugGroundHitDetected;
    public string debugGroundHitName => collision != null ? collision.debugGroundHitName : "None";
    public bool debugGroundHitIsOneWayPlatform => collision != null && collision.debugGroundHitIsOneWayPlatform;
    public bool debugOneWayGroundHitValid => collision != null && collision.debugOneWayGroundHitValid;
    public float debugGroundCheckY => collision != null ? collision.debugGroundCheckY : 0f;
    public float debugGroundCastBottomY => collision != null ? collision.debugGroundCastBottomY : 0f;
    public float debugPlatformTopY => collision != null ? collision.debugPlatformTopY : 0f;
    public float debugDistanceToPlatformTop => collision != null ? collision.debugDistanceToPlatformTop : 0f;
    public float debugOneWayPlatformTopTolerance => collision != null ? collision.debugOneWayPlatformTopTolerance : 0f;
    public float debugOneWayPlatformMaxLandingDistance => collision != null ? collision.debugOneWayPlatformMaxLandingDistance : 0f;

    // HUD end
    
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
