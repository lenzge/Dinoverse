using DefaultNamespace;
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

        private Transform characterTransform;

        protected override void TimedStart()
        {
            characterTransform = transform;
        }

        private void Update()
        {
            MovementController.Move(this, characterTransform);
        }

        protected override void TimedUpdate()
        {
            //Eyes.LookAround(characterTransform.position, food, species, enemySpecies);
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