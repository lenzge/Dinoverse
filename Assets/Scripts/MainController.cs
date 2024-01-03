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
        }

        protected override void TimedUpdate()
        {
            pastTimeSteps += 1;
        }
    }
}