using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class EnvironmentCreator : TimeBasedBehaviour
    {
        [SerializeField] private GameObject treePrefab;
        [SerializeField] private int mapSize;
        [SerializeField] private int initialAmount;
        [SerializeField] private int offspringAmount;

        [SerializeField] private List<GameObject> naturePrefabs;
        [SerializeField] private int natureAmount;
        [SerializeField] private List<GameObject> grassPrefabs;
        [SerializeField] private int grassAmount;
        
        
        private Collider[] colliderBuffer = new Collider[1];

        protected override void TimedStart()
        {
            for (int i = 0; i < initialAmount; i++)
            {
                SpawnNurture(treePrefab);
            }
            
            for (int i = 0; i < natureAmount; i++)
            {
                foreach (var prefab in naturePrefabs)
                {
                    SpawnNature(prefab);
                }
            }
            
            for (int i = 0; i < grassAmount; i++)
            {
                foreach (var prefab in grassPrefabs)
                {
                    SpawnNature(prefab);
                }
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
            while (true)
            {
                var randomRotation = Quaternion.Euler( 0,Random.Range(0, 360) , 0);
                Vector3 randomPosition = new Vector3(Random.Range(-5 * mapSize, 5 * mapSize), 5.6f,
                    Random.Range(-5 * mapSize, 5 * mapSize));
                GameObject nurture = Instantiate(prefab, randomPosition, randomRotation);
                if (Physics.OverlapSphereNonAlloc(nurture.transform.position, 2, colliderBuffer,
                    1 << (int) Layer.Water) >= 1)
                {
                    Destroy(nurture);
                }
                else
                {
                    nurture.GetComponent<Nurture>().NurtureEatenEvent.AddListener(SpawnNewNurture);
                    break;
                }
            }
        }
        
        private void SpawnNature(GameObject prefab)
        {
            while (true)
            {
                Vector3 randomPosition = new Vector3(Random.Range(-5 * mapSize, 5 * mapSize), 1.06f,
                    Random.Range(-5 * mapSize, 5 * mapSize));
                var randomRotation = Quaternion.Euler( -90,Random.Range(0, 360) , 0);
                GameObject nature = Instantiate(prefab, randomPosition, randomRotation);
                if (Physics.OverlapSphereNonAlloc(nature.transform.position, 2, colliderBuffer,
                    1 << (int) Layer.Water) >= 1)
                {
                    Destroy(nature);
                }
                else
                {
                    break;
                }
            }
        }
        
        public void SpawnNurtureSeed(GameObject prefab, Vector3 position, int randomOffset)
        {
            while (true)
            {
                Vector2 rando = RNG.RandomDonut(55, 20, randomOffset);
                Vector3 randomPosition = new Vector3(position.x + rando.x, 4,
                         position.z + rando.y);
                
                Vector3 newPosition = transform.position + randomPosition;
                GameObject nurture = Instantiate(prefab, newPosition, Quaternion.identity);
                if (Physics.OverlapSphereNonAlloc(nurture.transform.position, 2, colliderBuffer,
                    1 << (int) Layer.Water) >= 1)
                {
                    Destroy(nurture);
                }
                else
                {
                    nurture.GetComponent<Nurture>().NurtureEatenEvent.AddListener(SpawnNewNurture);
                    break;
                }
            }
        }
        
        private void SpawnNewNurture(Nurture oldNurture, GameObject prefab)
        {
            oldNurture.NurtureEatenEvent.RemoveListener(SpawnNewNurture);
            SpawnNurture(prefab);
        }
    }
}