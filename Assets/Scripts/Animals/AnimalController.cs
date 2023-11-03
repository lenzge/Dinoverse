using DefaultNamespace;
using Enums;
using UnityEngine;

namespace Animals
{
    public class AnimalController: TimeBasedBehaviour
    {
        public MovementController MovementController;
        public CharacterController CharacterController;
        public FoodManager FoodManager;
        public ReproductionManager ReproductionManager;
        public Eyes Eyes;
        public Plot Plot;
        
        [SerializeField] private float weight;
        [SerializeField] private Gender gender;
        [SerializeField] private Layer species;
        [SerializeField] private Layer enemySpecies;
        [SerializeField] private Layer food;
        [SerializeField] private int age;
        
        public dynamic Brain;
        
        private Transform characterTransform;


        protected override void TimedStart()
        {
            characterTransform = transform;
            age = 0;
        }

        private void Update()
        {
            MovementController.Move(this, characterTransform);
        }

        protected override void TimedUpdate()
        {
            age += 1;
            Vector3 nearestFoodPosition = Eyes.FindFood(characterTransform, food);
            Vector3 characterPosition = characterTransform.position;
            
            Debug.Log($"{name} [{characterPosition.x} , {characterPosition.z}] " +
                      $" [{nearestFoodPosition.x} , {nearestFoodPosition.z}]");
            
            dynamic output = Brain.survive(characterPosition.x, characterPosition.z, 
                nearestFoodPosition.x, nearestFoodPosition.z);
            
            MovementController.SetMoveDirection(new Vector2((float) output[0], (float) output[1]));
            FoodManager.BurnCalories(weight);
            
            bool isEating = FoodManager.TryToEat(characterTransform, CharacterController.radius, food);
            if (isEating)
            {
                ReproductionManager.TryToReproduce(characterTransform, Brain);
            }

            KillIfDead();
        }

        private void KillIfDead()
        {
            if (FoodManager.IsStarving() || ReproductionManager.IsInMenopause())
            {
                int timeOfDeath = Mathf.FloorToInt(Time.time * EnvironmentData.TimeSpeed / 60f);
                Debug.LogWarning($"{timeOfDeath} R.I.P: Died the age of {age} Timesteps, " +
                                 $"Reproduced {ReproductionManager.GetChildCount()} " +
                                 $"times. Complex: {Brain.complexity}");
                Plot.SaveData(age, ReproductionManager.GetChildCount(), timeOfDeath);
                Destroy(gameObject);
            }
            
        }

    }
}