using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class MovingP : MonoBehaviour
{
    [Header("Waypoints")]
    [SerializeField] private Vector2[] localWaypoints;
    [SerializeField] private float speed = 3f;
    [SerializeField] private bool cyclic = false;
    [SerializeField] private float waitTime = 0f;
    [SerializeField, Range(0f, 2f)] private float easeAmount = 1f;

    [Header("Passenger Detection")]
    [SerializeField] private LayerMask passengerMask;
    [SerializeField] private float skinWidth = 0.015f;
    [SerializeField] private int horizontalRayCount = 4;
    [SerializeField] private int verticalRayCount = 4;

    // ─── private state ────────────────────────────────────────────────────────

    private Rigidbody2D        rb;
    private BoxCollider2D      col;
    private Vector2[]          globalWaypoints;
    private int                fromWaypointIndex;
    private float              percentBetweenWaypoints;
    private float              nextMoveTime;
    private float              horizontalRaySpacing;
    private float              verticalRaySpacing;
    private RaycastOrigins     raycastOrigins;

    // Кешуємо Rigidbody2D пасажирів, щоб не робити GetComponent кожен FixedUpdate
    private readonly Dictionary<Transform, Rigidbody2D> passengerRbCache = new();
    private readonly HashSet<Transform>                  movedThisFrame   = new();
    private          List<PassengerMovement>             passengerMoves;

    // ─── structs ──────────────────────────────────────────────────────────────

    private struct RaycastOrigins
    {
        public Vector2 topLeft, topRight, bottomLeft, bottomRight;
    }

    private struct PassengerMovement
    {
        public Transform transform;
        public Vector2   velocity;
        public bool      standingOnPlatform; // стоїть зверху (true) vs збоку (false)
        public bool      moveBeforePlatform; // рухати ДО або ПІСЛЯ платформи
    }

    // ─── lifecycle ────────────────────────────────────────────────────────────

    private void Awake()
    {
        rb  = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();

        rb.isKinematic  = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; // плавність між FixedUpdate

        // Конвертуємо локальні waypoints у world-space один раз
        globalWaypoints = new Vector2[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
            globalWaypoints[i] = localWaypoints[i] + (Vector2)transform.position;
    }

    private void Start() => CalculateRaySpacing();

    private void FixedUpdate()
    {
        UpdateRaycastOrigins();

        Vector2 platformDelta = CalculatePlatformMovement();
        CalculatePassengerMovement(platformDelta);

        MovePassengers(beforePlatform: true);   // тих, кого штовхає платформа — рухаємо ПЕРШИМИ
        rb.MovePosition(rb.position + platformDelta);
        MovePassengers(beforePlatform: false);  // тих, хто стоїть зверху — рухаємо ПІСЛЯ
    }

    // ─── platform movement ────────────────────────────────────────────────────

    private Vector2 CalculatePlatformMovement()
    {
        if (Time.time < nextMoveTime) return Vector2.zero;

        fromWaypointIndex %= globalWaypoints.Length;
        int   toIndex   = (fromWaypointIndex + 1) % globalWaypoints.Length;
        float dist      = Vector2.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toIndex]);

        percentBetweenWaypoints += Time.fixedDeltaTime * speed / dist;
        percentBetweenWaypoints  = Mathf.Clamp01(percentBetweenWaypoints);

        Vector2 newPos = Vector2.Lerp(
            globalWaypoints[fromWaypointIndex],
            globalWaypoints[toIndex],
            Ease(percentBetweenWaypoints)
        );

        if (percentBetweenWaypoints >= 1f)
        {
            percentBetweenWaypoints = 0f;
            fromWaypointIndex++;

            if (!cyclic && fromWaypointIndex >= globalWaypoints.Length - 1)
            {
                fromWaypointIndex = 0;
                System.Array.Reverse(globalWaypoints); // ping-pong
            }

            nextMoveTime = Time.time + waitTime;
        }

        return newPos - rb.position;
    }

    // Ease in-out: плавне прискорення / гальмування на waypoints
    private float Ease(float t)
    {
        float a = easeAmount + 1f;
        return Mathf.Pow(t, a) / (Mathf.Pow(t, a) + Mathf.Pow(1f - t, a));
    }

    // ─── passenger detection ──────────────────────────────────────────────────

    private void CalculatePassengerMovement(Vector2 velocity)
    {
        movedThisFrame.Clear();
        passengerMoves = new List<PassengerMovement>();

        float dirX = Mathf.Sign(velocity.x);
        float dirY = Mathf.Sign(velocity.y);

        // 1. Вертикальний рух — платформа штовхає пасажира вгору або вниз
        if (velocity.y != 0f)
        {
            float rayLen = Mathf.Abs(velocity.y) + skinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 origin = (dirY == -1 ? raycastOrigins.bottomLeft : raycastOrigins.topLeft)
                               + Vector2.right * (verticalRaySpacing * i);

                RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up * dirY, rayLen, passengerMask);
                if (!hit || hit.distance == 0f || movedThisFrame.Contains(hit.transform)) continue;

                movedThisFrame.Add(hit.transform);
                passengerMoves.Add(new PassengerMovement
                {
                    transform          = hit.transform,
                    velocity           = new Vector2(dirY == 1 ? velocity.x : 0f,
                                                     velocity.y - (hit.distance - skinWidth) * dirY),
                    standingOnPlatform = dirY == 1,
                    moveBeforePlatform = true  // платформа іде вгору → пасажир ПЕРШИМ
                });
            }
        }

        // 2. Горизонтальний рух — платформа штовхає пасажира збоку
        if (velocity.x != 0f)
        {
            float rayLen = Mathf.Abs(velocity.x) + skinWidth;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 origin = (dirX == -1 ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight)
                               + Vector2.up * (horizontalRaySpacing * i);

                RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * dirX, rayLen, passengerMask);
                if (!hit || hit.distance == 0f || movedThisFrame.Contains(hit.transform)) continue;

                movedThisFrame.Add(hit.transform);
                passengerMoves.Add(new PassengerMovement
                {
                    transform          = hit.transform,
                    velocity           = new Vector2(velocity.x - (hit.distance - skinWidth) * dirX, -skinWidth),
                    standingOnPlatform = false,
                    moveBeforePlatform = true
                });
            }
        }

        // 3. Пасажир стоїть ЗВЕРХУ на платформі, що рухається вниз або горизонтально
        //    Якщо пропустити цей кейс — пасажир не поїде з горизонтальною платформою
        if (dirY == -1 || (velocity.y == 0f && velocity.x != 0f))
        {
            float rayLen = skinWidth * 2f;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 origin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);

                RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, rayLen, passengerMask);
                if (!hit || hit.distance == 0f || movedThisFrame.Contains(hit.transform)) continue;

                movedThisFrame.Add(hit.transform);
                passengerMoves.Add(new PassengerMovement
                {
                    transform          = hit.transform,
                    velocity           = new Vector2(velocity.x, velocity.y),
                    standingOnPlatform = true,
                    moveBeforePlatform = false  // платформа іде вниз → пасажир ПІСЛЯ
                });
            }
        }
    }

    // ─── passenger movement ───────────────────────────────────────────────────

    private void MovePassengers(bool beforePlatform)
    {
        foreach (PassengerMovement p in passengerMoves)
        {
            if (p.moveBeforePlatform != beforePlatform) continue;

            if (!passengerRbCache.TryGetValue(p.transform, out Rigidbody2D passengerRb))
            {
                passengerRb = p.transform.GetComponent<Rigidbody2D>();
                passengerRbCache[p.transform] = passengerRb;
            }

            if (passengerRb != null)
                passengerRb.MovePosition(passengerRb.position + p.velocity);
            else
                p.transform.Translate(p.velocity); // fallback для об'єктів без Rigidbody2D
        }
    }

    // ─── raycasting helpers ───────────────────────────────────────────────────

    private void UpdateRaycastOrigins()
    {
        Bounds b = col.bounds;
        b.Expand(skinWidth * -2f);

        raycastOrigins.bottomLeft  = new Vector2(b.min.x, b.min.y);
        raycastOrigins.bottomRight = new Vector2(b.max.x, b.min.y);
        raycastOrigins.topLeft     = new Vector2(b.min.x, b.max.y);
        raycastOrigins.topRight    = new Vector2(b.max.x, b.max.y);
    }

    private void CalculateRaySpacing()
    {
        Bounds b = col.bounds;
        b.Expand(skinWidth * -2f);

        horizontalRayCount = Mathf.Max(horizontalRayCount, 2);
        verticalRayCount   = Mathf.Max(verticalRayCount,   2);

        horizontalRaySpacing = b.size.y / (horizontalRayCount - 1);
        verticalRaySpacing   = b.size.x / (verticalRayCount   - 1);
    }

    // ─── editor gizmos ────────────────────────────────────────────────────────

    private void OnDrawGizmosSelected()
    {
        if (localWaypoints == null) return;
        Gizmos.color = Color.red;
        const float s = 0.3f;

        for (int i = 0; i < localWaypoints.Length; i++)
        {
            Vector2 p = Application.isPlaying
                ? globalWaypoints[i]
                : (Vector2)transform.position + localWaypoints[i];

            Gizmos.DrawLine(p + Vector2.down  * s, p + Vector2.up    * s);
            Gizmos.DrawLine(p + Vector2.left  * s, p + Vector2.right * s);

            if (i < localWaypoints.Length - 1)
            {
                Vector2 next = Application.isPlaying
                    ? globalWaypoints[i + 1]
                    : (Vector2)transform.position + localWaypoints[i + 1];
                Gizmos.DrawLine(p, next);
            }
        }
    }
}