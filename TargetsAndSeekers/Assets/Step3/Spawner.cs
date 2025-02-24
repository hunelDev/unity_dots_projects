using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Step_3
{
    public class Spawner : MonoBehaviour
    {

        public static Transform[] SeekerTransforms;
        public static Transform[] TargetTransforms;

        public GameObject SeekerPrefab;
        public GameObject TargetPrefab;

        public int NumSeekers;
        public int NumTargets;

        public Vector2 Bounds;


        private void Start()
        {
            Random.InitState(123);
            SeekerTransforms = new Transform[NumSeekers];
            TargetTransforms = new Transform[NumTargets];

            for (int i = 0; i < NumSeekers; i++)
            {
                GameObject go = Instantiate(SeekerPrefab);
                SeekerTransforms[i] = go.transform;
                Seeker seeker = go.GetComponent<Seeker>();
                seeker.Direction = Random.insideUnitCircle;
                go.transform.localPosition = new Vector3(Random.Range(0, Bounds.x), Random.Range(0, Bounds.y), 0);
            }


            for (int i = 0; i < NumTargets; i++)
            {
                GameObject go = Instantiate(TargetPrefab);
                TargetTransforms[i] = go.transform;
                Target target = go.GetComponent<Target>();
                target.Direction = Random.insideUnitCircle;
                go.transform.localPosition = new Vector3(Random.Range(0, Bounds.x), Random.Range(0, Bounds.y), 0);
            }
            

        }
    }
}