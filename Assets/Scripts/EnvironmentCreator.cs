using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Data/EnvironmentCreator")]
    public class EnvironmentCreator : ScriptableObject
    {
        [Header("References")]
        [SerializeField] private EnvironmentData environmentData;
        
        [Header("Nurture")]
        [SerializeField] private GameObject treePrefab;

        [Header("Nature")]
        [SerializeField] private List<GameObject> naturePrefabs;
        [SerializeField] private List<GameObject> grassPrefabs;
        [SerializeField] private GameObject lakePrefab;

        public int CurrentTreeCount;
        private Collider[] colliderBuffer = new Collider[1];

        public void StartGame()
        {
            CurrentTreeCount = 0;
            
            for (int i = 0; i < environmentData.LakeCount; i++)
            {
                SpawnNature(lakePrefab);
            }
            
            for (int i = 0; i < environmentData.InitialTreeAmount; i++)
            {
                SpawnNurture(treePrefab, true);
            }

            int natureAmount = environmentData.MapSize / 20;
            for (int i = 0; i < natureAmount ; i++)
            {
                foreach (var prefab in naturePrefabs)
                {
                    SpawnNature(prefab);
                }
            }

            int grassAmount = environmentData.MapSize / 3;
            for (int i = 0; i < grassAmount; i++)
            {
                foreach (var prefab in grassPrefabs)
                {
                    SpawnNature(prefab);
                }
            }
        }

        /*protected override void TimedSlowUpdate()
        {
            if (AnimalCreator.LakeCount > lakeAmount)
            {
                SpawnNature(lakePrefab);
                lakeAmount += 1;
            }
            for (int i = 0; i < offspringAmount; i++)
            {
                //SpawnNurture(treePrefab);
            }
        }*/

        public void SpawnNurture(GameObject prefab, bool firstGeneration = false)
        {
            for (int i = 0; i < 10; i++)
            {
                var randomRotation = Quaternion.Euler( 0,Random.Range(0, 360) , 0);
                Vector3 randomPosition = new Vector3(Random.Range(-5 * environmentData.MapSize, 5 * environmentData.MapSize), 5.6f,
                    Random.Range(-5 * environmentData.MapSize, 5 * environmentData.MapSize));
                GameObject nurture = Instantiate(prefab, randomPosition, randomRotation);
                if (Physics.OverlapSphereNonAlloc(nurture.transform.position, 2, colliderBuffer,
                    1 << (int) Layer.Water) >= 1)
                {
                    Destroy(nurture);
                }
                else
                {
                    CurrentTreeCount += 1;
                    nurture.GetComponent<Nurture>().NurtureEatenEvent.AddListener(SpawnNewNurture);
                    if (firstGeneration) nurture.GetComponent<Nurture>().FirstGeneration = true;
                    else nurture.GetComponent<Nurture>().FirstGeneration = false;
                    return;
                }
            }
            //Debug.LogWarning("tree cant find a spot. mapsize: "+ environmentData.MapSize);
        }
        
        private void SpawnNature(GameObject prefab)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector3 randomPosition = new Vector3(Random.Range(-5 * environmentData.MapSize, 5 * environmentData.MapSize), 0f,
                    Random.Range(-5 * environmentData.MapSize, 5 * environmentData.MapSize));
                var randomRotation = Quaternion.Euler( -90,Random.Range(0, 360) , 0);
                GameObject nature = Instantiate(prefab, randomPosition, randomRotation);
                if (Physics.OverlapSphereNonAlloc(nature.transform.position, 2, colliderBuffer,
                    1 << (int) Layer.Water) >= 1 && colliderBuffer[0].gameObject != nature)
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
            for (int i = 0; i < 10; i++)
            {
                var randomRotation = Quaternion.Euler( 0,Random.Range(0, 360) , 0);
                Vector2 rando = RNG.RandomDonut(60, 25, randomOffset);
                //Vector2 rando = new Vector2(Random.Range(5, 30),Random.Range(7, 26));
                Vector3 randomPosition = new Vector3(position.x + rando.x, 4,
                         position.z + rando.y);
                
                Vector3 newPosition = randomPosition;
                GameObject nurture = Instantiate(prefab, newPosition, randomRotation);
                if (Physics.OverlapSphereNonAlloc(nurture.transform.position, 2, colliderBuffer,
                    1 << (int) Layer.Water) >= 1)
                {
                    Destroy(nurture);
                }
                else
                {
                    CurrentTreeCount += 1;
                    nurture.GetComponent<Nurture>().NurtureEatenEvent.AddListener(SpawnNewNurture);
                    nurture.GetComponent<Nurture>().FirstGeneration = false;
                    break;
                }
            }
        }
        
        private void SpawnNewNurture(Nurture oldNurture, GameObject prefab)
        {
            CurrentTreeCount -= 1;
            oldNurture.NurtureEatenEvent.RemoveListener(SpawnNewNurture);
            if (environmentData.ConstantTreeAmount && CurrentTreeCount < environmentData.MaxTrees || 
                CurrentTreeCount < environmentData.MinTrees)
            {
                SpawnNurture(prefab);
            }
        }
    }
}