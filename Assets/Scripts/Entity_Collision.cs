using UnityEngine;

public class Entity_Collision : MonoBehaviour
{
    [Header("Ground detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.7f, 0.08f);
    [SerializeField, Min(0f)] private float oneWayPlatformTopTolerance = 0.05f;
    [SerializeField, Min(0f)] private float oneWayPlatformMaxLandingDistance = 0.08f;

    [Header("Wall detection")]
    [SerializeField] private Transform topWallCheck;
    [SerializeField] private Transform bottomWallCheck;
    [SerializeField] private float wallCheckDistance;

    [Header("Layers")]
    [SerializeField] private LayerMask whatIsGround;

    public bool groundDetected { get; private set; }
    public bool wallDetected { get; private set; }
    public bool wallSlideSurfaceDetected { get; private set; }

    public float groundCheckDistanceValue => groundCheckDistance;
    public LayerMask whatIsGroundValue => whatIsGround;

    // HUD start
    public bool debugGroundHitDetected { get; private set; }
    public string debugGroundHitName { get; private set; } = "None";
    public bool debugGroundHitIsOneWayPlatform { get; private set; }
    public bool debugGroundHitIsMovingPlatform { get; private set; }
    public string debugGroundHitMovingPlatformName { get; private set; } = "None";
    public bool debugOneWayGroundHitValid { get; private set; }
    public float debugGroundCheckY { get; private set; }
    public float debugGroundCastBottomY { get; private set; }
    public float debugPlatformTopY { get; private set; }
    public float debugDistanceToPlatformTop { get; private set; }
    public float debugOneWayPlatformTopTolerance => oneWayPlatformTopTolerance;
    public float debugOneWayPlatformMaxLandingDistance => oneWayPlatformMaxLandingDistance;
    // HUD end

    public void HandleCollisionDetection(int facingDirection)
    {
        RaycastHit2D topWallHit = default;
        if (topWallCheck != null)
        {
            topWallHit = Physics2D.Raycast(
                topWallCheck.position,
                Vector2.right * facingDirection,
                wallCheckDistance,
                whatIsGround
            );
        }

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

        RaycastHit2D groundHit = default;
        if (groundCheck != null)
        {
            groundHit = Physics2D.BoxCast(
                groundCheck.position,
                groundCheckSize,
                0f,
                Vector2.down,
                groundCheckDistance,
                whatIsGround
            );
        }

        wallDetected = topWallHit || bottomWallHit;
        wallSlideSurfaceDetected = topWallHit && bottomWallHit;
        groundDetected = IsValidGroundHit(groundHit);
    }

    private bool IsValidGroundHit(RaycastHit2D groundHit)
    {
        debugGroundHitDetected = groundHit;
        debugGroundHitName = groundHit ? groundHit.collider.name : "None";
        debugGroundHitIsOneWayPlatform = false;
        debugGroundHitIsMovingPlatform = false;
        debugGroundHitMovingPlatformName = "None";
        debugOneWayGroundHitValid = false;
        debugGroundCheckY = groundCheck != null ? groundCheck.position.y : 0f;
        debugGroundCastBottomY = groundCheck != null
            ? groundCheck.position.y - groundCheckDistance - groundCheckSize.y * 0.5f
            : 0f;
        debugPlatformTopY = 0f;
        debugDistanceToPlatformTop = 0f;

        if (!groundHit)
        {
            return false;
        }

        MovingPlatformPassengerMover movingPlatform = groundHit.collider.GetComponentInParent<MovingPlatformPassengerMover>();

        if (movingPlatform != null)
        {
            debugGroundHitIsMovingPlatform = true;
            debugGroundHitMovingPlatformName = movingPlatform.PlatformName;
        }

        OneWayPlatform oneWayPlatform = groundHit.collider.GetComponent<OneWayPlatform>();

        if (oneWayPlatform == null)
        {
            return true;
        }

        debugGroundHitIsOneWayPlatform = true;

        if (groundCheck == null)
        {
            return false;
        }

        debugPlatformTopY = groundHit.collider.bounds.max.y;
        debugDistanceToPlatformTop = debugGroundCastBottomY - debugPlatformTopY;

        debugOneWayGroundHitValid =
            debugDistanceToPlatformTop >= -oneWayPlatformTopTolerance &&
            debugDistanceToPlatformTop <= oneWayPlatformMaxLandingDistance;

        return debugOneWayGroundHitValid;
    }

    private void OnDrawGizmos()
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
            Gizmos.DrawLine(topWallCheck.position, topWallCheck.position + Vector3.right * wallCheckDistance);
        }

        if (bottomWallCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(bottomWallCheck.position, bottomWallCheck.position + Vector3.right * wallCheckDistance);
        }
    }
}