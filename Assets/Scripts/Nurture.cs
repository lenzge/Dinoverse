using System;
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

        private float currentMass;

        private void Start()
        {
            currentMass = Mass;
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