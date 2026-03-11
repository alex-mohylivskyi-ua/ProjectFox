using UnityEngine;

public class Entity_Health : MonoBehaviour
{
    [SerializeField] protected float maxHP = 3;
    private float currentHP;
    [SerializeField] protected bool isDead;
    
    private Entity_VFX entityVFX;
    private Entity_Knockback knockbackController;
    private Entity entity;

    protected virtual void Awake()
    {
        currentHP = maxHP;
        entityVFX = GetComponent<Entity_VFX>();
        knockbackController = GetComponent<Entity_Knockback>();
        entity = GetComponent<Entity>();
    }

    public virtual void TakeDamage(float damage, Transform damageDealer)
    {
        if (isDead)
            return;
        
        knockbackController?.ReceiveKnockback(damageDealer, IsHeavyDamage(damage));
        entityVFX?.PlayOnDamageVFX();
        ReduceHP(damage);
    }

    private void ReduceHP(float damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
            Die();
    }

    private void Die()
    {
        isDead = true;
        entity.EntityDeath();
        Debug.Log("Died: " + isDead);
    }

    private bool IsHeavyDamage(float damage)
    {
        return damage >= maxHP * knockbackController.heavyDamageThreshold;
    }
}
