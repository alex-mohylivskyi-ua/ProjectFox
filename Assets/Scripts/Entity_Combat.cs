using System;
using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    [Header("Target detection")] 
    [SerializeField] private Transform targetCheck;
    [SerializeField] private float targetCheckRadius = 1;
    [SerializeField] private LayerMask whatIsTarget;
    
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
        GetDetectedColliders();

        foreach (Collider2D target in GetDetectedColliders())
        {
            Debug.Log(target.name);
        }
    }
}
