using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Data/Environment Data")]
    public class EnvironmentData : ScriptableObject
    {
        [SerializeField] public int MaxTimeSpeed = 10;
        public int TimeSpeed;

        public void SetTimeSpeed(int timeSpeed)
        {
            TimeSpeed = timeSpeed;
        }
    }
}