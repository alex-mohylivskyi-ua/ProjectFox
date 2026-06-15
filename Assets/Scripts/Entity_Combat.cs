using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    [Header("Target detection")] 
    [SerializeField] private Transform targetCheck;
    [SerializeField] private float targetCheckRadius = 1;
    [SerializeField] private LayerMask whatIsTarget;
    [SerializeField] private float damage = 1;
    
    private Collider2D[] GetDetectedColliders()
    {
        return Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatIsTarget);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.pink;
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }

    public void PerformAttack()
    {
        foreach (Collider2D target in GetDetectedColliders())
        {
            IDamagable damagable = target.GetComponent<IDamagable>();
            damagable?.TakeDamage(damage, transform);
        }
    }
}
