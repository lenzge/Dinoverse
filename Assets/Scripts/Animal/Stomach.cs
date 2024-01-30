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
        
        public void BurnCalories(Action action, float movementSpeed, bool inAction)
        {
            currentCalories -= animalController.DNA.Weight[0]/15f * EvalPAL(action, movementSpeed, inAction) + animalController.DNA.VisualRadius[0] /50f;
            //Debug.Log($"{name} burned {animalController.DNA.Weight[0]/15f * EvalPAL(action, movementSpeed, inAction)+ animalController.DNA.VisualRadius[0] /50f} " +
                      //$"calories. Action: {action}, active: {inAction}, speed: {movementSpeed} ");
        }

        public void BurnCaloriesOnBirthGiving()
        {
            currentCalories -= 10;
            //Debug.Log($"{name} burned 10 calories on birth giving");
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

        public float HungerLevel()
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
            if (eatenCalories < 0) return currentCalories;
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
            
            float newCalories = currentCalories + eatenCalories * 1.8f;
            //Debug.LogWarning($"{name} ate {eatenCalories} of {food}, carnivore: {animalController.DNA.Carnivore[0]}");
            if (newCalories <= maxCalories)
            {
                return newCalories;
            }
            return maxCalories;
        }

        private float EvalPAL(Action action, float movementSpeed, bool inAction)
        {
            if (!inAction)
            {
                if (action == Action.Fight) return 1 + movementSpeed * 0.04f + 0.3f;
                return 1 + movementSpeed * 0.04f;
            }
            
            switch (action)
            {
                case Action.Eat:
                    return 1 + 0.2f;
                case Action.Reproduce:
                    return 1 + 0.3f;
                case Action.Fight:
                    return 1 + 1.5f;
                default:
                    return 1;
            }
        }
    }
}