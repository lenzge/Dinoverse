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
        
        public bool TryToEat(Transform animalTransform, float characterRadius, Layer foodType)
        {
            if (Physics.OverlapSphereNonAlloc(animalTransform.position, characterRadius, colliderBuffer,
                1 << (int) foodType) >= 1)
            {
                Nurture food = colliderBuffer[0].GetComponent<Nurture>();
                float eatenCalories = food.Eaten(animalController.DNA.EatingSpeed[0]);
                currentCalories = AddCalories(eatenCalories);
                //Debug.LogWarning($"{currentCalories} after eating {eatenCalories} calories");
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

        private float EvalPAL(Action action, float movementSpeed)
        {
            if (movementSpeed < 0)
            {
                movementSpeed = animalController.DNA.MovementSpeed[1];
            }
            else
            {
                movementSpeed = animalController.DNA.MovementSpeed[0];
            }
            
            switch (action)
            {
                case Action.Rest:
                    return 2;
                case Action.Eat:
                    return 1 + movementSpeed * 0.1f;
                case Action.Reproduce:
                    return 1 + 0.75f;
                default:
                    return 1;
            }
        }
    }
}