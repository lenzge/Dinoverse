using System;
using System.Collections;
using UnityEngine;

namespace DefaultNamespace
{
    public class TimeBasedBehaviour : MonoBehaviour
    {
        [SerializeField] protected float timeInterval;
        private void Start()
        {
            StartCoroutine(TimedUpdateRoutine());
            TimedStart();
            
        }

        protected virtual void TimedUpdate() {}
        protected virtual void TimedStart() {}

        IEnumerator TimedUpdateRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(timeInterval);
                TimedUpdate();
            }
        }
    }
}