using System;
using AI;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Animals
{
    public class ReproductionManager : MonoBehaviour
    {
        [SerializeField] public GameObject AnimalPrefab;
        [SerializeField] private int MaxChildCount;
        [SerializeField] private int mapSize;

        private int childCount = 0;
        private int reproductionEnergy = 0;
        private AnimalCreator animalCreator;

        private void Start()
        {
            childCount = 0;
            reproductionEnergy = 0;
            animalCreator = GameObject.Find("AnimalCreator").GetComponent<AnimalCreator>();
        }

        public bool TryToReproduce(AnimalController animalController)
        {
            reproductionEnergy += 1;

            if (reproductionEnergy == 2)
            {
                childCount += 2;
                for (int i = 0; i < childCount; i++)
                {
                    //create a new agent, and set its position to the parent's position + a random offset in the x and z directions (so they don't all spawn on top of each other)
                    AnimalController child = animalCreator.SpawnAnimal();
            
                    //copy the parent's neural network to the child
                    child.Brain.layers = animalController.Brain.copyLayers();
                    Debug.LogWarning($"Reproduced!");
                }
                reproductionEnergy = 0;
                return true;
            }

            return false;
        }

        public bool IsInMenopause()
        {
            if (childCount == MaxChildCount)
            {
                return true;
            }

            return false;
        }

        public int GetChildCount()
        {
            return childCount;
        }
        
        private void BearAnimal(dynamic animalBrain, Vector3 position)
        {
            //Vector3 spawnPosition = new Vector3(position.x + Random.Range(2,10), 0,
                //position.z + Random.Range(2,10));
            Vector3 spawnPosition = new Vector3(Random.Range(-5 * mapSize, 5 * mapSize), 0,
                Random.Range(-5 * mapSize, 5 * mapSize));
            GameObject newAnimal = Instantiate(AnimalPrefab, spawnPosition, Quaternion.identity);
            newAnimal.GetComponent<AnimalController>().Brain = animalBrain;
        }
    }
}