﻿using System;
using System.Collections.Generic;
using System.Linq;
using Animal;
using Enums;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class AnimalCreator : TimeBasedBehaviour
    {
        [SerializeField] private GameObject animalPrefab;
        [SerializeField] private int mapSize;
        [SerializeField] private int initialAmount;
        [SerializeField] private int minAmount;
        [SerializeField] public int stopRespawn;
        [SerializeField] public int BrainBuffer;
        [SerializeField] public GenomeParser GenomeParser;

        private List<AnimalController> animalControllers = new List<AnimalController>();

        private int currentPopulation;
        private int nextKey;
        public int pastTimeSteps;
        private Collider[] colliderBuffer = new Collider[1];
        private List<GameObject> savedAnimalControllers = new List<GameObject>();
        private List<Genome> frozenGenomes = new List<Genome>();
        private int bestFitness;
        public int FitnessToScore;
        private int animalsScoredFitness;
        private float newGenomes;
        
        public int DrowningPunishment;
        public int MaxTrees;
        public int MinTrees;
        public int LakeCount;
        public int ReproductionEnergy;
        public int ReproductionRadius;
        public bool SelfReproduction;
        public bool MutualReproduction;

        protected override void TimedStart()
        {
            currentPopulation = 0;
            nextKey = 0;
            pastTimeSteps = 0;
            
            bestFitness = 0;
            FitnessToScore = 3;
            animalsScoredFitness = 0;

            MaxTrees = 160;
            MinTrees = 100;
            LakeCount = 8; //12
            ReproductionEnergy = 1;
            ReproductionRadius = 170;
            SelfReproduction = true;
            MutualReproduction = false;
            
            newGenomes = 0.5f;
            DrowningPunishment = 1;
            frozenGenomes = GenomeParser.LoadAllGenomes();
            CreateNewGeneration();
        }

        protected override void TimedUpdate()
        {
            pastTimeSteps += 1;
        }

        private void CreateNewGeneration()
        {
            currentPopulation += 1;
            Debug.LogWarning($"Create new Generation ({currentPopulation})");
            for (int i = 0; i < initialAmount; i++)
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
            animalControllers.Add(controller);
            controller.Died.AddListener(OnDead);
            //var randomRotation = Quaternion.Euler( 0,Random.Range(0, 360) , 0);
            while (true)
            {
                animal.transform.position = EvaluateSpawnPosition(spawnPositionType, parent);
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
                return new Vector3(Random.Range(-5 * mapSize, 5 * mapSize), 0,
                        Random.Range(-5 * mapSize, 5 * mapSize));
            }
        }

        private void OnDead(AnimalController animalController)
        {
            animalController.Died.RemoveListener(OnDead);
            animalControllers.Remove(animalController);

            EvaluateFitness(animalController);

            if (animalController.Fitness >= FitnessToScore)
            {
                animalsScoredFitness += 1;
                
                if (animalController.Fitness >= bestFitness)
                {
                    if (animalController.Fitness > bestFitness)
                        GenomeParser.SaveToJson(animalController.Brain, animalController.DNA);
                    bestFitness = animalController.Fitness;
                    Debug.LogWarning("Best fitness: " + bestFitness);
                }

                /*if (fitnessToScore <= 30)
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
                    Debug.LogWarning($"[{Mathf.FloorToInt(Time.time * EnvironmentData.TimeSpeed / 60f)}]50 Animals Scored " + FitnessToScore);
                    animalController.NewLevel = 1;
                    
                    if (FitnessToScore < 15) FitnessToScore += 3;
                    else if (FitnessToScore == 15)
                    {
                        ReproductionEnergy = 2;
                        FitnessToScore += 3;
                    }
                    else if (FitnessToScore == 18 || FitnessToScore == 21 || FitnessToScore == 24)
                    {
                        if (MaxTrees > MinTrees) MaxTrees -= 10;
                        if (LakeCount < 20) LakeCount += 1;
                        ReproductionRadius -= 10;
                        FitnessToScore += 3;
                    }
                    else if (FitnessToScore == 27)
                    {
                        ReproductionEnergy = 3;
                        FitnessToScore += 3;
                    }
                    else if (FitnessToScore == 30 || FitnessToScore == 33)
                    {
                        if (MaxTrees > MinTrees) MaxTrees -= 10;
                        if (LakeCount < 20) LakeCount += 1;
                        ReproductionRadius -= 10;
                        FitnessToScore += 3;
                    }
                    else if (FitnessToScore == 36)
                    {
                        ReproductionEnergy = 4;
                        FitnessToScore += 3;
                    }
                    else if (FitnessToScore > 39)
                    {
                        if (MaxTrees > MinTrees) MaxTrees -= 10;
                        if (LakeCount < 20) LakeCount += 1;
                        ReproductionRadius -= 10;
                        //if (fitnessToScore >= 40) SelfReproduction = false;
                        if (FitnessToScore >= 52) MutualReproduction = true;
                        FitnessToScore += 4;
                    }
                    animalsScoredFitness = 0;
                }

                // Delete some frozen brains, if they are to many
                if (savedAnimalControllers.Count > BrainBuffer)
                {
                    for (int i = BrainBuffer; i < savedAnimalControllers.Count; i++)
                    {
                        Destroy(savedAnimalControllers[i]);
                    }
                    Debug.LogWarning($"{savedAnimalControllers.Count - BrainBuffer} animals removed from buffer");
                    savedAnimalControllers.RemoveRange(BrainBuffer, savedAnimalControllers.Count - BrainBuffer);
                }
            }
            
            // Spawn saved or new animal, when there aren't enough on the map
            if (animalControllers.Count < initialAmount)
            {
                if (Random.value >= newGenomes && savedAnimalControllers.Count > 0)
                {
                    while (animalControllers.Count < initialAmount && savedAnimalControllers.Count > 0)
                    {
                        GameObject parent = savedAnimalControllers[0];
                        SpawnAnimal(parent, SpawnType.Random);
                        savedAnimalControllers.RemoveAt(0);
                    }
                    
                }
                else if (pastTimeSteps < stopRespawn)
                {
                    CreateAnimalObject(nextKey, 0, true, GenomeType.Random, SpawnType.Random);
                    //CreateAnimalObject(nextKey, 0, true, GenomeType.Frozen, SpawnType.Random);
                }
            }

        }

        public void EvaluateFitness(AnimalController animal)
        {
            animal.EvaluateFitness();
        }

        public int GetReproductionEnergy()
        {
            if (animalControllers.Count < 100)
            {
                return 1;
            }
            else if (animalControllers.Count < 300)
            {
                return 2;
            }
            else if (animalControllers.Count < 500)
            {
                return 3;
            }
            else
            {
                return 5;
            }
        }

        public int GetAnimalCount()
        {
            return animalControllers.Count;
        }
    }
}