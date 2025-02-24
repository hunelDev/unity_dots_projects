using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public struct BallData : IComponentData
{
    public float Speed;
    public float3 Direction;
}
