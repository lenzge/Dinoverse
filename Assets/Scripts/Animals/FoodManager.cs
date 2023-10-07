using System;
using DefaultNamespace;
using UnityEngine;

namespace Animals
{
    public class FoodManager : MonoBehaviour
    {
        [SerializeField] 
        private float maxCalories;
        [SerializeField]
        [Range(1,10)]
        [Tooltip("kg per hour")]
        private int eatingSpeed;

        private float currentCalories;


        private void Start()
        {
            currentCalories = maxCalories / 2;
        }

        // Call once per ingame hour
        public void BurnCalories(int weight)
        {
            float PAL = 1; //PAL of current action
            currentCalories -= weight * PAL;
            Debug.Log($"{currentCalories} after burn");
        }

        public void Eat(Nurture food)
        {
            if (isFull())
            {
                return;
            }
            
            int eatenCalories = food.Eaten(eatingSpeed);
            currentCalories = AddCalories(eatenCalories);
            Debug.Log($"{currentCalories} after eating {eatenCalories} calories");
        }

        public bool isStarving()
        {
            if (currentCalories <= 0)
            {
                Debug.Log($"<color=red>Starved</color>");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Add calories in limit of the maxCalories
        /// </summary>
        /// <param name="eatenCalories"></param>
        /// <returns></returns>
        private float AddCalories(float eatenCalories)
        {
            float newCalories = currentCalories + eatenCalories;
            if (newCalories <= maxCalories)
            {
                return newCalories;
            }
            return maxCalories;
        }

        private bool isFull()
        {
            if (currentCalories >= maxCalories)
            {
                Debug.Log($"<color=yellow>Ich bin so satt ich mag kein Blatt. Mäh!</color>");
                return true;
            }
            return false;
        }
    }
}