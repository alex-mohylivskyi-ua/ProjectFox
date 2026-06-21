using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class OneWayPlatform : MonoBehaviour
{
    [SerializeField, Min(0.05f)] private float restoreCollisionDelay = 0.35f;

    public float RestoreCollisionDelay => restoreCollisionDelay;
}