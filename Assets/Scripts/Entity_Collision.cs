using UnityEngine;

public class Entity_Collision : MonoBehaviour
{
    [Header("Ground detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.7f, 0.08f);

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
        groundDetected = groundHit;
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