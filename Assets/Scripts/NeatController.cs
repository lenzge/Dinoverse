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
        [SerializeField] private List<AnimalController> animalControllers = new List<AnimalController>();

        private List<AnimalController> bestGenomes = new List<AnimalController>();
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
                //StartCoroutine(RunIslands(Islands));
                RunAll();
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
        private void RunAll()
        {
            animalControllers.Clear();
            bestGenomes.Clear();
            
            neatController.create_generation();
            dynamic animalBrains = neatController.animals;
            foreach (dynamic animalBrain in animalBrains)
            {
                AnimalController animalController = SpawnAnimalRandom(animalBrain);
            }
            Debug.LogError($"There are {animalControllers.Count} animal safed");
        }

        private void OnDead(AnimalController animalController)
        {
            //Debug.LogError($"I'm dead");
            animalController.AlsoDead.RemoveListener(OnDead);
            animalControllers.Sort((c1, c2) => c2.Fitness.CompareTo(c1.Fitness));
            int index = animalControllers.IndexOf(animalController);
            if (index >= 0 && index < 10 && animalControllers[index].Fitness >= 100)
            {
                Debug.LogWarning($"One of the best Brains with fitness {animalController.Fitness} !");
                bestGenomes.Add(animalController);
            }
            animalControllers.Remove(animalController);
            //Debug.LogError($"There are {animalControllers.Count} animal left");
            Debug.LogWarning(animalControllers.Count);
            if (animalControllers.Count == 0)
            {
                CreateNewGeneration();
            }
            
        }

        public AnimalController GetBestGenome()
        {
            animalControllers.Sort((c1, c2) => c2.Fitness.CompareTo(c1.Fitness));
            Debug.LogWarning($"Best Partner has the Fitness of {animalControllers[0].Fitness}");
            return animalControllers[0];
        }

        private void CreateNewGeneration()
        {
            Debug.LogWarning($"Create new Generation. There are {bestGenomes.Count} Genomes saved.");
            
            bestGenomes.Sort((c1, c2) => c2.Fitness.CompareTo(c1.Fitness));
            if (bestGenomes.Count > 50)
            {
                bestGenomes = bestGenomes.GetRange(0, 50);
            }

            for (int i = 0; i < 2; i++)
            {
                if (bestGenomes.Count > i)
                {
                    SpawnAnimalRandom(bestGenomes[i].Brain.reproduce(bestGenomes[i].Brain));
                }
                
            }
            
            for (int i = 0; i < bestGenomes.Count - 1; i++)
            {
                AnimalController animalController = SpawnAnimalRandom(bestGenomes[i].Brain.reproduce(bestGenomes[i+1].Brain));
            }
            
            for (int i = 1; i < bestGenomes.Count - 1; i++)
            {
                AnimalController animalController = SpawnAnimalRandom(bestGenomes[i].Brain.reproduce(bestGenomes[i-1].Brain));
            }
        }
        
        public AnimalController SpawnAnimalRandom(dynamic animalBrain)
        {
            Vector3 randomPosition = new Vector3(Random.Range(-5 * mapSize, 5 * mapSize), 0,
                Random.Range(-5 * mapSize, 5 * mapSize));
            GameObject newAnimal = Instantiate(animalPrefab, randomPosition, Quaternion.identity);
            newAnimal.GetComponent<AnimalController>().Brain = animalBrain;
            animalControllers.Add(newAnimal.GetComponent<AnimalController>());
            newAnimal.GetComponent<AnimalController>().AlsoDead.AddListener(OnDead);
            //Debug.LogError("Spawn animal");
            return newAnimal.GetComponent<AnimalController>();
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