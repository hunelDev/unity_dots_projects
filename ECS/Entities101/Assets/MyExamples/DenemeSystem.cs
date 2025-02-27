using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace MyExamples
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct DenemeSystem : ISystem,ISystemStartStop
    {
        EntityQuery query;
        EntityQuery query2;

        private bool isDone;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Monster>();
            Debug.Log("onCreateOn");
            isDone = false;
            query = state.GetEntityQuery(ComponentType.ReadWrite<Monster>());
            query2 = state.GetEntityQuery(ComponentType.ReadWrite<DenemeData>());

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var monsters = query.ToEntityArray(Allocator.Temp);

            Debug.Log($"{monsters.Length}");
            for (int i = 0; i < monsters.Length; i++)
            {
                state.EntityManager.AddBuffer<DenemeData>(monsters[i]);
            }


            if (!isDone)
            {
                for (int i = 0; i < monsters.Length; i++)
                {

                    Debug.Log("yes");
                    DynamicBuffer<DenemeData> buffer = state.EntityManager.GetBuffer<DenemeData>(monsters[i]);
                    buffer.Add(new DenemeData()
                    {
                        Speed = 123f
                    });

                }

                isDone = true;
            }
            else
            {

                for (int i = 0; i < monsters.Length; i++)
                {

                    Debug.Log("mod");
                    DynamicBuffer<DenemeData> buffer = state.EntityManager.GetBuffer<DenemeData>(monsters[i]);

                    for (int j = 0; j < buffer.Length; j++)
                    {

                        buffer[i] = new DenemeData()
                        {
                            Speed = 4444
                        };

                    }


                }
            }


        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
        
        public void OnStartRunning(ref SystemState state)
        {
            Debug.Log("OnStartRunning");
        }
        public void OnStopRunning(ref SystemState state)
        {
            
        }
    }
}