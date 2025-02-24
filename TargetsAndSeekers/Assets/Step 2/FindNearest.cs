using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Step_2
{
    public class FindNearest : MonoBehaviour
    {
        NativeArray<float3> TargetPositions;
        NativeArray<float3> SeekerPositions;
        NativeArray<float3> NearestTargetPositions;

        private void Start()
        {
            Spawner spawner = FindFirstObjectByType<Spawner>();
            TargetPositions = new NativeArray<float3>(spawner.NumTargets, Allocator.Persistent);
            SeekerPositions = new NativeArray<float3>(spawner.NumSeekers, Allocator.Persistent);
            NearestTargetPositions = new NativeArray<float3>(spawner.NumSeekers, Allocator.Persistent);
        }
        private void Update()
        {

            for (int i = 0; i < TargetPositions.Length; i++)
            {
                TargetPositions[i] = Spawner.TargetTransforms[i].localPosition;
            }

            for (int i = 0; i < SeekerPositions.Length; i++)
            {
                SeekerPositions[i] = Spawner.SeekerTransforms[i].localPosition;
            }

            FindNearestJob job = new()
            {
                SeekerTransforms = SeekerPositions,
                TargetTransforms = TargetPositions,
                NearestTargetPositions = NearestTargetPositions
            };

            JobHandle handle = job.Schedule();
            handle.Complete();

            for (int i = 0; i < NearestTargetPositions.Length; i++)
            {
                Debug.DrawLine(SeekerPositions[i],NearestTargetPositions[i]);
            }

        }

        private void OnDestroy()
        {
            Debug.Log("Destroyed");
            TargetPositions.Dispose();
            SeekerPositions.Dispose();
            NearestTargetPositions.Dispose();
        }
    }
}