using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Step_1
{
    
    
    public class Spawner : MonoBehaviour
    {
        public static Transform[] TargetTransforms;
        
        public GameObject SeekerPrefab;
        public GameObject TargetPrefab;
        public int NumSeekers;
        public int NumTargets;
        public Vector2 Bounds;

        private void Start()
        {
            Random.InitState(123);
            for (int i = 0; i< NumSeekers; i++)
            {
               GameObject go =  Instantiate(SeekerPrefab);
               Seeker seeker = go.GetComponent<Seeker>();
               Vector2 dir = Random.insideUnitCircle;
               seeker.Direction = dir;
               go.transform.localPosition = new Vector3(Random.Range(0, Bounds.x), Random.Range(0, Bounds.y), 0);
               
            }
            
            TargetTransforms = new Transform[NumTargets];
            for (int i = 0; i < NumTargets; i++)
            {
                GameObject go =  Instantiate(TargetPrefab);
                TargetTransforms[i] = go.transform;
                Target target = go.GetComponent<Target>();
                Vector2 dir = Random.insideUnitCircle;
                target.Direction = dir;
                go.transform.localPosition = new Vector3(Random.Range(0, Bounds.x), Random.Range(0, Bounds.y), 0);
            }
        }
    }
}