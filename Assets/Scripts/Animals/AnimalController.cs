using DefaultNamespace;
using UnityEngine;

namespace Animals
{
    public class AnimalController: TimeBasedBehaviour
    {
        public MovementController MovementController;
        public CharacterController CharacterController;
        public FoodManager FoodManager;
        public Eyes Eyes;
        public Plot Plot;

        [SerializeField] private GameObject animalPrefab;
        [SerializeField] private float weight;
        [SerializeField] private GENDER gender;
        [SerializeField] private LAYER species;
        [SerializeField] private LAYER enemySpecies;
        [SerializeField] private LAYER food;

        public dynamic Brain;
        
        private Transform characterTransform;

        private int age = 0;
        public int ChildCount = 0;
        private int repEnergy = 0;

        protected override void TimedStart()
        {
            characterTransform = transform;
            age = 0;
            ChildCount = 0;
            repEnergy = 0;
            
            //Debug.LogWarning(Brain.return_genome());
            //Eyes.LookAround(characterTransform.position, food, species, enemySpecies);
            //Eyes.RaycastLookAround(characterTransform, food, species, enemySpecies);
        }

        private void Update()
        {
            MovementController.Move(this, characterTransform);
        }

        protected override void TimedUpdate()
        {
            age += 1;
            Eyes.RaycastLookAround(characterTransform, food, species, enemySpecies);
            //Eyes.LookAround(characterTransform.position, food, species, enemySpecies);
            //Debug.Log($"{name} [{characterTransform.position.x} , {characterTransform.position.z}] " +
                      //$" [{Eyes.NearestFoodPosition.x} , {Eyes.NearestFoodPosition.z}]");
            dynamic output = Brain.survive(characterTransform.position.x, characterTransform.position.z, 
                Eyes.NearestFoodPosition.x, Eyes.NearestFoodPosition.z);
            //dynamic output = Brain.survive2(Eyes.FoodDistance, Eyes.FoodAngle, Eyes.isWall);
            MovementController.SetMoveDirection(new Vector2((float) output[0], (float) output[1]));
            FoodManager.BurnCalories(weight);
            bool isEating = FoodManager.TryToEat(characterTransform, CharacterController.radius,LAYER.Tree);
            //Brain.update_fitness(EvaluateFitness(isEating));
            if (isEating)
            {
                repEnergy += 1;

                if (repEnergy == 2)
                {
                    ChildCount += 1;
                    AnimalController partner = Eyes.LookForPartner(characterTransform, species);
                    for (int i = 0; i < ChildCount; i++)
                    {
                        BornAnimal(Brain.reproduce(partner.Brain), characterTransform.position);
                    }
                    repEnergy = 0;
                }
                
            }

            KillIfDead();
        }

        private void KillIfDead()
        {
            if (FoodManager.IsStarving() || ChildCount == 5)
            {
                Debug.LogWarning($"{Mathf.FloorToInt(Time.time * EnvironmentData.TimeSpeed / 60f)} " +
                                 $"R.I.P: Died the age of {age} Seconds, Reproduced {ChildCount} times. Complex: {Brain.complexity}");
                Plot.SaveData(age, ChildCount, Mathf.FloorToInt(Time.time * EnvironmentData.TimeSpeed / 60f));
                Destroy(gameObject);
            }
            
        }

        private float EvaluateFitness(bool isEating)
        {
            float fitness = 0;
            if (FoodManager.currentCalories <= 300)
            {
                fitness -= 50;
            }

            if (isEating)
            {
                fitness += 100;
            }

            return fitness;
        }
        
        public void BornAnimal(dynamic animalBrain, Vector3 position)
        {
            Vector3 spawnPosition = new Vector3(position.x + Random.Range(2,10), 0,
                position.z +Random.Range(2,10));
            GameObject newAnimal = Instantiate(animalPrefab, spawnPosition, Quaternion.identity);
            newAnimal.GetComponent<AnimalController>().Brain = animalBrain;
        }
        
    }
}