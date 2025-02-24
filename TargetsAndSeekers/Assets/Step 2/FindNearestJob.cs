using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Step_2
{
    [BurstCompile]
    public struct FindNearestJob : IJob
    {
        [ReadOnly] public NativeArray<float3> SeekerTransforms;
        [ReadOnly] public NativeArray<float3> TargetTransforms;

        public NativeArray<float3> NearestTargetPositions;

        public void Execute()
        {
            for (int i = 0; i < SeekerTransforms.Length; i++)
            {
                float3 seekerPos = SeekerTransforms[i];
                float nearestDistSq = float.MaxValue;
                for (int j = 0; j < TargetTransforms.Length; j++)
                {
                    float3 targetPos = TargetTransforms[j];
                    float distSq = math.distancesq(seekerPos, targetPos);
                    if (distSq < nearestDistSq)
                    {
                        nearestDistSq = distSq;
                        NearestTargetPositions[i] = targetPos;
                    }
                }
            }
        }
    }
}