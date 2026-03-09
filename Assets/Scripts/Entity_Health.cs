using UnityEngine;

public class Entity_Health : MonoBehaviour
{
    [SerializeField] protected float maxHP = 3;
    [SerializeField] protected bool isDead;
    
    private Entity_VFX entityVFX;

    protected virtual void Awake()
    {
        entityVFX = GetComponent<Entity_VFX>();
    }

    public virtual void TakeDamage(float damage, Transform damageDealer)
    {
        if (isDead)
            return;
        entityVFX?.PlayOnDamageVFX();
        ReduceHP(damage);
    }

    private void ReduceHP(float damage)
    {
        maxHP -= damage;

        if (maxHP <= 0)
            Die();
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Died: " + isDead);
    }


}
