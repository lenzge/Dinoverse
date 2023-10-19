using System;
using System.Collections;
using UnityEngine;

namespace DefaultNamespace
{
    public class TimeBasedBehaviour : MonoBehaviour
    {
        public EnvironmentData EnvironmentData;
        private float timeInterval;

        private void Start()
        {
            TimedStart();
            StartCoroutine(TimedUpdateRoutine());
        }

        protected virtual void TimedUpdate() {}
        protected virtual void TimedStart() {}

        IEnumerator TimedUpdateRoutine()
        {
            while (true)
            {
                timeInterval = 1f / EnvironmentData.timeSpeed;
                yield return new WaitForSeconds(timeInterval);
                TimedUpdate();
            }
        }
    }
}