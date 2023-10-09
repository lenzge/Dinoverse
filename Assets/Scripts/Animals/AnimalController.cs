using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using System.Linq;

namespace Animals
{
    public class AnimalController: TimeBasedBehaviour
    {
        public MovementController MovementController;
        public FoodManager FoodManager;
        public Eyes Eyes;

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
            Eyes.LookAround(characterTransform);
            FoodManager.BurnCalories(weight);
            //FoodManager.Eat(PLFood);
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