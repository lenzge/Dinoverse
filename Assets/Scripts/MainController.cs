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
            pastTimeSteps = 0;
            animalCreator.StartGame();
            environmentCreator.StartGame();

            environment.transform.localScale = new Vector3(EnvironmentData.MapSize / 20f, EnvironmentData.MapSize / 20f,
                EnvironmentData.MapSize / 20f);
        }

        protected override void TimedUpdate()
        {
            pastTimeSteps += 1;
        }
    }
}