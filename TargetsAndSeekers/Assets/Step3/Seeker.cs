using UnityEngine;

namespace Step_3
{
    public class Seeker : MonoBehaviour
    {

        public Vector3 Direction;
        void Start()
        {
        
        }
    
        void Update()
        {
            transform.localPosition += Direction * Time.deltaTime;
        }
    }

}