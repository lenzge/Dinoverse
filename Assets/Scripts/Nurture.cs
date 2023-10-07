using System.Collections;
using UnityEditor;
using UnityEngine;

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

            if (Mass <= 0)
            {
                Destroy(gameObject);
            }
            
            return eatenMass * Calories;
        }

    }
}