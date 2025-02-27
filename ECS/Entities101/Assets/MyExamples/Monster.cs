using Unity.Collections;
using Unity.Entities;

namespace MyExamples
{
    public struct Monster : IComponentData
    {
        public FixedString32Bytes name;
    }
}