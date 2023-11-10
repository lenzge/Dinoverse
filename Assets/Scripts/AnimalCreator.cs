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

        private int currentGeneration;

        public void Start()
        {
            currentGeneration = 0;
            CreateNewGeneration();
        }

        private void CreateNewGeneration()
        {
            currentGeneration += 1;
            Debug.LogWarning($"Create new Generation ({currentGeneration})");
            for (int i = 0; i < initialAmount; i++)
            {
                AnimalController animalController = SpawnAnimal(i, 0);
                Random.InitState((int)System.DateTime.Now.Ticks);
                animalController.MutationAmount = Random.Range(0.1f, 0.8f);
                animalController.MutationChance = Random.Range(0.05f, 0.4f);
                int numRaycasts = Random.Range(4, 8);
                animalController.Eyes.SetEyeParams(numRaycasts, Random.Range(10,30), Random.Range(100,400));
                animalController.Brain.CreateBrain(new []{numRaycasts + 1, 32,32, 2});
                animalController.MutateCreature();
            }
        }
        
        

        public AnimalController SpawnAnimal(int key, int grandChild)
        {
            Vector3 randomPosition = new Vector3(Random.Range(-5 * mapSize, 5 * mapSize), 0,
                Random.Range(-5 * mapSize, 5 * mapSize));
            Vector3 newPosition = transform.position + randomPosition;
            GameObject animal = Instantiate(animalPrefab, newPosition, Quaternion.identity);
            AnimalController controller = animal.GetComponent<AnimalController>();
            controller.Key = key;
            controller.Generation = currentGeneration;
            controller.GrandChild = grandChild;
            controller.UpdateName();
            animalControllers.Add(controller);
            controller.AlsoDead.AddListener(OnDead);
            return controller;
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
    }
}