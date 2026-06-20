using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(Collider2D))]
public class PlayerOneWayPlatformDrop : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private LayerMask oneWayPlatformLayer;

    [Header("Input")]
    [SerializeField, Range(-1f, 0f)] private float downInputThreshold = -0.5f;

    [Header("Drop")]
    [SerializeField, Min(0f)] private float dropStartVelocity = 0.5f;

    private Player player;
    private Collider2D playerCollider;
    private Rigidbody2D rb;
    private Collider2D currentPlatformCollider;
    private OneWayPlatform currentPlatform;
    private Collider2D ignoredPlatformCollider;
    private Coroutine restoreCollisionCoroutine;
    
    // HUD start
    public bool HasCurrentPlatform => currentPlatformCollider != null && currentPlatform != null;
    public string CurrentPlatformName => currentPlatformCollider != null ? currentPlatformCollider.name : "None";
    public bool DropInputDetected => player != null &&
                                     player.inputReader != null &&
                                     player.inputReader.jumpPressed &&
                                     player.moveInput.y <= downInputThreshold;
    public bool IsIgnoringPlatformCollision => ignoredPlatformCollider != null;
    // HUD end

    private void Awake()
    {
        player = GetComponent<Player>();
        playerCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        if (playerCollider == null)
        {
            Debug.LogError($"{nameof(PlayerOneWayPlatformDrop)} needs a Collider2D on {name}.", this);
        }

        if (rb == null)
        {
            Debug.LogError($"{nameof(PlayerOneWayPlatformDrop)} needs a Rigidbody2D on {name}.", this);
        }
    }

    public bool TryDropThroughPlatform()
    {
        if (player == null || player.inputReader == null)
        {
            return false;
        }

        if (currentPlatformCollider == null || currentPlatform == null)
        {
            return false;
        }

        if (!ShouldDropThroughPlatform())
        {
            return false;
        }

        DropThroughCurrentPlatform();
        return true;
    }

    private bool ShouldDropThroughPlatform()
    {
        return player.inputReader.jumpPressed &&
               player.moveInput.y <= downInputThreshold &&
               player.groundDetected;
    }

    private void DropThroughCurrentPlatform()
    {
        if (restoreCollisionCoroutine != null)
        {
            StopCoroutine(restoreCollisionCoroutine);
        }

        Collider2D platformCollider = currentPlatformCollider;
        float restoreDelay = currentPlatform.RestoreCollisionDelay;

        Physics2D.IgnoreCollision(playerCollider, platformCollider, true);
        ignoredPlatformCollider = platformCollider;

        if (rb != null && rb.linearVelocity.y > -dropStartVelocity)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -dropStartVelocity);
        }

        restoreCollisionCoroutine = StartCoroutine(RestoreCollisionAfterDelay(
            platformCollider,
            restoreDelay
        ));

        currentPlatformCollider = null;
        currentPlatform = null;
    }

    private IEnumerator RestoreCollisionAfterDelay(Collider2D platformCollider, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (playerCollider != null && platformCollider != null)
        {
            Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
        }
        
        // HUD start
        if (ignoredPlatformCollider == platformCollider)
        {
            ignoredPlatformCollider = null;
        }
        // HUD end

        restoreCollisionCoroutine = null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TrySetCurrentPlatform(collision.collider);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TrySetCurrentPlatform(collision.collider);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider == currentPlatformCollider)
        {
            currentPlatformCollider = null;
            currentPlatform = null;
        }
    }

    private void TrySetCurrentPlatform(Collider2D other)
    {
        if (!LayerIsOneWayPlatform(other.gameObject.layer))
        {
            return;
        }
        
        OneWayPlatform platform = other.GetComponentInParent<OneWayPlatform>();

        if (platform == null)
        {
            return;
        }

        currentPlatformCollider = other;
        currentPlatform = platform;
    }

    private bool LayerIsOneWayPlatform(int layer)
    {
        return (oneWayPlatformLayer.value & (1 << layer)) != 0;
    }
}