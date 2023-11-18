﻿using System;
using UnityEngine;
using UnityEngine.Events;
using Util;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class Nurture : TimeBasedBehaviour
    {
        public GameObject Prefab;
        public float Mass;
        public int Calories;
        public int SeedAmount;
        public int SeedInterval;
        public int NoMoreSeeds;

        [HideInInspector]
        public UnityEvent<Nurture,GameObject> NurtureEatenEvent;

        private float currentMass;
        private int age;
        private int seedCount;
        private EnvironmentCreator environmentCreator;

        public int RandoOffset;

        protected override void TimedStart()
        {
            currentMass = Mass;
            seedCount = 0;
            age = Random.Range(-20,0);
            environmentCreator = GameObject.Find("Environment").GetComponent<EnvironmentCreator>();
        }

        protected override void TimedUpdate()
        {
            age += 1;

            if (seedCount < NoMoreSeeds && age == SeedInterval)
            {
                age = Random.Range(-20,0);
                seedCount += 1;
                for (int i = 0; i < SeedAmount; i++)
                {
                    try
                    {
                        // TODO irgendwie stürzt da ständig das game ab
                        //environmentCreator.SpawnNurtureSeed(Prefab, gameObject.transform.position, i);
                        //Debug.LogWarning("Spawn new tree");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    
                }
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
                NurtureEatenEvent.Invoke(this, Prefab);
                Destroy(gameObject);
            }
            
            return eatenMass * Calories;
        }

    }
}