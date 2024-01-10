using Enums;
using DefaultNamespace;
using UnityEngine;

namespace Animal
{
    public class Stomach : Organ
    {
        
        [SerializeField] private float maxCalories;
        [SerializeField] private float currentCalories;
        private Collider[] colliderBuffer = new Collider[1];

        public override void Init(bool isChild = false)
        {
            maxCalories = animalController.DNA.Weight[0] * 30;
            currentCalories = maxCalories / 2;
        }
        
        public void BurnCalories(Action action, float movementSpeed)
        {
            currentCalories -= animalController.DNA.Weight[0]/15f * EvalPAL(action, movementSpeed);
        }
        
        public bool TryToEatPlants(Transform animalTransform, float characterRadius, Layer foodType)
        {
            if (Physics.OverlapSphereNonAlloc(animalTransform.position, characterRadius, colliderBuffer,
                1 << (int) foodType) >= 1)
            {
                Nurture food = colliderBuffer[0].GetComponent<Nurture>();
                float eatenCalories = food.Eaten(animalController.DNA.EatingSpeed[0]);
                currentCalories = AddCalories(eatenCalories, FoodSource.plant);
                //Debug.LogWarning($"{currentCalories} after eating {eatenCalories} calories");
                if (currentCalories == 0) return false;
                return true;
            }

            return false;
        }

        public bool IsStarving()
        {
            if (currentCalories <= 0)
            {
                //Debug.Log($"<color=red>Starved</color>");
                return true;
            }

            return false;
        }

        public float HungerInput()
        {
            return currentCalories / maxCalories;
        }

        public float GetCurrentCalories()
        {
            return currentCalories;
        }
        
        /// <summary>
        /// Add calories in limit of the maxCalories
        /// </summary>
        /// <param name="eatenCalories"></param>
        /// <returns></returns>
        public float AddCalories(float eatenCalories, FoodSource food)
        {
            if (EnvironmentData.AllowPredation)
            {
                if (food == FoodSource.meat)
                {
                    eatenCalories *= animalController.DNA.Carnivore[0];
                }
                else if (food == FoodSource.plant)
                {
                    eatenCalories *= 1 - animalController.DNA.Carnivore[0];
                }
            }
            
            float newCalories = currentCalories + eatenCalories;
            if (newCalories <= maxCalories)
            {
                return newCalories;
            }
            return maxCalories;
        }

        private float EvalPAL(Action action, float movementSpeed)
        {
            switch (action)
            {
                case Action.Chill:
                    return 1 + movementSpeed * 0.02f;
                case Action.Eat:
                    return 1 + 0.2f + movementSpeed * 0.02f;
                case Action.Reproduce:
                    return 1 + 0.3f + movementSpeed * 0.02f;
                case Action.Fight:
                    return 1 + 0.8f + movementSpeed * 0.02f;
                default:
                    return 1;
            }
        }
    }
}