using System;
using System.IO;
using UnityEngine;
using Python.Runtime;

    public class NewTest : MonoBehaviour
    {
        private void Awake()
        {
            Runtime.PythonDLL = @"C:\Users\Lena Sophie\AppData\Local\Programs\Python\Python39\python39.dll";
        }

        // Start is called before the first frame update
        void Start()
        {
            PythonEngine.Shutdown();
            PythonEngine.Initialize();
            //PythonRunner.RunFile("/Python/HelloWorld");
            using (Py.GIL())
            {
                
                
                string scriptPath = @"C:\Users\Lena Sophie\Desktop\Game Dev\Dinoverse\Assets\Scripts\Python";

                // Füge den Pfad zum Systempfad hinzu
                dynamic sys = Py.Import("sys");
                sys.path.append(scriptPath);
                
                dynamic myModule = Py.Import("numtest");

                // Instanziiere die Klasse
                dynamic myClass = myModule.MyClass();

                // Rufe die Funktion auf
                var result = myClass.my_function(5);

                Debug.Log(result);
                
                //string site_pkg =
                    //@"C:\Users\Lena Sophie\Desktop\Game Dev\Dinoverse\Packages\python_net\Lib\site-packages";
                //sys.path.insert(0, Path.Combine(Application.streamingAssetsPath, site_pkg));
                
                dynamic neat = PyModule.Import("neat");
                Debug.Log(neat.StatisticsReporter());
                
            }
        }

        private void OnDestroy()
        {
            PythonEngine.Shutdown();
        }
    }