using System;
using DefaultNamespace;
using UnityEngine;

namespace Animals
{
    public class AnimalController: MonoBehaviour

    {
        public MovementController MovementController;
        public FoodManager FoodManager;
        public Nurture PLFood;

        [SerializeField] private int weight;

        private Transform characterTransform;

        private void Start()
        {
            characterTransform = transform;
        }

        private void Update()
        {
            MovementController.Move(characterTransform);
            FoodManager.BurnCalories(weight);
            FoodManager.Eat(PLFood);
            if (FoodManager.isStarving())
            {
                Destroy(gameObject);
            }
        }
    }
}