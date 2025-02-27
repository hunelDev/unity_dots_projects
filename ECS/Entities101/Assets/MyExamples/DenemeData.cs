using Unity.Entities;

namespace MyExamples
{
    [InternalBufferCapacity(0)]//mesela burda tanimla ile chunkda depolanmicak Dynamic buffer in her elementi eklendiginde artik.
    public struct DenemeData : IBufferElementData
    {
        public float Speed;
    }
}