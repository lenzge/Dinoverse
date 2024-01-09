using UnityEngine;
using Util;

namespace DefaultNamespace
{
    public class MainController : TimeBasedBehaviour
    {
        public static int pastTimeSteps;

        [SerializeField] private AnimalCreator animalCreator;
        [SerializeField] private EnvironmentCreator environmentCreator;
        [SerializeField] private GameObject environment;

        public void StartGame()
        {
            environment.transform.localScale = new Vector3(EnvironmentData.MapSize / 20f, EnvironmentData.MapSize / 20f,
                EnvironmentData.MapSize / 20f);
            Physics.SyncTransforms();
            
            pastTimeSteps = 0;
            environmentCreator.StartGame();
            animalCreator.StartGame();
        }

        protected override void TimedUpdate()
        {
            pastTimeSteps += 1;
        }
    }
}