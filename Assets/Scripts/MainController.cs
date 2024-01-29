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
        [SerializeField] private Plot plot;

        private bool isStarted;

        public void StartGame()
        {
            isStarted = false;
            environment.transform.localScale = new Vector3(EnvironmentData.MapSize / 20f, EnvironmentData.MapSize / 20f,
                EnvironmentData.MapSize / 20f);
            Physics.SyncTransforms();
            
            pastTimeSteps = 0;
            environmentCreator.StartGame();
            animalCreator.StartGame();
            plot.StartGame();
            isStarted = true;
        }

        protected override void TimedUpdate()
        {
            pastTimeSteps += 1;

            if (pastTimeSteps == animalCreator.stopRespawnTime)
            {
                Debug.LogError($"{animalCreator.stopRespawnTime} Timesteps.No new animals");
            }
            
            if (pastTimeSteps == animalCreator.stopRespawnTime * 2)
            {
                Debug.LogError($"{animalCreator.stopRespawnTime*2} no extra kids");
            }
        }

        protected override void TimedSlowUpdate()
        {
            if (isStarted)
            {
                animalCreator.Classify();
            }
            
        }
    }
}