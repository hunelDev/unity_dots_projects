using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

namespace DefaultNamespace
{
    public class BallAuthoring : MonoBehaviour
    {
        public float Speed;
        public float3 Direction;

        private class BallBaker : Baker<BallAuthoring>
        {
            public override void Bake(BallAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                
                if(GetTag() == "Player")
                    AddComponent(entity, new BallData { Speed = authoring.Speed, Direction = authoring.Direction });

            }
        }
    }
}