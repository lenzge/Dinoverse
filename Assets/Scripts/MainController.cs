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
            int mapSize = EnvironmentData.MapSize / 20;
            environment.transform.localScale = new Vector3(mapSize, mapSize, 6.8f);
            animalCreator.StartGame();
            environmentCreator.StartGame();
        }

        protected override void TimedUpdate()
        {
            pastTimeSteps += 1;
        }
    }
}