using System;
using System.Collections;
using System.Drawing;
using UnityEngine;
using UnityEngine.Events;
using Util;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class Nurture : TimeBasedBehaviour
    {
        public EnvironmentCreator EnvironmentCreator;
        
        [Header("Prefabs")]
        public GameObject Prefab;
        public GameObject FullGrownModel;
        public GameObject SmallModel;
        public SphereCollider Collider;
        
        [Header("Params")]
        public float Mass;
        public int Calories;
        public int CurrentCalories;
        public int FullGrownAge;
        public int MaxAge;
        public int SeedAmount;
        public bool FirstGeneration;

        [HideInInspector]
        public UnityEvent<Nurture,GameObject> NurtureEatenEvent;

        private float currentMass;
        private int age;

        protected override void TimedStart()
        {
            currentMass = Mass;
            age = Random.Range(-20,0);
            Collider.enabled = true;
            
            if (FirstGeneration)
            {
                CurrentCalories = Calories;
                SmallModel.SetActive(false);
                FullGrownModel.SetActive(true);
            }
            else
            {
                CurrentCalories = 0;
                SmallModel.SetActive(true);
                FullGrownModel.SetActive(false);
            }
            
        }

        protected override void TimedUpdate()
        {
            age += 1;

            if (age == FullGrownAge)
            {
                CurrentCalories = Calories;
                SmallModel.SetActive(false);
                FullGrownModel.SetActive(true);
                SpawnSeeds();
            }
            
            else if (age >= MaxAge)
            {
                NurtureEatenEvent.Invoke(this, Prefab);
                Destroy(gameObject);
            }
        }

        public float Eaten(float eatingSpeed)
        {
            float eatenMass;

            if (currentMass >= eatingSpeed)
            {
                eatenMass = eatingSpeed;
                currentMass -= eatingSpeed;

            }
            else
            {
                eatenMass = currentMass;
                currentMass = 0;
            }

            if (currentMass == 0)
            {
                //StartCoroutine(Recover());
                StartCoroutine(DestroyAfterDelay());
            }
            
            return eatenMass * CurrentCalories;
        }

        private void SpawnSeeds()
        {
            if (!EnvironmentData.ConstantTreeAmount &&
                EnvironmentCreator.CurrentTreeCount < EnvironmentData.MaxTrees)
            {
                for (int i = 0; i < SeedAmount; i++)
                {
                    EnvironmentCreator.SpawnNurtureSeed(Prefab, gameObject.transform.position, Random.Range(0,100));
                }
            }
        }
        
        IEnumerator Recover()
        {
            gameObject.GetComponent<SphereCollider>().enabled = false;
            gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
            float timeInterval = 10f / EnvironmentData.TimeSpeed;
            yield return new WaitForSeconds(timeInterval);
            gameObject.GetComponent<SphereCollider>().enabled = true;
            gameObject.GetComponentInChildren<MeshRenderer>().enabled = true;
            currentMass = Mass;
        }

        IEnumerator DestroyAfterDelay()
        {
            Collider.enabled = false;
            float timeInterval = 10f / EnvironmentData.TimeSpeed;
            yield return new WaitForSeconds(timeInterval);
            NurtureEatenEvent.Invoke(this, Prefab);
            Destroy(gameObject);
        }

    }
}