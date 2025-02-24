using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DefaultNamespace
{
    [BurstCompile]
    public partial struct BallSystem : ISystem
    {
   
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BallData>();
        }


        public void OnUpdate(ref SystemState state)
        {
            
            BallJob job = new BallJob()
            {
                DeltaTime = SystemAPI.Time.DeltaTime
            };

            job.ScheduleParallel();

        }


        public void OnDestroy(ref SystemState state)
        {
           
        }
    }


    [BurstCompile]
    partial struct BallJob : IJobEntity
    {
        public float DeltaTime;
        public void Execute(ref BallData ballData, ref  LocalTransform transform, ref EntityGuid guid)
        {
            transform = transform.Translate(ballData.Direction * ballData.Speed * DeltaTime);

        }
    }
}