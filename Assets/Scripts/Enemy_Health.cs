using UnityEngine;

public class Enemy_Health : Entity_Health
{
    private Enemy enemy;

    protected override void Awake()
    {
        base.Awake();
        
        enemy = GetComponent<Enemy>();
    }

    public override void TakeDamage(float damage, Transform damageDealer)
    {
        base.TakeDamage(damage, damageDealer);
        
        if (damageDealer.CompareTag("Player"))
            enemy.TryEnterBattleState(damageDealer);
    }
}
