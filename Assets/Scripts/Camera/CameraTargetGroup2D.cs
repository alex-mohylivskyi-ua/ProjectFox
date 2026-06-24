using System.Collections.Generic;
using UnityEngine;

public class CameraTargetGroup2D : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private List<Transform> targets = new List<Transform>();

    [Header("Position")]
    [SerializeField] private Vector2 offset = new Vector2(0f, 1f);
    [SerializeField] private bool keepInitialZPosition = true;

    [Header("Smoothing")]
    [SerializeField, Min(0f)] private float smoothTime = 0.15f;

    [Header("Debug")]
    [SerializeField] private bool drawDebugGizmos = true;
    [SerializeField, Min(0f)] private float targetGizmoRadius = 0.12f;

    private Vector3 smoothVelocity;
    private float initialZPosition;

    public int TargetCount => GetValidTargetCount();
    public Vector3 CurrentCenter { get; private set; }

    private void Awake()
    {
        initialZPosition = transform.position.z;
        RemoveNullTargets();
    }

    private void LateUpdate()
    {
        RemoveNullTargets();

        if (!TryGetTargetsCenter(out Vector3 targetsCenter))
        {
            return;
        }

        CurrentCenter = targetsCenter;

        Vector3 desiredPosition = targetsCenter + (Vector3)offset;

        if (keepInitialZPosition)
        {
            desiredPosition.z = initialZPosition;
        }

        if (smoothTime <= 0f)
        {
            transform.position = desiredPosition;
            return;
        }

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref smoothVelocity,
            smoothTime
        );
    }

    public void RegisterTarget(Transform target)
    {
        if (target == null)
        {
            return;
        }

        if (!targets.Contains(target))
        {
            targets.Add(target);
        }
    }

    public void UnregisterTarget(Transform target)
    {
        if (target == null)
        {
            return;
        }

        targets.Remove(target);
    }

    public void ClearTargets()
    {
        targets.Clear();
    }

    private bool TryGetTargetsCenter(out Vector3 center)
    {
        center = Vector3.zero;

        int validTargetCount = 0;

        for (int i = 0; i < targets.Count; i++)
        {
            Transform target = targets[i];

            if (target == null)
            {
                continue;
            }

            center += target.position;
            validTargetCount++;
        }

        if (validTargetCount == 0)
        {
            center = transform.position;
            return false;
        }

        center /= validTargetCount;
        return true;
    }

    private int GetValidTargetCount()
    {
        int validTargetCount = 0;

        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] != null)
            {
                validTargetCount++;
            }
        }

        return validTargetCount;
    }

    private void RemoveNullTargets()
    {
        for (int i = targets.Count - 1; i >= 0; i--)
        {
            if (targets[i] == null)
            {
                targets.RemoveAt(i);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawDebugGizmos || targets == null || targets.Count == 0)
        {
            return;
        }

        Gizmos.color = Color.yellow;

        Vector3 center = Vector3.zero;
        int validTargetCount = 0;

        for (int i = 0; i < targets.Count; i++)
        {
            Transform target = targets[i];

            if (target == null)
            {
                continue;
            }

            Gizmos.DrawWireSphere(target.position, targetGizmoRadius);

            center += target.position;
            validTargetCount++;
        }

        if (validTargetCount == 0)
        {
            return;
        }

        center /= validTargetCount;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(center, targetGizmoRadius * 1.5f);

        Vector3 offsetCenter = center + (Vector3)offset;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(offsetCenter, targetGizmoRadius * 2f);
        Gizmos.DrawLine(center, offsetCenter);
    }
}