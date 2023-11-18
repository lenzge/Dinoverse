using Enums;
using DefaultNamespace;
using UnityEngine;

namespace Animal
{
    public class Stomach : Organ
    {
        public DNA DNA;
        
        private float maxCalories;
        private float currentCalories;
        private Collider[] colliderBuffer = new Collider[1];

        public override void Init(bool isChild = false)
        {
            maxCalories = DNA.Weight[0] * 30;
            currentCalories = maxCalories / 2;
        }
        
        public void BurnCalories(Action action)
        {
            currentCalories -= DNA.Weight[0]/15f * EvalPAL(action);
        }
        
        public bool TryToEat(Transform animalTransform, float characterRadius, Layer foodType)
        {
            if (Physics.OverlapSphereNonAlloc(animalTransform.position, characterRadius, colliderBuffer,
                1 << (int) foodType) >= 1)
            {
                Nurture food = colliderBuffer[0].GetComponent<Nurture>();
                float eatenCalories = food.Eaten(DNA.EatingSpeed[0]);
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

        private float EvalPAL(Action action)
        {
            switch (action)
            {
                case Action.Rest:
                    return 1;
                case Action.Eat:
                    return 1 + DNA.EatingSpeed[0] * 0.25f;
                case Action.Walk:
                    return 1 + DNA.MovementSpeed[0] * 0.1f;
                default:
                    return 1;
            }
        }
    }
}