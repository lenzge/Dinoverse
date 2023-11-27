﻿using System.Collections.Generic;
using System.Linq;
using Animal;
using Enums;
using UnityEngine;
using Util;

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

        [SerializeField] private List<AnimalController> animalControllers = new List<AnimalController>();

        private int currentPopulation;
        private int nextKey;
        public int pastTimeSteps;
        private Collider[] colliderBuffer = new Collider[1];
        private List<GameObject> savedAnimalControllers = new List<GameObject>();
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
            CreateNewGeneration();
        }

        protected override void TimedUpdate()
        {
            pastTimeSteps += 1;
        }

        public void SpawnChildAnimal(int key, int generation, AnimalController parent, int randomOffset)
        {
            Random.InitState(randomOffset);
            Vector3 spawnPosition = new Vector3(Random.Range(-5 * mapSize, 5 * mapSize), 0,
                Random.Range(-5 * mapSize, 5 * mapSize));
            //Vector3 parentPosition = parent.transform.position;
            //Vector3 spawnPosition = new Vector3(parentPosition.x + Random.Range(10, 30), 0,
                //parentPosition.z + Random.Range(10, 30));
            AnimalController childController = SpawnAnimal(key, generation, spawnPosition);
            childController.DNA.CopyValuesFrom(parent.DNA);
            childController.Brain.Layers = parent.Brain.CopyLayers();
            childController.DNA.Mutate();
            childController.InitOrgans(true);
        }
        

        private void CreateNewGeneration()
        {
            currentPopulation += 1;
            Debug.LogWarning($"Create new Generation ({currentPopulation})");
            for (int i = 0; i < initialAmount; i++)
            {
                SpawnNewPopAnimal(nextKey, 0);
            }
        }

        private void SpawnNewPopAnimal(int key, int generation)
        {
            nextKey += 1;
            while (true)
            {
                Vector3 spawnPosition = new Vector3(Random.Range(-5 * mapSize, 5 * mapSize), 0,
                    Random.Range(-5 * mapSize, 5 * mapSize));
                AnimalController animalController = SpawnAnimal(key, generation, spawnPosition);
                if (Physics.OverlapSphereNonAlloc(animalController.transform.position, 2, colliderBuffer,
                    1 << (int) Layer.Water) >= 1)
                {
                    animalControllers.Remove(animalController);
                    animalController.Died.RemoveListener(OnDead);
                    Destroy(animalController.gameObject);
                }
                else
                {
                    animalController.DNA.CreateNewDNA();
                    //Genome genome = GenomeParser.LoadFromJson();
                    //genome.LoadGenome(animalController.Brain, animalController.DNA);
                    animalController.InitOrgans(false);
                    break;
                }
            }
            
        }

        private AnimalController SpawnAnimal(int key, int generation, Vector3 spawnPosition)
        {
            //var randomRotation = Quaternion.Euler( 0,Random.Range(0, 360) , 0);
            GameObject animal = Instantiate(animalPrefab, spawnPosition, Quaternion.identity);
            AnimalController controller = animal.GetComponent<AnimalController>();
            controller.Key = key;
            controller.Population = currentPopulation;
            controller.Generation = generation;
            controller.UpdateName();
            animalControllers.Add(controller);
            controller.Died.AddListener(OnDead);
            return controller;
        }

        private GameObject CreateChildAnimal(int key, int generation, AnimalController parent)
        {
            GameObject animal = Instantiate(animalPrefab);
            AnimalController controller = animal.GetComponent<AnimalController>();
            controller.Key = key;
            controller.Population = currentPopulation;
            controller.Generation = generation;
            controller.UpdateName();
            controller.Died.AddListener(OnDead);
            controller.DNA.CopyValuesFrom(parent.DNA);
            controller.Brain.Layers = parent.Brain.CopyLayers();
            controller.DNA.Mutate();
            controller.InitOrgans(true);
            animal.SetActive(false);
            return animal;
        }

        private void SpawnAnimal(GameObject animal)
        {
            AnimalController controller = animal.GetComponent<AnimalController>();
            animalControllers.Add(controller);
            while (true)
            {
                Vector3 spawnPosition = new Vector3(Random.Range(-5 * mapSize, 5 * mapSize), 0,
                    Random.Range(-5 * mapSize, 5 * mapSize));
                animal.transform.position = spawnPosition;
                if (Physics.OverlapSphereNonAlloc(controller.transform.position, 2, colliderBuffer,
                    1 << (int) Layer.Water) >= 1)
                {
                    
                }
                else
                {
                    animal.SetActive(true);
                    break;
                }
            }
        }

        private void OnDead(AnimalController animalController)
        {
            animalController.Died.RemoveListener(OnDead);
            animalControllers.Remove(animalController);
            
            //GenomeParser.SaveToJson(animalController.Brain, animalController.DNA);

            if (animalController.Fitness > 0)
            {
                if (animalController.Fitness >= bestFitness)
                {
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
                        savedAnimalControllers.Insert(0,CreateChildAnimal(animalController.Key,
                            animalController.Generation + 1, animalController));
                    }
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        savedAnimalControllers.Add(CreateChildAnimal(animalController.Key,
                            animalController.Generation + 1, animalController));
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

            if (pastTimeSteps < stopRespawn && animalControllers.Count < initialAmount)
            {
                if (Random.value >= newGenomes && savedAnimalControllers.Count > 0)
                {
                    GameObject parent = savedAnimalControllers[0];
                    SpawnAnimal(parent);
                    savedAnimalControllers.RemoveAt(0);
                }
                else
                {
                    SpawnNewPopAnimal(nextKey, 0);
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