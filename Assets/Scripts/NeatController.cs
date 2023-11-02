using Animals;
using Python.Runtime;
using UnityEngine;

namespace DefaultNamespace
{
    public class NeatController : MonoBehaviour
    {
        [SerializeField] private GameObject animalPrefab;
        [SerializeField] private int mapSize;
        
        
        private void Awake()
        {
            Runtime.PythonDLL = @"C:\Users\Lena Sophie\AppData\Local\Programs\Python\Python312\python312.dll";
        }

        // Start is called before the first frame update
        void Start()
        {
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
                dynamic neatController = ai.NeatController();
                neatController.run();
                
                dynamic animalBrains = neatController.animals;
                foreach (dynamic animalBrain in animalBrains)
                {
                    SpawnAnimal(animalBrain);
                }
            }
        }
        
        private void SpawnAnimal(dynamic animalBrain)
        {
            Vector3 randomPosition = new Vector3(Random.Range(-5 * mapSize, 5 * mapSize), 0,
                Random.Range(-5 * mapSize, 5 * mapSize));
            GameObject newAnimal = Instantiate(animalPrefab, randomPosition, Quaternion.identity);
            newAnimal.GetComponent<AnimalController>().Brain = animalBrain;
        }
    }
}