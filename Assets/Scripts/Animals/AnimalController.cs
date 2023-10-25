using System.Collections.Generic;
using System.Xml;
using DefaultNamespace;
using Python.Runtime;
using UnityEditor.UI;
using UnityEngine;

namespace Animals
{
    public class AnimalController: TimeBasedBehaviour
    {
        public MovementController MovementController;
        public FoodManager FoodManager;
        public Eyes Eyes;

        [SerializeField] private float weight;
        [SerializeField] private GENDER gender;
        [SerializeField] private LAYER species;
        [SerializeField] private LAYER enemySpecies;
        [SerializeField] private LAYER food;

        public dynamic Brain;
        
        private Transform characterTransform;

        protected override void TimedStart()
        {
            characterTransform = transform;
            Eyes.LookAround(characterTransform.position, food, species, enemySpecies);
        }

        private void Update()
        {
            MovementController.Move(this, characterTransform);
        }

        protected override void TimedUpdate()
        {
            Eyes.LookAround(characterTransform.position, food, species, enemySpecies);
            dynamic output = Brain.survive(characterTransform.position.x, characterTransform.position.z,
                Eyes.FoodPositions[0].x, Eyes.FoodPositions[0].z); 
            MovementController.SetMoveDirection(new Vector2((float) output[0], (float) output[1]));
            //FoodManager.BurnCalories(weight);
            //FoodManager.Eat(PLFood);
            KillIfDead();
        }

        private void KillIfDead()
        {
            if (FoodManager.IsStarving())
            {
                Destroy(gameObject);
            }
        }
    }
}