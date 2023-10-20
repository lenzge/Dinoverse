﻿using System;
using DefaultNamespace;
using UnityEngine;

namespace Animals
{
    public class FoodManager : MonoBehaviour
    {
        [SerializeField] 
        private float maxCalories;
        [SerializeField]
        [Range(0.1f,1f)]
        [Tooltip("kg per deci-hour")]
        private float eatingSpeed;

        private float currentCalories;


        private void Start()
        {
            currentCalories = maxCalories / 2;
        }

        // Call once per deci-hour
        public void BurnCalories(float weight)
        {
            float PAL = 1; //PAL of current action
            currentCalories -= weight/10 * PAL;
            Debug.Log($"{currentCalories} after burn");
        }

        public void Eat(Nurture food)
        {
            if (isFull())
            {
                return;
            }
            
            float eatenCalories = food.Eaten(eatingSpeed);
            currentCalories = AddCalories(eatenCalories);
            Debug.Log($"{currentCalories} after eating {eatenCalories} calories");
        }

        public bool IsStarving()
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

        // Never the case because calories are always burned
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