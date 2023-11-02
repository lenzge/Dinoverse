using UnityEngine;
using UnityEngine.Events;

namespace DefaultNamespace
{
    public class Nurture : MonoBehaviour
    {
        [Range(0,100)]
        [Tooltip("Mass in kg")]
        public float Mass;

        [Range(50,1000)]
        [Tooltip("Calories per kg")]
        public int Calories;

        public UnityEvent<Nurture,GameObject> NurtureEatenEvent;
        public GameObject Prefab;

        public float Eaten(float eatingSpeed)
        {
            float eatenMass;

            if (Mass >= eatingSpeed)
            {
                eatenMass = eatingSpeed;
                Mass -= eatingSpeed;

            }
            else
            {
                eatenMass = Mass;
                Mass = 0;
            }

            if (Mass == 0)
            {
                NurtureEatenEvent.Invoke(this, Prefab);
                Destroy(gameObject);
            }
            
            return eatenMass * Calories;
        }

    }
}