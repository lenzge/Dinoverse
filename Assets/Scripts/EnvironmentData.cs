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
        
        [Header("Animals")]
        public bool SexualReproduction;
        public bool AllowPredation;
        public bool RandomSpawnPoint;
        public int InitialAnimalAmount;
        public int MaxAnimalAmount;
        public int ReproductionEnergy;

        [Header("Environment")] 
        public int InitialTreeAmount;
        public int MaxTrees;
        public int MinTrees;
        public int LakeCount;
        public int MapSize;
        public bool ConstantTreeAmount;
        public bool EndlessWorld;
        
        [HideInInspector]
        public UnityEvent<int> TimeSpeedChangedEvent;

        public void SetTimeSpeed(int timeSpeed)
        {
            TimeSpeed = timeSpeed;
            TimeSpeedChangedEvent.Invoke(TimeSpeed);
        }
    }
}