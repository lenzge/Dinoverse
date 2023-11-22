using System;
using System.Collections;
using DefaultNamespace;
using UnityEngine;

namespace Util
{
    public class TimeBasedBehaviour : MonoBehaviour
    {
        public EnvironmentData EnvironmentData;
        private float timeInterval;
        private float slowTimeIntervalValue = 5;
        private float slowTimeInterval;

        private void Start()
        {
            TimedStart();
            StartCoroutine(TimedUpdateRoutine());
            StartCoroutine(TimedSlowUpdateRoutine());
            
        }

        protected virtual void TimedUpdate() {}
        
        protected virtual void TimedSlowUpdate() {}
        protected virtual void TimedStart() {}

        IEnumerator TimedUpdateRoutine()
        {
            while (true)
            {
                timeInterval = 1f / EnvironmentData.TimeSpeed;
                yield return new WaitForSeconds(timeInterval);
                TimedUpdate();
            }
        }
        
        IEnumerator TimedSlowUpdateRoutine()
        {
            while (true)
            {
                slowTimeInterval = slowTimeIntervalValue / EnvironmentData.TimeSpeed;
                yield return new WaitForSeconds(slowTimeInterval);
                TimedSlowUpdate();
            }
        }
    }
}