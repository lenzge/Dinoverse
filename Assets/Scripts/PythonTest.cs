using System;
using System.IO;
using UnityEngine;
using Python.Runtime;

    public class PythonTest : MonoBehaviour
    {
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
                reload.delete_module("my_script");

                // Use a Python class
                dynamic myModule = Py.Import("my_script");
                dynamic myClass = myModule.MyClass();
                var result = myClass.my_function(5);
                Debug.Log(result);
                
                // No need anymore?
                //string site_pkg =
                    //@"C:\Users\Lena Sophie\Desktop\Game Dev\Dinoverse\Packages\python_net\Lib\site-packages";
                //sys.path.insert(0, Path.Combine(Application.streamingAssetsPath, site_pkg));
                
                // Use a imported module 
                dynamic neat = PyModule.Import("neat");
                Debug.Log(neat.StatisticsReporter());
                
            }
        }
    }