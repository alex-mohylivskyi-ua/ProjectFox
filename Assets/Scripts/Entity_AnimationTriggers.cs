using UnityEngine;

public class Entity_AnimationTriggers : MonoBehaviour
{
    private Entity entity;
    private Entity_Combat entityCombat;

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
        entityCombat = GetComponentInParent<Entity_Combat>();
    }

    private void CurrentStateTrigger()
    {
        // Debug.Log("Attack over!");
        entity.CurrentStateAnimationTrigger();
    }

    private void AttackTrigger()
    {
        Debug.Log("Attack");
        entityCombat.PerformAttack();
    }
}
