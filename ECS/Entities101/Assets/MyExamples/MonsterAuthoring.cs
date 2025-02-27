using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace MyExamples
{
    public class MonsterAuthoring : MonoBehaviour
    {

        private class MonsterBaker : Baker<MonsterAuthoring>
        {
            public string name;
            public override void Bake(MonsterAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Monster
                {
                    name = authoring.name,
                });
            }
        }
    }
}