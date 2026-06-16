using UnityEngine;

[CreateAssetMenu(
    fileName = "PlayerCombatData",
    menuName = "Game/Data/Player Combat Data"
)]
public class PlayerCombatData : ScriptableObject
{
    [Header("Basic attack details")]
    public Vector2[] attackVelocity =
    {
        new Vector2(4f, 3f),
        new Vector2(2f, 4f),
        new Vector2(3f, 3f)
    };

    [Min(0)] public float attackVelocityDuration = 0.1f;
    [Min(0)] public float comboResetTime = 1f;

    [Header("Jump attack details")]
    public Vector2 jumpAttackVelocity = new Vector2(3f, -5f);
}