using System.Collections.Generic;
using UnityEngine;

public class Hazards : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private float damage = 1f;
    [SerializeField] private float damageCooldown = .5f;
    [SerializeField] private bool isInstantKill = false;

    [Header("Detection")]
    [SerializeField] private LayerMask whatCanBeDamaged;
    [SerializeField] private bool damageOnEnter = true;
    [SerializeField] private bool damageOnStay = true;

    private readonly Dictionary<IDamagable, float> lastDamageTimeByTarget = new Dictionary<IDamagable, float>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!damageOnEnter)
            return;

        TryDamage(other);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        RemoveTarget(other);
    }
    
    private void RemoveTarget(Collider2D other)
    {
        IDamagable damagable = other.GetComponent<IDamagable>();
        if (damagable != null)
            lastDamageTimeByTarget.Remove(damagable);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!damageOnStay)
            return;

        TryDamage(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!damageOnEnter)
            return;

        TryDamage(collision.collider);
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        RemoveTarget(collision.collider);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!damageOnStay)
            return;

        TryDamage(collision.collider);
    }

    private void TryDamage(Collider2D other)
    {
        if (!CanDamageLayer(other.gameObject.layer))
            return;

        IDamagable damagable = other.GetComponent<IDamagable>();

        if (damagable == null)
            return;

        if (!CanDamageTarget(damagable))
            return;
        
        float appliedDamage = isInstantKill ? Mathf.Infinity : damage;
        damagable.TakeDamage(appliedDamage, transform);
        lastDamageTimeByTarget[damagable] = Time.time;
    }

    private bool CanDamageLayer(int targetLayer)
    {
        return (whatCanBeDamaged.value & (1 << targetLayer)) != 0;
    }

    private bool CanDamageTarget(IDamagable damagable)
    {
        if (!lastDamageTimeByTarget.TryGetValue(damagable, out float lastDamageTime))
            return true;

        return Time.time >= lastDamageTime + damageCooldown;
    }
}