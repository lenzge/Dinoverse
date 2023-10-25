using Animals;
using Python.Runtime;
using UnityEngine;

namespace DefaultNamespace
{
    public class NeatController : MonoBehaviour
    {
        public GameObject AnimalPrefab;
        
        private void Awake()
        {
            Runtime.PythonDLL = @"C:\Users\Lena Sophie\AppData\Local\Programs\Python\Python39\python39.dll";
        }

        // Start is called before the first frame update
        void Start()
        {
            PythonEngine.Initialize();

            using (Py.GIL())
            {

                // Add directory with python scripts to sys
                string scriptPath = @"C:\Users\Lena Sophie\Desktop\Game Dev\Dinoverse\Assets\Scripts\Python";
                dynamic sys = Py.Import("sys");
                sys.path.append(scriptPath);

                // Use the reload class to delete all cached modules
                dynamic reload = Py.Import("reload");
                reload.delete_module("brain");

                // Use a Python class
                dynamic config = Py.Import("brain");
                dynamic neatController = config.NeatController();
                neatController.run();
                dynamic neuralAnimal = neatController.animals[0];
                GameObject newAnimal = Instantiate(AnimalPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                newAnimal.GetComponent<AnimalController>().Brain = neuralAnimal;
                //Debug.Log(neuralAnimal.survive(1, 1, 2, 2));

            }
        }
    }
}