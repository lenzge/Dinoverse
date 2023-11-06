using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class EnvironmentCreator : TimeBasedBehaviour
    {
        [SerializeField] private GameObject treePrefab;
        [SerializeField] private int mapSize;
        [SerializeField] private int initialAmount;
        [SerializeField] private int offspringAmount;

        protected override void TimedStart()
        {
            for (int i = 0; i < initialAmount; i++)
            {
                SpawnNurture(treePrefab);
            }
        }

        protected override void TimedSlowUpdate()
        {
            for (int i = 0; i < offspringAmount; i++)
            {
                //SpawnNurture(treePrefab);
            }
        }

        private void SpawnNurture(GameObject prefab)
        {
            Vector3 randomPosition = new Vector3(Random.Range(-5 * mapSize, 5 * mapSize), 4,
                Random.Range(-5 * mapSize, 5 * mapSize));
            Vector3 newPosition = transform.position + randomPosition;
            GameObject nurture = Instantiate(prefab, newPosition, Quaternion.identity);
            nurture.GetComponent<Nurture>().NurtureEatenEvent.AddListener(SpawnNewNurture);
        }
        
        private void SpawnNewNurture(Nurture oldNurture, GameObject prefab)
        {
            oldNurture.NurtureEatenEvent.RemoveListener(SpawnNewNurture);
            SpawnNurture(prefab);
        }
    }
}