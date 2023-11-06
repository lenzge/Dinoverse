using System.Collections;
using System.Collections.Generic;
using Animals;
using Python.Runtime;
using UnityEngine;

namespace DefaultNamespace
{
    public class NeatController : MonoBehaviour
    {
        [SerializeField] private GameObject animalPrefab;
        [SerializeField] private int mapSize;
        [SerializeField] private Transform[] Islands;

        private dynamic neatController;
        
        int emptyIslands = 0;
        
        private void Awake()
        {
            Runtime.PythonDLL = @"C:\Users\Lena Sophie\AppData\Local\Programs\Python\Python312\python312.dll";
        }

        // Start is called before the first frame update
        void Start()
        {
            emptyIslands = 0;
            PythonEngine.Initialize();

            using (Py.GIL())
            {

                //dynamic np = Py.Import("numpy");
                //Debug.Log(np.cos(np.pi * 2));
                
                // Add directory with python scripts to sys
                string scriptPath = @"C:\Users\Lena Sophie\Desktop\Game Dev\Dinoverse\Assets\Scripts\Python";
                dynamic sys = Py.Import("sys");
                sys.path.append(scriptPath);

                // Use the reload class to delete all cached modules
                dynamic reload = Py.Import("reload");
                reload.delete_module("ai");

                // Use a Python class
                dynamic ai = Py.Import("ai");
                neatController = ai.NeatController();
                neatController.run();

                //dynamic animalBrains = neatController.animals;
                // Spawn animals after each other
                //StartCoroutine(Run(animalBrains));
                StartCoroutine(RunIslands(Islands));
            }
        }

        IEnumerator RunIslands(Transform[] islands)
        {
            int currentGeneration = 0;
            int generations = 400;
            int currentIsland = 0;
            while (currentGeneration < generations)
            {
                currentGeneration += 1;
                neatController.create_generation();
                dynamic animalBrains = neatController.animals;
                foreach (dynamic animalBrain in animalBrains)
                {
                    AnimalController animalController = SpawnAnimal(animalBrain, currentGeneration, islands[currentIsland]);
                    animalController.Dead.AddListener(OnDeadAnimal);
                    currentIsland += 1;

                    if (currentIsland == islands.Length)
                    {
                        currentIsland = 0;
                        emptyIslands = 0;
                        yield return new WaitUntil(() => emptyIslands == islands.Length);
                    }
                    
                }
                
                dynamic generationWinner = neatController.evaluate();
                Debug.Log($"Winner of Generation {currentGeneration}: " + generationWinner);
            }
            
            Debug.Log("Overall Winner: " + neatController.return_winner());
        }

        private void OnDeadAnimal()
        {
            emptyIslands += 1;
        }
        
        IEnumerator Run()
        {
            int currentGeneration = 0;
            int generations = 400;
            while (currentGeneration < generations)
            {
                currentGeneration += 1;
                neatController.create_generation();
                dynamic animalBrains = neatController.animals;
                foreach (dynamic animalBrain in animalBrains)
                {
                    bool eventInvoked = false;
                    AnimalController animalController = SpawnAnimal(animalBrain, currentGeneration, neatController);
                    animalController.Dead.AddListener(()=> eventInvoked = true);
                    yield return new WaitUntil(() => eventInvoked);
                }

                dynamic generationWinner = neatController.evaluate();
                Debug.Log($"Winner of Generation {currentGeneration}: " + generationWinner);
            }
            
            Debug.Log("Overall Winner: " + neatController.return_winner());
            
        }
        
        private void SpawnAnimalRandom(dynamic animalBrain)
        {
            Vector3 randomPosition = new Vector3(Random.Range(-5 * mapSize, 5 * mapSize), 0,
                Random.Range(-5 * mapSize, 5 * mapSize));
            GameObject newAnimal = Instantiate(animalPrefab, randomPosition, Quaternion.identity);
            newAnimal.GetComponent<AnimalController>().Brain = animalBrain;
        }
        
        private AnimalController SpawnAnimal(dynamic animalBrain, int generation, Transform island)
        {
            Vector3 randomPosition = island.position;
            GameObject newAnimal = Instantiate(animalPrefab, randomPosition, Quaternion.identity);
            newAnimal.GetComponent<AnimalController>().Brain = animalBrain;
            newAnimal.GetComponent<AnimalController>().Generation = generation;
            return newAnimal.GetComponent<AnimalController>();
        }
    }
}