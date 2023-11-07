using System;
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

        private void Start()
        {
            childCount = 0;
            reproductionEnergy = 0;
        }

        public bool TryToReproduce(AnimalController animalController, Transform characterTransform, dynamic brain, dynamic partnerBrain)
        {
            reproductionEnergy += 1;

            if (reproductionEnergy == 2)
            {
                childCount += 1;
                for (int i = 0; i < childCount; i++)
                {
                    AnimalController newAnimal = animalController.NeatController.SpawnAnimalRandom(brain.reproduce(partnerBrain));
                    Debug.LogWarning($"Reproduced! {newAnimal.Brain.return_genome()}");
                }
                reproductionEnergy = -childCount;
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