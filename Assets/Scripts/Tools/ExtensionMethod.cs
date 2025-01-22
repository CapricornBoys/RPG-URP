using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethod
{
    private const float dotThreshold = 0.5f;

    public static bool isFacingTarget(this Transform transform, Transform target)
    {
        var targetDirection = target.position - transform.position;
        targetDirection.Normalize();

        float dot = Vector3.Dot(transform.forward, targetDirection);

        return dot >= dotThreshold;       

    }
}
