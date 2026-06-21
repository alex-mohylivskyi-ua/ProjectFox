using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MovingPlatformPassengerMover : MonoBehaviour
{
    [Header("Passengers")]
    [SerializeField] private LayerMask passengerLayers;
    [SerializeField, Min(0f)] private float topCheckTolerance = 0.05f;

    private readonly List<Rigidbody2D> passengerRigidbodies = new List<Rigidbody2D>();
    private Collider2D platformCollider;

    private void Awake()
    {
        platformCollider = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryAttachPassenger(collision.collider);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryAttachPassenger(collision.collider);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        TryDetachPassenger(collision.collider);
    }

    public void MovePassengers(Vector2 platformDelta)
    {
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

            passengerRb.position += platformDelta;
        }
    }

    private void TryAttachPassenger(Collider2D other)
    {
        if (!LayerIsPassenger(other.gameObject.layer))
        {
            return;
        }

        if (!PassengerIsOnTop(other))
        {
            return;
        }

        Rigidbody2D passengerRb = other.attachedRigidbody;

        if (passengerRb == null)
        {
            return;
        }

        if (!passengerRigidbodies.Contains(passengerRb))
        {
            passengerRigidbodies.Add(passengerRb);
        }
    }

    private void TryDetachPassenger(Collider2D other)
    {
        if (!LayerIsPassenger(other.gameObject.layer))
        {
            return;
        }

        Rigidbody2D passengerRb = other.attachedRigidbody;

        if (passengerRb == null)
        {
            return;
        }

        passengerRigidbodies.Remove(passengerRb);
    }

    private bool PassengerIsOnTop(Collider2D passengerCollider)
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