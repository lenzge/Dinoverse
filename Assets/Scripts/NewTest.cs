using System.IO;
using UnityEngine;
using Python.Runtime;

namespace DefaultNamespace
{
    public class NewTest : MonoBehaviour
    {
        private void Awake()
        {
            Runtime.PythonDLL = @"C:\Users\Lena Sophie\AppData\Local\Programs\Python\Python39\python39.dll";
        }

        // Start is called before the first frame update
        void Start()
        {
            
            PythonEngine.Initialize();
            //PythonRunner.RunFile("/Python/HelloWorld");
            using (Py.GIL())
            {
                dynamic py_sys = Py.Import("sys");
                
                string site_pkg =
                    @"C:\Users\Lena Sophie\Desktop\Game Dev\Dinoverse\Packages\python_net\Lib\site-packages";
                py_sys.path.insert(0, Path.Combine(Application.streamingAssetsPath, site_pkg));
                
                dynamic neat = PyModule.Import("neat");
                Debug.Log(neat.StatisticsReporter());
            }
        }
    }
}