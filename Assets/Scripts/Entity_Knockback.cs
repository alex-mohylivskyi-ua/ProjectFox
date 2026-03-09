using UnityEngine;
using System.Collections;

public class Entity_Knockback : MonoBehaviour
{
    [Header("Knockback details")]
    [SerializeField] private float knockbackDuration = .25f;
    [SerializeField] private Vector2 knockbackDirection = new Vector2(1.5f, 2.5f);
    [SerializeField] private Vector2 heavyKnockbackDirection = new Vector2(1.5f, 2.5f);
    [SerializeField] private float heavyKnockbackDuration = .4f;
    [SerializeField] public float heavyDamageThreshold = .3f; // Percentage of HP you need to lose, to count attack as heavy one
    public bool isKnocked { get; private set; }
    private Coroutine knockbackCoroutine;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private IEnumerator OnKnockbackCoroutine(Transform damageDealer,  bool isHeavyKnockback)
    {
        isKnocked = true;
        float duration = isHeavyKnockback ? heavyKnockbackDuration : knockbackDuration;
        Vector2 knockback = isHeavyKnockback ? heavyKnockbackDirection : knockbackDirection;

        rb.linearVelocity = CalculateKnockbackDirection(damageDealer, knockback);
        yield return new WaitForSeconds(duration);
        
        rb.linearVelocity = Vector2.zero;
        knockbackCoroutine = null;
        isKnocked = false;
    }

    public void ReceiveKnockback(Transform damageDealer, bool isHeavyKnockback)
    {
        if (knockbackCoroutine != null)
        {
            StopCoroutine(knockbackCoroutine);
        }

        knockbackCoroutine = StartCoroutine(OnKnockbackCoroutine(damageDealer, isHeavyKnockback));
    }

    private Vector2 CalculateKnockbackDirection(Transform damageDealer,  Vector2 knockback)
    {
        int direction = transform.position.x > damageDealer.position.x ? 1 : -1;
        knockback.x = direction * knockback.x;

        return knockback;
    }
}
