using System;
using UnityEngine;

namespace Step_2
{
    public class Target : MonoBehaviour
    {   
        public Vector3 Direction;
        private void Update()
        {
            transform.localPosition += Direction * Time.deltaTime;
        }
    }
}