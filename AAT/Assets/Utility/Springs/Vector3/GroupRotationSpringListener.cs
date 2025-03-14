using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroupRotationSpringListener : Vector3SpringListener
{
    [SerializeField] private List<Transform> targetTransforms;

    protected override Vector3 GetOrig()
    {
        return targetTransforms.First().localRotation.eulerAngles;
    }

    protected override void ChangeValue(Vector3 value)
    {
        foreach (var targetTransform in targetTransforms)
        {
            targetTransform.localRotation = Quaternion.Euler(value);
        }
    }
}
