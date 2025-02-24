using System;
using Unity.Collections;
using UnityEngine;

namespace My_Examples
{
    public class Deneme : MonoBehaviour
    {
        private NativeArray<int> Bro;
        private void Start()
        {
            Bro = new NativeArray<int>(new int[]{1,44,6,92,11,27,33},Allocator.Persistent);
            Debug.Log(~Bro.BinarySearch(92));
        }


        private void OnDestroy()
        {
            Bro.Dispose();
        }
    }
    
  
}