using System;
using DefaultNamespace;
using UnityEngine;

namespace Animals
{
    public class AnimalController: TimeBasedBehaviour
    {
        public MovementController MovementController;
        public FoodManager FoodManager;
        public Nurture PLFood;

        [SerializeField] private float weight;

        private Transform characterTransform;

        protected override void TimedStart()
        {
            characterTransform = transform;
        }

        private void Update()
        {
            MovementController.Move(characterTransform);
        }

        protected override void TimedUpdate()
        {
            FoodManager.BurnCalories(weight);
            FoodManager.Eat(PLFood);
            KillIfDead();
        }

        private void KillIfDead()
        {
            if (FoodManager.isStarving())
            {
                Destroy(gameObject);
            }
        }
    }
}