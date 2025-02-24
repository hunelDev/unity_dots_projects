using System;
using UnityEngine;

namespace Step_1
{
    public class FindNearest : MonoBehaviour
    {
        private void Update()
        {
            float nearestDistSq = float.MaxValue;
            Vector3 nearestPos = default;
            
            foreach (var targetTransform in  Spawner.TargetTransforms)
            {
                 Vector3 diff = targetTransform.position - transform.position;
                float distSq = diff.sqrMagnitude;

                if (nearestDistSq > distSq)
                {
                    nearestDistSq = distSq;
                    nearestPos = targetTransform.position;
                }
            }
            
            Debug.DrawLine(transform.position, nearestPos);
        }
    }
}