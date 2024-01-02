using System;
using System.Collections.Generic;
using System.Linq;
using Animal;
using Enums;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Data/AnimalCreator")]
    public class AnimalCreator : ScriptableObject
    {
        [Header("References")]
        [SerializeField] private EnvironmentData environmentData;
        [SerializeField] private GameObject animalPrefab;
        [SerializeField] private GenomeParser genomeParser;
        
        [SerializeField] private int stopRespawnTime;
        [SerializeField] private int brainBuffer;
        
        private int currentPopulation;
        private int nextKey;
        private float newGenomesAmount;
        private List<AnimalController> activeAnimalControllers = new List<AnimalController>();
        private List<GameObject> savedAnimalControllers = new List<GameObject>();
        private List<Genome> frozenGenomes = new List<Genome>();

        // Level system
        public int FitnessToScore;
        private int bestFitness;
        private int animalsScoredFitness;

        private Collider[] colliderBuffer = new Collider[1];

        public void StartGame()
        {
            activeAnimalControllers.Clear();
            savedAnimalControllers.Clear();
            
            currentPopulation = 0;
            nextKey = 0;
            bestFitness = 0;
            FitnessToScore = 5;
            animalsScoredFitness = 0;

            newGenomesAmount = 0.5f;
            frozenGenomes = genomeParser.LoadAllGenomes();
            CreateNewGeneration();
        }

        private void CreateNewGeneration()
        {
            currentPopulation += 1;
            Debug.LogWarning($"Create new Generation ({currentPopulation})");
            for (int i = 0; i < environmentData.InitialAnimalAmount; i++)
            {
                CreateAnimalObject(nextKey, 0, true, GenomeType.Random, SpawnType.Random);
                //CreateAnimalObject(nextKey, 0, true, GenomeType.Frozen, SpawnType.Random);
            }
        }

        public void CreateChildObject(bool prio, int key, int generation, GenomeType genomeType,
            SpawnType spawnPositionType, AnimalController parent, AnimalController parent2 = null)
        {
            if (prio)
            {
                savedAnimalControllers.Insert(0,CreateAnimalObject(key,
                    generation, false, genomeType,
                    spawnPositionType, parent, parent2));
            }
            else
            {
                savedAnimalControllers.Add(CreateAnimalObject(key,
                    generation, false, genomeType,
                    spawnPositionType, parent, parent2));
            }
        }
        
        
        private GameObject CreateAnimalObject(int key, int generation, bool spawnInstant, GenomeType genomeType, 
            SpawnType spawnPositionType, AnimalController parent = null, AnimalController parent2 = null)
        {
            GameObject animal = Instantiate(animalPrefab);
            animal.SetActive(false);
            AnimalController controller = animal.GetComponent<AnimalController>();
            controller.UpdateInfo(key, currentPopulation, generation);

            switch (genomeType)
            {
                case GenomeType.Random:
                    controller.DNA.CreateNewDNA();
                    controller.InitOrgans(false);
                    controller.Brain.MutateNetwork();
                    nextKey += 1;
                    break;
                case GenomeType.Parent:
                    if (parent == null) throw new Exception("Try to create a child animal without parent");
                    controller.DNA.CopyValuesFrom(parent.DNA);
                    controller.DNA.Mutate();
                    controller.Brain.Layers = parent.Brain.CopyLayers();
                    controller.InitOrgans(true);
                    controller.Brain.MutateNetwork();
                    break;
                case GenomeType.Crossover:
                    if (parent == null || parent2 == null) throw new Exception("Try to create a child animal without parent");
                    controller.DNA.CopyValuesFrom(parent.DNA);
                    controller.DNA.CrossoverDNA(parent2.DNA);
                    controller.DNA.Mutate();
                    controller.Brain.Layers = parent.Brain.CopyLayers();
                    controller.Brain.CrossoverNetwork(parent2.Brain);
                    controller.InitOrgans(true);
                    controller.Brain.MutateNetwork();
                    break;
                case GenomeType.Frozen:
                    // TODO Maybe find a more balanced way later
                    Genome genome = frozenGenomes.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                    genome.LoadGenome(controller.Brain, controller.DNA);
                    controller.InitOrgans(true);
                    nextKey += 1;
                    break;
            }

            if (spawnInstant)
            {
                SpawnAnimal(animal,spawnPositionType, parent);
            }
            return animal;
        }

        private void SpawnAnimal(GameObject animal, SpawnType spawnPositionType, AnimalController parent = null)
        {
            AnimalController controller = animal.GetComponent<AnimalController>();
            activeAnimalControllers.Add(controller);
            controller.Died.AddListener(OnDead);
            var randomRotation = Quaternion.Euler( 0,Random.Range(0, 360) , 0);
            while (true)
            {
                animal.transform.position = EvaluateSpawnPosition(spawnPositionType, parent);
                animal.transform.rotation = randomRotation;
                if (Physics.OverlapSphereNonAlloc(animal.transform.position, 2, colliderBuffer,
                    1 << (int) Layer.Water) < 1)
                {
                    animal.SetActive(true);
                    break;
                }
            }
        }

        private Vector3 EvaluateSpawnPosition(SpawnType spawnPositionType,AnimalController parent = null)
        {
            if (parent != null && spawnPositionType == SpawnType.NearParent)
            {
                Vector3 parentPosition = parent.transform.position;
                return new Vector3(parentPosition.x + Random.Range(10, 30), 0,
                    parentPosition.z + Random.Range(10, 30));
            }
            else{
                return new Vector3(Random.Range(-5 * environmentData.MapSize, 5 * environmentData.MapSize), 0,
                        Random.Range(-5 * environmentData.MapSize, 5 * environmentData.MapSize));
            }
        }

        private void OnDead(AnimalController animalController)
        {
            animalController.Died.RemoveListener(OnDead);
            activeAnimalControllers.Remove(animalController);

            EvaluateFitness(animalController);

            if (animalController.Fitness >= FitnessToScore)
            {
                animalsScoredFitness += 1;
                
                if (animalController.Fitness >= bestFitness)
                {
                    if (animalController.Fitness > bestFitness)
                        genomeParser.SaveToJson(animalController.Brain, animalController.DNA);
                    bestFitness = animalController.Fitness;
                    Debug.LogWarning("Best fitness: " + bestFitness);
                }

                // Spawn animals from parents without reproduction action
                /*if (FitnessToScore <= 30)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        savedAnimalControllers.Add(CreateAnimalObject(animalController.Key,
                                            animalController.Generation + 1, false, GenomeType.Parent,
                                            SpawnType.Random, animalController));
                    }
                }*/

                if (animalsScoredFitness >= 50)
                {
                    Debug.LogWarning($"[{MainController.pastTimeSteps}]50 Animals Scored " + FitnessToScore);
                    animalController.NewLevel = 1;
                    FitnessToScore += 5;
                    animalsScoredFitness = 0;
                }

                // Delete some saved brains, if there are to many
                if (savedAnimalControllers.Count > brainBuffer)
                {
                    for (int i = brainBuffer; i < savedAnimalControllers.Count; i++)
                    {
                        Destroy(savedAnimalControllers[i]);
                    }
                    Debug.LogWarning($"{savedAnimalControllers.Count - brainBuffer} animals removed from buffer");
                    savedAnimalControllers.RemoveRange(brainBuffer, savedAnimalControllers.Count - brainBuffer);
                }
            }
            
            // Spawn saved or new animal, when there aren't enough on the map
            if (activeAnimalControllers.Count < environmentData.MaxAnimalAmount)
            {
                if ((Random.value >= newGenomesAmount || MainController.pastTimeSteps > stopRespawnTime) && savedAnimalControllers.Count > 0)
                {
                    while (activeAnimalControllers.Count < environmentData.MaxAnimalAmount && savedAnimalControllers.Count > 0)
                    {
                        GameObject parent = savedAnimalControllers[0];
                        SpawnAnimal(parent, SpawnType.Random);
                        savedAnimalControllers.RemoveAt(0);
                    }
                    
                }
                else if (MainController.pastTimeSteps < stopRespawnTime)
                {
                    CreateAnimalObject(nextKey, 0, true, GenomeType.Random, SpawnType.Random);
                    //CreateAnimalObject(nextKey, 0, true, GenomeType.Frozen, SpawnType.Random);
                }
            }

        }

        private void EvaluateFitness(AnimalController animal)
        {
            animal.EvaluateFitness();
        }
    }
}