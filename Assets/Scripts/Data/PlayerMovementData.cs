using UnityEngine;

[CreateAssetMenu(
    fileName = "PlayerMovementData",
    menuName = "Game/Data/Player Movement Data"
)]
public class PlayerMovementData : ScriptableObject
{
    [Header("Movement details")]
    [Range(0, 20)] public float moveSpeed = 8f;

    [Header("Jump details")]
    [Range(0, 20)] public float jumpForce = 20f;
    [Min(0)] public float jumpBufferTime = 0.1f;
    [Min(0)] public float coyoteTime = 0.1f;

    [Header("Jump cut")]
    [Range(0.1f, 1f)] public float jumpCutMultiplier = 0.4f;
    [Range(0f, 20f)] public float jumpCutMinVelocity = 0f;

    [Header("Wall jump")]
    public Vector2 wallJumpForce = new Vector2(8f, 16f);

    [Header("Air details")]
    [Range(0, 1)] public float airMoveMultiplier = 0.9f;
    [Range(0, 100)] public float airMoveDeceleration = 10f;
    [Range(0f, 10f)] public float apexThreshold = 2f;
    [Range(1f, 2f)] public float apexMoveMultiplier = 1.1f;

    [Header("Fall details")]
    [Range(1f, 5f)] public float fallGravityMultiplier = 1.5f;
    [Range(1f, 50f)] public float maxFallSpeed = 18f;
    [Range(0, 1)] public float wallSlideSlowMultiplier = 0.3f;

    [Header("Dash details")]
    [Min(0)] public float dashSpeed = 10f;
    [Min(0)] public float dashDuration = 0.25f;
}