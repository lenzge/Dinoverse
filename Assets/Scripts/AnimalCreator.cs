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
        private float newGenomes;
        public int DrowningPunishment;

        protected override void TimedStart()
        {
            currentPopulation = 0;
            nextKey = 0;
            pastTimeSteps = 0;
            bestFitness = 0;
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
                //CreateAnimalObject(nextKey, 0, true, GenomeType.Random, SpawnType.Random);
                CreateAnimalObject(nextKey, 0, true, GenomeType.Frozen, SpawnType.Random);
            }
        }

        private GameObject CreateAnimalObject(int key, int generation, bool spawnInstant, GenomeType genomeType, 
            SpawnType spawnPositionType, AnimalController parent = null)
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
            
            if (animalController.Fitness > 0)
            {
                if (animalController.Fitness >= bestFitness)
                {
                    if (animalController.Fitness > bestFitness) GenomeParser.SaveToJson(animalController.Brain, animalController.DNA);
                    bestFitness = animalController.Fitness;
                    if (bestFitness >= 8)
                    {
                        DrowningPunishment = 10;
                        newGenomes = 0.1f;
                    }
                    else if (bestFitness >= 3)
                    {
                        DrowningPunishment = 5;
                        newGenomes = 0.25f;
                    }
                    Debug.LogWarning("Best fitness: "+bestFitness);
                    for (int i = 0; i < 6; i++)
                    {
                        savedAnimalControllers.Insert(0,CreateAnimalObject(animalController.Key,
                            animalController.Generation + 1, false, GenomeType.Parent,
                            SpawnType.Random, animalController));
                    }
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        savedAnimalControllers.Add(CreateAnimalObject(animalController.Key,
                            animalController.Generation + 1, false, GenomeType.Parent,
                            SpawnType.Random, animalController));
                    }
                }

                if (savedAnimalControllers.Count > BrainBuffer)
                {
                    for (int i = BrainBuffer; i < savedAnimalControllers.Count; i++)
                    {
                        Destroy(savedAnimalControllers[i]);
                    }
                    savedAnimalControllers.RemoveRange(BrainBuffer, savedAnimalControllers.Count - BrainBuffer);
                }
            }

            Debug.LogWarning(pastTimeSteps + " " + animalControllers.Count);
            if (pastTimeSteps < stopRespawn && animalControllers.Count < initialAmount)
            {
                Debug.LogWarning(newGenomes + " " + savedAnimalControllers.Count);
                if (Random.value >= newGenomes && savedAnimalControllers.Count > 0)
                {
                    GameObject parent = savedAnimalControllers[0];
                    SpawnAnimal(parent, SpawnType.Random);
                    savedAnimalControllers.RemoveAt(0);
                }
                else
                {
                    //CreateAnimalObject(nextKey, 0, true, GenomeType.Random, SpawnType.Random);
                    CreateAnimalObject(nextKey, 0, true, GenomeType.Frozen, SpawnType.Random);
                }
            }
            
            /*if (animalControllers.Count == 0)
            {
                pastTimeSteps = 0;
                nextKey = 0;
                CreateNewGeneration();
            }*/
            
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