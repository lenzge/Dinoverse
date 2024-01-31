using System;
using Enums;
using UnityEngine;
using UnityEngine.Events;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Data/Environment Data")]
    public class EnvironmentData : ScriptableObject
    {
        [Header("Time")]
        public int MaxTimeSpeed = 40;
        public int TimeSpeed;
        
        [HideInInspector]
        public UnityEvent<int> TimeSpeedChangedEvent;
        [HideInInspector]
        public UnityEvent KillAnimalsEvent;
        [HideInInspector]
        public UnityEvent KillTreesEvent;
        [HideInInspector]
        public UnityEvent SeparationEvent;
        [HideInInspector]
        public UnityEvent AllowPredationEvent;

        public bool NaturalDisaster;
        
        [Header("Animals")]
        public bool SexualReproduction;
        public bool AllowPredation;
        public bool RandomSpawnPoint;
        public int InitialAnimalAmount;
        public int MaxAnimalAmount;
        public int ReproductionEnergy;
        public bool Classify;

        [Header("Environment")] 
        public int InitialTreeAmount;
        public int MaxTrees;
        public int MinTrees;
        public int LakeCount;
        public int MaxLakeCount;
        public int MapSize;
        public bool ConstantTreeAmount;
        public bool EndlessWorld;
        public Change RateOfChange;

        public void SetTimeSpeed(int timeSpeed)
        {
            TimeSpeed = timeSpeed;
            TimeSpeedChangedEvent.Invoke(TimeSpeed);
        }

        public void ChangePredation()
        {
            AllowPredation = !AllowPredation;
            AllowPredationEvent.Invoke();
        }
    }
}