using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MovingPlatformPassengerMover : MonoBehaviour
{
    [Header("Passengers")]
    [SerializeField] private LayerMask passengerLayers;
    [SerializeField, Range(0f, 1f)] private float minimumTopContactNormalY = 0.5f;

    [Header("Fallback Top Check")]
    [SerializeField, Min(0f)] private float topCheckTolerance = 0.08f;

    [Header("Detach")]
    [SerializeField, Min(0f)] private float detachGraceDuration = 0.08f;

    private readonly List<Rigidbody2D> passengerRigidbodies = new List<Rigidbody2D>();
    private readonly Dictionary<Rigidbody2D, float> pendingDetachTimes = new Dictionary<Rigidbody2D, float>();

    private Collider2D platformCollider;
    private Vector2 lastPlatformDelta;
    private Vector2 lastAppliedPassengerDelta;
    private string lastAttachedPassengerName = "None";
    private string lastDetachedPassengerName = "None";
    private string lastAttachRejectReason = "None";
    private string lastDetachReason = "None";
    private float lastContactNormalY;
    private bool lastHadTopContact;
    private bool lastPassedBoundsTopCheck;

    public int PassengerCount => passengerRigidbodies.Count;
    public Vector2 LastPlatformDelta => lastPlatformDelta;
    public Vector2 LastAppliedPassengerDelta => lastAppliedPassengerDelta;
    public string LastAttachedPassengerName => lastAttachedPassengerName;
    public string LastDetachedPassengerName => lastDetachedPassengerName;
    public string LastAttachRejectReason => lastAttachRejectReason;
    public string LastDetachReason => lastDetachReason;
    public float LastContactNormalY => lastContactNormalY;
    public bool LastHadTopContact => lastHadTopContact;
    public bool LastPassedBoundsTopCheck => lastPassedBoundsTopCheck;
    public string PlatformName => gameObject.name;

    private void Awake()
    {
        platformCollider = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        ProcessPendingDetaches();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryAttachPassenger(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryAttachPassenger(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        TryDetachPassenger(collision.collider);
    }

    public void MovePassengers(Vector2 platformDelta)
    {
        lastPlatformDelta = platformDelta;
        lastAppliedPassengerDelta = Vector2.zero;

        if (platformDelta == Vector2.zero)
        {
            return;
        }

        for (int i = passengerRigidbodies.Count - 1; i >= 0; i--)
        {
            Rigidbody2D passengerRb = passengerRigidbodies[i];

            if (passengerRb == null)
            {
                passengerRigidbodies.RemoveAt(i);
                continue;
            }

            MovePassenger(passengerRb, platformDelta);
        }
    }

    public bool HasPassenger(Rigidbody2D passengerRb)
    {
        if (passengerRb == null)
        {
            return false;
        }

        return passengerRigidbodies.Contains(passengerRb);
    }

    private void MovePassenger(Rigidbody2D passengerRb, Vector2 platformDelta)
    {
        // Horizontal platform movement must be applied manually, otherwise a dynamic
        // velocity-driven player can slide off or fail to inherit diagonal platform X movement.
        //
        // Upward platform movement is already resolved by Unity's collision solver.
        // Applying positive Y delta here as well causes the player to be lifted twice
        // and creates strong bouncing on fast elevators.
        //
        // Downward platform movement is still applied manually so the platform does not
        // move away from under the player between physics contacts.
        Vector2 passengerDelta = new Vector2(
            platformDelta.x,
            platformDelta.y < 0f ? platformDelta.y : 0f
        );

        lastAppliedPassengerDelta = passengerDelta;
        passengerRb.transform.position += (Vector3)passengerDelta;
    }

    private void TryAttachPassenger(Collision2D collision)
    {
        lastHadTopContact = false;
        lastPassedBoundsTopCheck = false;

        if (collision == null || collision.collider == null)
        {
            lastAttachRejectReason = "Collision missing";
            return;
        }

        Collider2D passengerCollider = collision.collider;

        if (!LayerIsPassenger(passengerCollider.gameObject.layer))
        {
            lastAttachRejectReason = "Layer is not passenger";
            return;
        }

        Rigidbody2D passengerRb = passengerCollider.attachedRigidbody;

        if (passengerRb == null)
        {
            lastAttachRejectReason = "Passenger Rigidbody missing";
            return;
        }

        lastHadTopContact = PassengerHasTopContact(collision);
        lastPassedBoundsTopCheck = PassengerLooksAbovePlatform(passengerCollider);

        if (!lastHadTopContact && !lastPassedBoundsTopCheck)
        {
            lastAttachRejectReason = "No valid top contact";
            return;
        }

        AttachPassenger(passengerRb);
    }

    private void AttachPassenger(Rigidbody2D passengerRb)
    {
        if (passengerRb == null)
        {
            return;
        }

        lastAttachRejectReason = "None";
        pendingDetachTimes.Remove(passengerRb);

        if (!passengerRigidbodies.Contains(passengerRb))
        {
            passengerRigidbodies.Add(passengerRb);
            lastAttachedPassengerName = passengerRb.name;
        }
    }

    private void TryDetachPassenger(Collider2D other)
    {
        if (other == null)
        {
            return;
        }

        if (!LayerIsPassenger(other.gameObject.layer))
        {
            return;
        }

        Rigidbody2D passengerRb = other.attachedRigidbody;

        if (passengerRb == null)
        {
            return;
        }

        ScheduleDetachPassenger(passengerRb);
    }

    private void ScheduleDetachPassenger(Rigidbody2D passengerRb)
    {
        if (passengerRb == null)
        {
            return;
        }

        pendingDetachTimes[passengerRb] = Time.time + detachGraceDuration;
        lastDetachReason = "Collision exit scheduled";
    }

    private void ProcessPendingDetaches()
    {
        if (pendingDetachTimes.Count == 0)
        {
            return;
        }

        List<Rigidbody2D> readyToDetach = null;

        foreach (KeyValuePair<Rigidbody2D, float> pendingDetach in pendingDetachTimes)
        {
            Rigidbody2D passengerRb = pendingDetach.Key;
            float detachTime = pendingDetach.Value;

            if (passengerRb == null || Time.time >= detachTime)
            {
                readyToDetach ??= new List<Rigidbody2D>();
                readyToDetach.Add(passengerRb);
            }
        }

        if (readyToDetach == null)
        {
            return;
        }

        for (int i = 0; i < readyToDetach.Count; i++)
        {
            Rigidbody2D passengerRb = readyToDetach[i];

            pendingDetachTimes.Remove(passengerRb);
            DetachPassenger(passengerRb, "Collision exit grace expired");
        }
    }

    private void DetachPassenger(Rigidbody2D passengerRb, string reason)
    {
        if (passengerRb == null)
        {
            return;
        }

        if (passengerRigidbodies.Remove(passengerRb))
        {
            lastDetachedPassengerName = passengerRb.name;
            lastDetachReason = reason;
        }
    }

    private bool PassengerHasTopContact(Collision2D collision)
    {
        lastContactNormalY = 0f;

        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint2D contact = collision.GetContact(i);
            lastContactNormalY = contact.normal.y;

            if (contact.normal.y >= minimumTopContactNormalY)
            {
                return true;
            }
        }

        return false;
    }

    private bool PassengerLooksAbovePlatform(Collider2D passengerCollider)
    {
        if (platformCollider == null || passengerCollider == null)
        {
            return false;
        }

        float passengerBottomY = passengerCollider.bounds.min.y;
        float platformTopY = platformCollider.bounds.max.y;

        return passengerBottomY >= platformTopY - topCheckTolerance;
    }

    private bool LayerIsPassenger(int layer)
    {
        return (passengerLayers.value & (1 << layer)) != 0;
    }
}