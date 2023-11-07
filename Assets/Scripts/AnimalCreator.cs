using System.Collections.Generic;
using Animals;
using UnityEngine;

namespace DefaultNamespace
{
    public class AnimalCreator : MonoBehaviour
    {
        [SerializeField] private GameObject animalPrefab;
        [SerializeField] private int mapSize;
        [SerializeField] private int initialAmount;
        //[SerializeField] private int offspringAmount;

        [SerializeField] private List<AnimalController> animalControllers = new List<AnimalController>();
        
        public void Start()
        {
            CreateNewGeneration();
        }

        private void CreateNewGeneration()
        {
            Debug.LogWarning("Create new Generation");
            for (int i = 0; i < initialAmount; i++)
            {
                SpawnAnimal();
            }
        }

        public AnimalController SpawnAnimal()
        {
            Vector3 randomPosition = new Vector3(Random.Range(-5 * mapSize, 5 * mapSize), 0,
                Random.Range(-5 * mapSize, 5 * mapSize));
            Vector3 newPosition = transform.position + randomPosition;
            GameObject animal = Instantiate(animalPrefab, newPosition, Quaternion.identity);
            animalControllers.Add(animal.GetComponent<AnimalController>());
            animal.GetComponent<AnimalController>().AlsoDead.AddListener(OnDead);
            return animal.GetComponent<AnimalController>();
        }

        private void OnDead(AnimalController animalController)
        {
            animalController.AlsoDead.RemoveListener(OnDead);
            animalControllers.Remove(animalController);

            if (animalControllers.Count == 0)
            {
                CreateNewGeneration();
            }
            
        }
    }
}