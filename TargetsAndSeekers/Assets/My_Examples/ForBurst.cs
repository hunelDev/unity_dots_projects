using System.Collections.Generic;
using Unity.Mathematics;

namespace My_Examples
{
    public struct ForBurst
    {
        
    }
    
    public struct MyXComparer:IComparer<float3>
    {

        public int Compare(float3 a, float3 b)
        {
            return a.x.CompareTo(b.x);
        }
    }
}