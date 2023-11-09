using System;
using AI;
using DefaultNamespace;
using Enums;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Animals
{
    public class AnimalController: TimeBasedBehaviour
    {
        public MovementController MovementController;
        public CharacterController CharacterController;
        public FoodManager FoodManager;
        public ReproductionManager ReproductionManager;
        public NeatController NeatController;
        public Brain Brain;
        public Eyes Eyes;
        public Plot Plot;

        // Mutatable Params
        public float MutationAmount;
        public float MutationChance;
        
        [SerializeField] private float weight;
        [SerializeField] private Gender gender;
        [SerializeField] private Layer species;
        [SerializeField] private Layer enemySpecies;
        [SerializeField] private Layer food;
        [SerializeField] private int age;

        public int Key;
        public int Generation;
        public int GrandChild;
        public int Fitness;
        
        private Transform characterTransform;

        public UnityEvent Dead;
        public UnityEvent<AnimalController> AlsoDead;

        // Parameters for Fitness
        private int treeCount;
        private int searchTime;
        private bool isHittingWall;


        protected override void TimedStart()
        {
            characterTransform = transform;
            //NeatController = GameObject.Find("NeatController").GetComponent<NeatController>();
            Plot = GameObject.Find("AnimalCreator").GetComponent<Plot>();
            MutateCreature();

            age = 0;
            treeCount = 0;
            searchTime = 0;
            Fitness = 0;
        }

        public void UpdateName()
        {
            gameObject.name = $"{Key}.{Generation}.{GrandChild}";
        }

        private void Update()
        {
            MovementController.Move(this, characterTransform);
        }

        protected override void TimedUpdate()
        {
            age += 1;
            searchTime += 1;
            //Vector3 nearestFoodPosition = Eyes.FindFood(characterTransform, food);
            Vector3 characterPosition = characterTransform.position;

            //float[] output = Brain.Survive(new float[] {characterPosition.x, characterPosition.z, 
                //nearestFoodPosition.x, nearestFoodPosition.z});

            float[] output = Brain.Survive(Eyes.LookAround(characterTransform, food));

            try
            {
                //Debug.Log($"{name} [{characterPosition.x} , {characterPosition.z}] " +
                          //$" [{nearestFoodPosition.x} , {nearestFoodPosition.z}]" +
                          //$" [{(float) output[0]} , {(float) output[1]}]");

                          if ((float) output[0] == 0 && (float) output[1] == 0)
                {
                    Fitness -= 1;
                    //Brain.update_fitness(-3);
                }

                if (Physics.Raycast(transform.position, transform.forward, CharacterController.radius + 0.5f,
                    1 << (int) Layer.Water))
                {
                    Fitness -= 3;
                    FoodManager.GetDamage(30);
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
                ReproductionManager.TryToReproduce(this);
            }

            KillIfDead();
        }

        private void UpdateFitness()
        {
            int fitness = 0;

            fitness += treeCount * 100;
            fitness += 100 / searchTime;

            Debug.LogWarning($"[{gameObject.name}] {treeCount} Tree found after {searchTime} timeSteps. This will increase the fitness by {fitness}");
            //Brain.update_fitness(fitness);
            Fitness += fitness;
        }

        private void KillIfDead()
        {
            if (FoodManager.IsStarving() || ReproductionManager.IsInMenopause())
            {
                int timeOfDeath = Mathf.FloorToInt(Time.time * EnvironmentData.TimeSpeed / 60f);
                Debug.LogWarning($"[{gameObject.name}] {timeOfDeath} R.I.P: Died the age of {age} Timesteps, " +
                                 $"Reproduced {ReproductionManager.GetChildCount()} times, " +
                                 $"Found {treeCount} Trees ");
                // Debug.LogWarning(Brain.return_genome());
                Plot.SaveData(Key, Generation, GrandChild,age, treeCount, ReproductionManager.GetChildCount(), timeOfDeath);
                //Plot.SaveData((int) Brain.return_fitness(), treeCount, Generation);
                Dead.Invoke();
                AlsoDead.Invoke(this);
                Destroy(gameObject);
            }
            
        }
        public void MutateCreature()
        {
            if (Random.value < MutationChance)
            {
                int angleBetweenRaycasts = Eyes.AngleBetweenRaycasts + Random.Range(-1, 1)*(int)(MutationAmount*2);
                int radius = Eyes.Radius + Random.Range(-1, 1)*(int)(MutationAmount*2);
                Eyes.SetEyeParams(Eyes.NumRaycasts, angleBetweenRaycasts, radius);
                
                MutationAmount += Random.Range(-1.0f, 1.0f)/100;
                MutationChance += Random.Range(-1.0f, 1.0f)/100;

                //make sure mutation amount and chance are positive using max function
                MutationAmount = Mathf.Max(MutationAmount, 0);
                MutationChance = Mathf.Max(MutationChance, 0);
                Brain.MutateNetwork(MutationAmount, MutationChance);
                
            }
            Eyes.SetEyeParams(Eyes.NumRaycasts, Eyes.AngleBetweenRaycasts, Eyes.Radius);
            Debug.Log($"[{gameObject.name}] Mutationamount: {Math.Round(MutationAmount, 3)}, chance: {Math.Round(MutationChance, 3)}");
        }


    }
}