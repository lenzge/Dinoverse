using System;
using DefaultNamespace;
using Enums;
using UnityEngine;
using UnityEngine.Events;

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
        public int Generation;
        
        private Transform characterTransform;

        public UnityEvent Dead;

        // Parameters for Fitness
        private int treeCount;
        private int searchTime;
        private bool isHittingWall;


        protected override void TimedStart()
        {
            characterTransform = transform;
            
            age = 0;
            treeCount = 0;
            searchTime = 0;
        }

        private void Update()
        {
            MovementController.Move(this, characterTransform);
        }

        protected override void TimedUpdate()
        {
            age += 1;
            searchTime += 1;
            Vector3 nearestFoodPosition = Eyes.FindFood(characterTransform, food);
            Vector3 characterPosition = characterTransform.position;

            dynamic output = Brain.survive(characterPosition.x, characterPosition.z, 
                nearestFoodPosition.x, nearestFoodPosition.z);

            try
            {
                Debug.Log($"{name} [{characterPosition.x} , {characterPosition.z}] " +
                          $" [{nearestFoodPosition.x} , {nearestFoodPosition.z}]" +
                          $" [{(float) output[0]} , {(float) output[1]}]");
            
                if ((float) output[0] == 0 && (float) output[1] == 0)
                {
                    Brain.update_fitness(-3);
                }

                if (Physics.Raycast(transform.position, transform.forward, CharacterController.radius + 0.5f,
                    1 << (int) Layer.Water))
                {
                    Brain.update_fitness(-1);
                }

                MovementController.SetMoveDirection(new Vector2((float) output[0], (float) output[1]));
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            
            FoodManager.BurnCalories(weight);
            
            bool isEating = FoodManager.TryToEat(characterTransform, CharacterController.radius, food);
            if (isEating)
            {
                treeCount += 1;
                UpdateFitness();
                searchTime = 0;
                //ReproductionManager.TryToReproduce(characterTransform, Brain);
            }

            KillIfDead();
        }

        private void UpdateFitness()
        {
            int fitness = 0;

            fitness += treeCount * 100;
            fitness += 100 / searchTime;

            Debug.LogWarning($"{treeCount} Tree found after {searchTime} timeSteps. This will increase the fitness by {fitness}");
            Brain.update_fitness(fitness);
        }

        private void KillIfDead()
        {
            if (FoodManager.IsStarving() || ReproductionManager.IsInMenopause())
            {
                int timeOfDeath = Mathf.FloorToInt(Time.time * EnvironmentData.TimeSpeed / 60f);
                Debug.LogWarning($"Generation {Generation} R.I.P: Died the age of {age} Timesteps, " +
                                 //$"Reproduced {ReproductionManager.GetChildCount()} " +
                                 $"Found {treeCount} Trees " +
                                 $"Complex: {Brain.complexity}");
                Debug.LogWarning(Brain.return_genome());
                //Plot.SaveData(age, ReproductionManager.GetChildCount(), timeOfDeath);
                Plot.SaveData((int) Brain.return_fitness(), treeCount, Generation);
                Dead.Invoke();
                Destroy(gameObject);
            }
            
        }

    }
}