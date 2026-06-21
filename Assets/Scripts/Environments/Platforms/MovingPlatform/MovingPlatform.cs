using System.Collections;
using UnityEngine;

// Для ліфта по кнопці
// В Inspector на MovingPlatform:
//     Movement Mode: Activated
//     Path Mode: PingPong
//     Start Moving On Awake: false
// Wait At Point Duration: 0




public class MovingPlatform : MonoBehaviour
{
    public enum MovementMode
    {
        Automatic,
        Activated
    }

    public enum PathMode
    {
        Loop,
        PingPong,
        Once
    }

    [Header("References")]
    [SerializeField] private Rigidbody2D platformRb;
    [SerializeField] private Transform platform;
    [SerializeField] private Transform platformPath;
    [SerializeField] private MovingPlatformPassengerMover passengerMover;

    [Header("Path")]
    [SerializeField] private PathMode pathMode = PathMode.PingPong;
    [SerializeField] private bool snapPlatformToFirstPointOnStart = true;

    [Header("Movement")]
    [SerializeField, Min(0f)] private float speed = 2f;
    [SerializeField, Min(0f)] private float waitAtPointDuration = 0.25f;
    [SerializeField] private bool startMovingOnAwake = true;

    [Header("Activation")]
    [SerializeField] private MovementMode movementMode = MovementMode.Automatic;

    private Transform[] points;
    private int currentPointIndex;
    private int direction = 1;
    private bool isMoving;
    private bool isWaiting;
    private Coroutine waitCoroutine;

    private void Awake()
    {
        if (platformRb == null && platform != null)
        {
            platformRb = platform.GetComponent<Rigidbody2D>();
        }

        if (platform == null && platformRb != null)
        {
            platform = platformRb.transform;
        }
        
        if (passengerMover == null && platform != null)
        {
            passengerMover = platform.GetComponent<MovingPlatformPassengerMover>();
        }

        if (platformRb == null)
        {
            Debug.LogError($"{nameof(MovingPlatform)} on {name} needs a platform Rigidbody2D reference.", this);
            enabled = false;
            return;
        }

        platformRb.bodyType = RigidbodyType2D.Kinematic;

        CachePathPoints();
    }

    private void Start()
    {
        if (points == null || points.Length == 0)
        {
            Debug.LogWarning($"{nameof(MovingPlatform)} on {name} has no path points.", this);
            enabled = false;
            return;
        }

        if (snapPlatformToFirstPointOnStart)
        {
            platformRb.position = points[0].position;
        }

        currentPointIndex = points.Length > 1 ? 1 : 0;
        isMoving = movementMode == MovementMode.Automatic && startMovingOnAwake;
    }

    private void FixedUpdate()
    {
        if (!isMoving || isWaiting || points == null || points.Length <= 1)
        {
            return;
        }

        MoveToCurrentPoint();
    }

    public void Activate()
    {
        if (points == null || points.Length <= 1)
        {
            return;
        }

        isMoving = true;
    }

    public void Stop()
    {
        isMoving = false;
    }

    public void Toggle()
    {
        if (points == null || points.Length <= 1)
        {
            return;
        }

        isMoving = !isMoving;
    }

    public void MoveToNextPoint()
    {
        if (points == null || points.Length <= 1)
        {
            return;
        }

        if (isWaiting)
        {
            return;
        }

        isMoving = true;
    }

    private void CachePathPoints()
    {
        if (platformPath == null)
        {
            Debug.LogError($"{nameof(MovingPlatform)} on {name} needs a PlatformPath reference.", this);
            points = new Transform[0];
            return;
        }

        int childCount = platformPath.childCount;
        points = new Transform[childCount];

        for (int i = 0; i < childCount; i++)
        {
            points[i] = platformPath.GetChild(i);
        }
    }

    private void MoveToCurrentPoint()
    {
        Vector2 currentPosition = platformRb.position;
        Vector2 targetPosition = points[currentPointIndex].position;

        Vector2 nextPosition = Vector2.MoveTowards(
            currentPosition,
            targetPosition,
            speed * Time.fixedDeltaTime
        );
        
        Vector2 platformDelta = nextPosition - currentPosition;

        platformRb.MovePosition(nextPosition);
        passengerMover?.MovePassengers(platformDelta);

        if (Vector2.Distance(nextPosition, targetPosition) <= 0.01f)
        {
            ReachPoint();
        }
    }

    private void ReachPoint()
    {
        if (movementMode == MovementMode.Activated)
        {
            isMoving = false;
        }

        if (waitAtPointDuration > 0)
        {
            if (waitCoroutine != null)
            {
                StopCoroutine(waitCoroutine);
            }

            waitCoroutine = StartCoroutine(WaitAtPoint());
            return;
        }

        SelectNextPoint();
    }

    private IEnumerator WaitAtPoint()
    {
        isWaiting = true;

        yield return new WaitForSeconds(waitAtPointDuration);

        SelectNextPoint();

        isWaiting = false;
        waitCoroutine = null;
    }

    private void SelectNextPoint()
    {
        switch (pathMode)
        {
            case PathMode.Loop:
                SelectNextLoopPoint();
                break;

            case PathMode.PingPong:
                SelectNextPingPongPoint();
                break;

            case PathMode.Once:
                SelectNextOncePoint();
                break;
        }
    }

    private void SelectNextLoopPoint()
    {
        currentPointIndex++;

        if (currentPointIndex >= points.Length)
        {
            currentPointIndex = 0;
        }
    }

    private void SelectNextPingPongPoint()
    {
        currentPointIndex += direction;

        if (currentPointIndex >= points.Length)
        {
            direction = -1;
            currentPointIndex = points.Length - 2;
        }
        else if (currentPointIndex < 0)
        {
            direction = 1;
            currentPointIndex = 1;
        }
    }

    private void SelectNextOncePoint()
    {
        currentPointIndex++;

        if (currentPointIndex >= points.Length)
        {
            currentPointIndex = points.Length - 1;
            isMoving = false;
        }
    }

    private void OnDrawGizmos()
    {
        Transform path = platformPath;

        if (path == null)
        {
            Transform foundPath = transform.Find("PlatformPath");
            path = foundPath;
        }

        if (path == null || path.childCount == 0)
        {
            return;
        }

        Gizmos.color = Color.cyan;

        for (int i = 0; i < path.childCount; i++)
        {
            Transform point = path.GetChild(i);

            if (point == null)
            {
                continue;
            }

            Gizmos.DrawSphere(point.position, 0.08f);

            int nextIndex = i + 1;

            if (nextIndex < path.childCount)
            {
                Transform nextPoint = path.GetChild(nextIndex);

                if (nextPoint != null)
                {
                    Gizmos.DrawLine(point.position, nextPoint.position);
                }
            }
        }

        if (pathMode == PathMode.Loop && path.childCount > 1)
        {
            Transform firstPoint = path.GetChild(0);
            Transform lastPoint = path.GetChild(path.childCount - 1);

            if (firstPoint != null && lastPoint != null)
            {
                Gizmos.DrawLine(lastPoint.position, firstPoint.position);
            }
        }
    }
}